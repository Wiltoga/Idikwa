using Avalonia.Controls;
using NAudio.CoreAudioApi;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NAudio.Wave;
using System.Collections.Specialized;
using DynamicData;
using System.IO;
using System.Diagnostics;

namespace IDIKWA_App
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            Window = null;
            Settings = App.InitialSettings is not null
                ? new SettingsViewModel(App.InitialSettings)
                : SettingsViewModel.Default;
            Record = CommandHandler.Create(async () =>
            {
                if (Recording)
                    await StopRecord();
                else
                    RunRecord();
            });
            CancelRecording = CommandHandler.Create(async () => await RunCancelRecording());
            EditSettings = CommandHandler.Create(async () => await RunEditSettingsAsync());
            Recording = false;
        }

        [Reactive]
        public ICommand CancelRecording { get; private set; }

        [Reactive]
        public ICommand EditSettings { get; private set; }

        [Reactive]
        public ICommand Record { get; private set; }

        [Reactive]
        public bool Recording { get; set; }

        public SettingsViewModel Settings { get; }

        [Reactive]
        public Window? Window { get; set; }

        public bool WithLogin => false;

        public async Task Exit()
        {
            await App.Factory.StopRecord();
        }

        public async Task RunCancelRecording()
        {
            try
            {
                await App.Factory.StopRecord();
                Recording = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task RunEditSettingsAsync()
        {
            var dialog = new SettingsWindow()
            {
                DataContext = Settings
            };

            await dialog.ShowDialog(Window);

            SettingsManager.Save(Settings.Model);
        }

        public void RunRecord()
        {
            if (Settings.RecordingDevices.Any() && Settings.Duration > TimeSpan.Zero)
                try
                {
                    App.Factory.StartRecord(Settings.RecordingDevices.Select(device => device.Device), WaveFormat.CreateIeeeFloatWaveFormat(Settings.SampleRate, Settings.Mono ? 1 : 2), Settings.Duration);
                    Recording = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
        }

        public async Task StopRecord()
        {
            try
            {
                var streams = await App.Factory.StopRecord();
                if (streams.Any())
                {
                    var computationStream = streams.First().Item2;
                    if (computationStream.Length / computationStream.WaveFormat.AverageBytesPerSecond < 1)
                        return;
                    var dialog = new SamplesEditionWindow()
                    {
                        DataContext = new SamplesEditionViewModel(
                            streams
                                .Select(stream => new RecordViewModel(Settings.AllDevices.First(device => device.Device.ID == stream.Item1.ID), stream.Item2)),
                            Settings)
                    };
                    dialog.Closed += (sender, e) => SettingsManager.Save(Settings.Model);
                    dialog.Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            RunRecord();
        }
    }
}