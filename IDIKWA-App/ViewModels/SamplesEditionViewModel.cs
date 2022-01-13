using Avalonia.Controls;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDIKWA_App
{
    public class SamplesEditionViewModel : ReactiveObject
    {
        private CancellationTokenSource? PositionUpdaterTokenSource;

        public SamplesEditionViewModel(IEnumerable<RecordViewModel> records)
        {
            TimeSpan lowest = records.First().Source.TotalTime;
            foreach (var item in records)
            {
                if (lowest > item.Source.TotalTime)
                    lowest = item.Source.TotalTime;
            }
            foreach (var item in records)
            {
                item.Source.SetLength((long)(item.Source.WaveFormat.AverageBytesPerSecond * lowest.TotalSeconds));
            }
            PlayPause = CommandHandler.Create(() =>
        {
            if (Playing)
                Pause();
            else
                Play();
        });
            StartStop = CommandHandler.Create(() =>
            {
                if (Playing)
                    Stop();
                else
                    Start();
            });
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
            masterSource.CurrentTime = TimeSpan.Zero;
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
            masterSource.CurrentTime = TimeSpan.Zero;
            Duration = masterSource.TotalTime;
            LeftBound = TimeSpan.Zero;
            RightBound = Duration;
            CurrentPosition = TimeSpan.Zero;
            this.WhenAnyValue(o => o.MasterVolume).Subscribe(value =>
            {
                foreach (var record in Records)
                {
                    record.Player.Volume = value / 100f;
                }
            });
        }

        [Reactive]
        public bool Advanced { get; set; }

        public float[] AverageMaster { get; }

        [Reactive]
        public TimeSpan CurrentPosition { get; set; }

        [Reactive]
        public TimeSpan Duration { get; set; }

        [Reactive]
        public TimeSpan LeftBound { get; set; }

        public ISampleProvider MasterCopy { get; }

        [Reactive]
        public int MasterVolume { get; set; }

        [Reactive]
        public bool Playing { get; set; }

        public IEnumerable<RecordViewModel> Records { get; }

        [Reactive]
        public TimeSpan RightBound { get; set; }

        public float Scale { get; }

        private Stream MasterMemory { get; }

        private ICommand PlayPause { get; }

        private ICommand StartStop { get; }

        public void Pause()
        {
            StopPlayers();
            Playing = false;
        }

        public void Play()
        {
            if (CurrentPosition < LeftBound)
                CurrentPosition = LeftBound;
            else if (CurrentPosition > RightBound)
                CurrentPosition = LeftBound;
            SetPlayers(CurrentPosition);
            StartPlayers();
            Playing = true;
        }

        public void Start()
        {
            CurrentPosition = LeftBound;
            SetPlayers(CurrentPosition);
            StartPlayers();
            Playing = true;
        }

        public void Stop()
        {
            CurrentPosition = LeftBound;
            StopPlayers();
            Playing = false;
        }

        private void SetPlayers(TimeSpan time)
        {
            foreach (var record in Records)
            {
                record.InitPlayer(time);
                record.Player.Volume = MasterVolume / 100f;
            }
        }

        private void StartPlayers()
        {
            PositionUpdaterTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                while (!PositionUpdaterTokenSource.IsCancellationRequested)
                {
                    var record = Records.First();
                    CurrentPosition = record.CurrentTime;
                    if (CurrentPosition > RightBound)
                        Stop();
                    else
                    if (record.Player.PlaybackState == PlaybackState.Stopped)
                        Stop();
                    Thread.Sleep(10);
                }
            }, PositionUpdaterTokenSource.Token);
            foreach (var record in Records)
            {
                record.Player.Play();
            }
        }

        private void StopPlayers()
        {
            PositionUpdaterTokenSource?.Cancel();
            foreach (var record in Records)
            {
                if (record.Player.PlaybackState != PlaybackState.Stopped)
                    record.Player.Stop();
            }
        }
    }
}