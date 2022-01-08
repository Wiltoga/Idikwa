using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    /// <summary>
    /// Wrapper to WasapiCapture to act as a wave provider
    /// </summary>
    public class RecorderWaveProvider : IWaveProvider
    {
        private WasapiCapture capture;

        private PipeStream pipe;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="device">Device to record</param>
        public RecorderWaveProvider(MMDevice device)
        {
            if (device.DataFlow == DataFlow.Render)
                capture = new WasapiLoopbackCapture(device);
            else
                capture = new WasapiCapture(device);
            pipe = new PipeStream();
            capture.DataAvailable += (sender, e) =>
            {
                pipe.Write(e.Buffer, 0, e.BytesRecorded);
            };
            capture.RecordingStopped += (sender, e) =>
            {
                capture.Dispose();
            };
        }

        public WaveFormat WaveFormat => capture.WaveFormat;

        public int Read(byte[] buffer, int offset, int count)
        {
            return pipe.Read(buffer, offset, count);
        }

        /// <summary>
        /// Start recording the device
        /// </summary>
        public void StartRecording()
        {
            capture.StartRecording();
        }

        /// <summary>
        /// Stop recording the device and close the internal stream.
        /// </summary>
        public void StopRecording()
        {
            capture.StopRecording();
            pipe.Close();
        }
    }
}