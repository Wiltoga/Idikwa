using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    /// <summary>
    /// Factory to record and save audio streams
    /// </summary>
    public class SampleFactory
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SampleFactory()
        {
            Recorders = new List<RecorderWaveProvider>();
            TemporaryBuffers = new List<(TemporaryWaveStream, Task)>();
        }

        /// <summary>
        /// Recommended bit rates to save mp3 files
        /// </summary>
        public static int[] RecommendedBitRates { get; } = new[]
        {
            64000,
            96000,
            128000,
            192000,
            256000,
            320000
        };

        /// <summary>
        /// Recommended sample rates to record
        /// </summary>
        public static int[] RecommendedSampleRates { get; } = new[]
        {
            8000,
            11025,
            22050,
            32000,
            44100,
            48000
        };

        private List<RecorderWaveProvider> Recorders { get; }
        private List<(TemporaryWaveStream, Task)> TemporaryBuffers { get; }

        /// <summary>
        /// Saves an array of audio sources to an output stream
        /// </summary>
        /// <param name="records">List of audio sources</param>
        /// <param name="output">Output stream to save to</param>
        /// <param name="bitRate">Bitrate used to encode mp3</param>
        public void Save(IEnumerable<IWaveProvider> records, Stream output, int bitRate)
        {
            var filename = Path.GetTempFileName();
            MediaFoundationEncoder.EncodeToMp3(new MixingWaveProvider32(records), filename, bitRate);
            using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                fileStream.CopyTo(output);
            File.Delete(filename);
        }

        /// <summary>
        /// Start recording the given devices
        /// </summary>
        /// <param name="devices">List of devices to record</param>
        /// <param name="format">Wave format to convert to</param>
        /// <param name="bufferDuration">Duration of the temporary buffer</param>
        public void StartRecord(IEnumerable<MMDevice> devices, WaveFormat format, TimeSpan bufferDuration)
        {
            if (Recorders.Any())
                throw new InvalidOperationException("ALready recording");
            Recorders.Clear();
            TemporaryBuffers.Clear();
            foreach (var device in devices)
            {
                var recorder = new RecorderWaveProvider(device);
                var sampler = new MediaFoundationResampler(recorder, format);
                var buffer = new TemporaryWaveStream(sampler, bufferDuration);
                Recorders.Add(recorder);
                TemporaryBuffers.Add((buffer, buffer.ListenAsync()));
            }
            foreach (var recorder in Recorders)
            {
                recorder.StartRecording();
            }
        }

        /// <summary>
        /// Stops any recording
        /// </summary>
        /// <returns>The list of recorded audio streams</returns>
        public async Task<WaveStream[]> StopRecord()
        {
            foreach (var recorder in Recorders)
            {
                recorder.StopRecording();
            }
            foreach (var buffer in TemporaryBuffers)
            {
                await buffer.Item2;
                buffer.Item1.Seek(0, System.IO.SeekOrigin.Begin);
            }
            var result = TemporaryBuffers.Select(buffer => buffer.Item1).ToArray();
            Recorders.Clear();
            TemporaryBuffers.Clear();
            return result;
        }
    }
}