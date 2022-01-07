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
    public class RecorderWaveProvider : IWaveProvider
    {
        private WasapiCapture capture;

        private PipeStream pipe;

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

        public void StartRecording()
        {
            capture.StartRecording();
        }

        public void StopRecording()
        {
            capture.StopRecording();
            pipe.Close();
        }
    }
}