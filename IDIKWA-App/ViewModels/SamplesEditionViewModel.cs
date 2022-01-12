using DynamicData;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class SamplesEditionViewModel : ReactiveObject
    {
        private ReadOnlyObservableCollection<object> allRecords;

        public SamplesEditionViewModel(IEnumerable<RecordViewModel> records)
        {
            MasterVolume = 100;
            Records = new ObservableCollection<RecordViewModel>(records);
            MasterMemory = new MemoryStream();
            var mixer = new MixingWaveProvider32(records.Select(record => record.Source));
            int bytesRead;
            var bytes = new byte[50000];
            while ((bytesRead = mixer.Read(bytes, 0, bytes.Length)) > 0)
                MasterMemory.Write(bytes, 0, bytesRead);
            foreach (var source in Records)
            {
                source.Source.Seek(0, SeekOrigin.Begin);
            }
            MasterMemory.Seek(0, SeekOrigin.Begin);
            var masterSource = new RawSourceWaveStream(MasterMemory, mixer.WaveFormat);
            MasterCopy = masterSource.ToSampleProvider();
            AverageMaster = RecordViewModel.GetAverage(MasterCopy);
            MasterMemory.Seek(0, SeekOrigin.Begin);
            int samplesRead;
            var samples = new float[50000];
            var highestSample = 0f;
            while ((samplesRead = MasterCopy.Read(samples, 0, samples.Length)) > 0)
                for (int i = 0; i < samplesRead; ++i)
                {
                    var abs = Math.Abs(samples[i]);
                    if (abs > highestSample)
                        highestSample = abs;
                }
            Scale = 1.5f / highestSample;
            MasterMemory.Seek(0, SeekOrigin.Begin);
            Duration = masterSource.TotalTime;
            LeftBound = TimeSpan.Zero;
            RightBound = Duration;
            var allRecords = new SourceCache<object, int>(o => o.GetHashCode());
            allRecords.Connect()
                .Filter(this.WhenAnyValue(o => o.Filter))
                /*.Sort(Comparer<object>.Create((left, right) =>
                {
                    if (left is SamplesEditionViewModel)
                        return 1;
                    else if (right is SamplesEditionViewModel)
                        return -1;
                    else
                        return Records.IndexOf((RecordViewModel)left).CompareTo((RecordViewModel)right);
                }))*/
                .Bind(out this.allRecords)
                .Subscribe();
            this.WhenAnyValue(o => o.Advanced).Subscribe(adv =>
            {
                if (adv)
                    Filter = item => true;
                else
                    Filter = item => item is SamplesEditionViewModel;
            });
            Filter = item => item is SamplesEditionViewModel;
            allRecords.Edit(updater => updater.AddOrUpdate(Records));
            allRecords.Edit(updater => updater.AddOrUpdate(this));
        }

        [Reactive]
        public bool Advanced { get; set; }

        public ReadOnlyObservableCollection<object> AllRecords => allRecords;

        public float[] AverageMaster { get; }

        [Reactive]
        public TimeSpan Duration { get; set; }

        [Reactive]
        public TimeSpan LeftBound { get; set; }

        public ISampleProvider MasterCopy { get; }

        [Reactive]
        public int MasterVolume { get; set; }

        public IEnumerable<RecordViewModel> Records { get; }

        [Reactive]
        public TimeSpan RightBound { get; set; }

        public float Scale { get; }

        [Reactive]
        private Func<object, bool> Filter { get; set; }

        private Stream MasterMemory { get; }
    }
}