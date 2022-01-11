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
        public RecordViewModel(DeviceViewModel origin, WaveStream source)
        {
            Origin = origin;
            Source = source;
            SourceAsSample = Source.ToSampleProvider();
            var volumeManager = new VolumeSampleProvider(SourceAsSample);
            volumeManager.Volume = Origin.Volume;
            Origin.WhenAnyValue(o => o.Volume).BindTo(volumeManager, manager => manager.Volume);
            Output = volumeManager;
            var samples = new float[50000];
            int samplesRead;
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
        }

        public float HighestSample { get; }

        public DeviceViewModel Origin { get; }

        public ISampleProvider Output { get; }

        public WaveStream Source { get; }

        public ISampleProvider SourceAsSample { get; }
    }
}