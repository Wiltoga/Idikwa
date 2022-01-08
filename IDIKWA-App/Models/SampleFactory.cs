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
    public class SampleFactory
    {
        public SampleFactory()
        {
            Recorders = new List<RecorderWaveProvider>();
            TemporaryBuffers = new List<(TemporaryWaveStream, Task)>();
        }

        public int[] RecommendedBitRates { get; } = new[]
        {
            64000,
            96000,
            128000,
            192000,
            256000,
            320000
        };

        public int[] RecommendedSampleRates { get; } = new[]
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

        public void Save(IEnumerable<IWaveProvider> records, Stream output, int bitRate)
        {
            var filename = Path.GetTempFileName();
            MediaFoundationEncoder.EncodeToMp3(new MixingWaveProvider32(records), filename, bitRate);
            using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                fileStream.CopyTo(output);
            File.Delete(filename);
        }

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