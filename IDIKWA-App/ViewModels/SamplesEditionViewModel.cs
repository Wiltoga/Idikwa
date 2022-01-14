using Avalonia.Controls;
using DynamicData;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public SamplesEditionViewModel(IEnumerable<RecordViewModel> records, SettingsViewModel settings)
        {
            Settings = settings;
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
            settings.WhenAnyValue(o => o.MasterVolume).Subscribe(value =>
            {
                foreach (var record in Records)
                {
                    record.MasterVolume = value;
                }
            });
        }

        public float[] AverageMaster { get; }

        [Reactive]
        public TimeSpan CurrentPosition { get; set; }

        [Reactive]
        public TimeSpan Duration { get; set; }

        [Reactive]
        public TimeSpan LeftBound { get; set; }

        public ISampleProvider MasterCopy { get; }

        [Reactive]
        public bool Playing { get; set; }

        public ICommand PlayPause { get; }
        public IEnumerable<RecordViewModel> Records { get; }

        [Reactive]
        public TimeSpan RightBound { get; set; }

        public float Scale { get; }

        public SettingsViewModel Settings { get; }

        public ICommand StartStop { get; }

        [Reactive]
        public Window? Window { get; set; }

        private Stream MasterMemory { get; }

        private TimeSpan VirtualLeftBound => Settings.AdvancedEdition ? LeftBound : TimeSpan.Zero;

        private TimeSpan VirtualRightBound => Settings.AdvancedEdition ? RightBound : Duration;

        public void Cancel()
        {
            StopPlayers();
        }

        public void Pause()
        {
            StopPlayers();
            Playing = false;
        }

        public void Play()
        {
            if (CurrentPosition < VirtualLeftBound)
                CurrentPosition = VirtualLeftBound;
            else if (CurrentPosition > VirtualRightBound)
                CurrentPosition = VirtualLeftBound;
            if (SetPlayers())
            {
                if (StartPlayers())
                    Playing = true;
            }
        }

        public void Save()
        {
            Stop();
            try
            {
                var filename = $"{DateTime.Now:yyyy-MM-dd HH.mm.ss}.mp3";
                Directory.CreateDirectory(Settings.OutputPath);
                var filepath = Path.Combine(Settings.OutputPath, filename);
                using (var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                {
                    Save(stream);
                }
                new Process
                {
                    StartInfo = new ProcessStartInfo("explorer.exe", $"/select,\"{filepath}\"")
                    {
                        UseShellExecute = true
                    }
                }.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task SaveAs()
        {
            var dialog = new SaveFileDialog
            {
                InitialFileName = $"{DateTime.Now:yyyy-MM-dd HH.mm.ss}.mp3",
                Directory = Settings.OutputPath,
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter
                    {
                         Name = new Resx("mp3Files").ProvideValue(null)?.ToString(),
                         Extensions = new List<string>
                         {
                             "mp3"
                         }
                    },
                    new FileDialogFilter
                    {
                         Name = new Resx("allFiles").ProvideValue(null)?.ToString(),
                         Extensions = new List<string>
                         {
                             "*"
                         }
                    }
                }
            };
            if (Window is not null)
                if (await dialog.ShowAsync(Window) is string str)
                {
                    Pause();
                    try
                    {
                        using (var stream = new FileStream(str, FileMode.Create, FileAccess.Write))
                        {
                            Save(stream);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    Play();
                }
        }

        public void Start()
        {
            CurrentPosition = VirtualLeftBound;
            if (SetPlayers())
            {
                if (StartPlayers())
                    Playing = true;
            }
        }

        public void Stop()
        {
            StopPlayers();
            CurrentPosition = VirtualLeftBound;
            Playing = false;
        }

        private void Save(Stream output)
        {
            App.Factory.Save(Records.Select(rec => rec.GetFinalProvider(VirtualLeftBound, VirtualRightBound - VirtualLeftBound)), output, Settings.BitRate);
        }

        private bool SetPlayers()
        {
            foreach (var record in Records)
            {
                if (!record.InitPlayer(CurrentPosition, VirtualRightBound - CurrentPosition))
                    return false;
            }
            return true;
        }

        private bool StartPlayers()
        {
            if (VirtualRightBound == VirtualLeftBound)
                return false;
            PositionUpdaterTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                while (!PositionUpdaterTokenSource.IsCancellationRequested)
                {
                    var record = Records.First();
                    CurrentPosition = record.CurrentTime;
                    if (record.Player.PlaybackState == PlaybackState.Stopped)
                        Stop();
                    Thread.Sleep(10);
                }
            }, PositionUpdaterTokenSource.Token);
            foreach (var record in Records)
            {
                record.Player.Play();
            }
            return true;
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