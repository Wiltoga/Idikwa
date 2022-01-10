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
            Skip = TimeSpan.Zero;
            Take = Source.TotalTime;
            var offsetManager = new OffsetSampleProvider(SourceAsSample);
            offsetManager.Take = Take;
            offsetManager.SkipOver = Skip;
            this.WhenAnyValue(o => o.Skip).Subscribe(skip => offsetManager.SkipOver = skip);
            this.WhenAnyValue(o => o.Take).Subscribe(take => offsetManager.Take = take);
            var volumeManager = new VolumeSampleProvider(offsetManager);
            volumeManager.Volume = Origin.Volume;
            Origin.WhenAnyValue(o => o.Volume).Subscribe(volume => volumeManager.Volume = volume);
            Output = volumeManager;
        }

        public DeviceViewModel Origin { get; }

        public ISampleProvider Output { get; }

        [Reactive]
        public TimeSpan Skip { get; set; }

        public WaveStream Source { get; }

        public ISampleProvider SourceAsSample { get; }

        [Reactive]
        public TimeSpan Take { get; set; }
    }
}