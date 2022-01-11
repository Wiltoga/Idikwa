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
            MasterCopy = new RawSourceWaveStream(MasterMemory, mixer.WaveFormat).ToSampleProvider();
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
        }

        public ISampleProvider MasterCopy { get; }

        [Reactive]
        public int MasterVolume { get; set; }

        public IEnumerable<RecordViewModel> Records { get; }
        public float Scale { get; }

        [Reactive]
        public TimeSpan Skip { get; set; }

        [Reactive]
        public TimeSpan Take { get; set; }

        private Stream MasterMemory { get; }
    }
}