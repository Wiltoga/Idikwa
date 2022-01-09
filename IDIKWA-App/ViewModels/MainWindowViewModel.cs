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

namespace IDIKWA_App
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            Factory = new SampleFactory();
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
            EditSettings = CommandHandler.Create(async () => await RunEditSettingsAsync());
            Recording = false;
        }

        [Reactive]
        public ICommand EditSettings { get; private set; }

        [Reactive]
        public ICommand Record { get; private set; }

        [Reactive]
        public bool Recording { get; set; }

        public SettingsViewModel Settings { get; }

        [Reactive]
        public Window? Window { get; set; }

        private SampleFactory Factory { get; }

        public async Task Exit()
        {
            await Factory.StopRecord();
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
                    Factory.StartRecord(Settings.RecordingDevices.Select(device => device.Device), WaveFormat.CreateIeeeFloatWaveFormat(Settings.SampleRate, Settings.Mono ? 1 : 2), Settings.Duration);
                    Recording = true;
                }
                catch (Exception)
                {
                }
        }

        public async Task StopRecord()
        {
            try
            {
                var streams = await Factory.StopRecord();
                Recording = false;
            }
            catch (Exception)
            {
            }
        }
    }
}