using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class RecordViewModel : ReactiveObject
    {
        private static readonly int TotalSampleReduction = 2000;

        public RecordViewModel(DeviceViewModel origin, WaveStream source)
        {
            Origin = origin;
            Source = source;
            Player = new WasapiOut();
            SourceAsSample = Source.ToSampleProvider();
            var deviceVolume = new VolumeSampleProvider(SourceAsSample);
            Origin.WhenAnyValue(o => o.Volume).Subscribe(value => deviceVolume.Volume = value / 100f);
            VolumeOutput = new VolumeSampleProvider(deviceVolume);
            this.WhenAnyValue(o => o.MasterVolume).Subscribe(value => VolumeOutput.Volume = value / 100f);
            Offset = new OffsetSampleProvider(VolumeOutput);
            var samples = new float[50000];
            int samplesRead;
            AverageSamples = new float[TotalSampleReduction];
            var tmpList = new List<float>(100000 * 60);
            HighestSample = 0;
            Source.Seek(0, System.IO.SeekOrigin.Begin);
            while ((samplesRead = SourceAsSample.Read(samples, 0, samples.Length)) > 0)
                for (int i = 0; i < samplesRead; ++i)
                {
                    var abs = Math.Abs(samples[i]);
                    if (abs > HighestSample)
                        HighestSample = abs;
                }
            Source.Seek(0, System.IO.SeekOrigin.Begin);
            AverageSamples = GetAverage(SourceAsSample);
            Source.Seek(0, System.IO.SeekOrigin.Begin);
        }

        public float[] AverageSamples { get; }

        public TimeSpan CurrentTime => Source.CurrentTime;

        public float HighestSample { get; }

        [Reactive]
        public int MasterVolume { get; set; }

        public DeviceViewModel Origin { get; }

        public WasapiOut Player { get; set; }
        public WaveStream Source { get; }
        public ISampleProvider SourceAsSample { get; }
        public VolumeSampleProvider VolumeOutput { get; private set; }
        private OffsetSampleProvider Offset { get; set; }

        public static float[] GetAverage(ISampleProvider samplesProvider)
        {
            var result = new float[TotalSampleReduction];
            var tmpList = new List<float>(100000 * 60);
            var samplesBuffer = new float[10000 * samplesProvider.WaveFormat.Channels];
            int samplesRead;
            while ((samplesRead = samplesProvider.Read(samplesBuffer, 0, samplesBuffer.Length)) > 0)
                for (int i = 0; i < samplesRead; ++i)
                    tmpList.Add(Math.Abs(samplesBuffer[i]));
            for (int i = 0; i < result.Length; ++i)
            {
                var samples = tmpList.Count / result.Length;
                var offset = samples * i;
                samples = Math.Min(samples, tmpList.Count - offset);
                result[i] = tmpList.GetRange(offset, samples).Average();
            }
            return result;
        }

        public IWaveProvider GetFinalProvider(TimeSpan offset, TimeSpan duration)
        {
            Source.Seek(0, System.IO.SeekOrigin.Begin);
            return new OffsetSampleProvider(VolumeOutput)
            {
                SkipOver = offset,
                Take = duration
            }.ToWaveProvider();
        }

        public bool InitPlayer(TimeSpan offset, TimeSpan duration)
        {
            try
            {
                Source.Seek(0, System.IO.SeekOrigin.Begin);
                Offset = new OffsetSampleProvider(VolumeOutput) { SkipOver = offset, Take = duration };
                Player.Dispose();
                Player = new WasapiOut();
                Player.Init(Offset);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}