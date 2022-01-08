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

namespace IDIKWA_App
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            Factory = new SampleFactory();
            Window = null;
            Settings = SettingsViewModel.Default;
            Record = CommandHandler.Create(async () =>
            {
                if (Recording)
                    await StopRecord();
                else
                    RunRecord();
            });
            Recording = false;
        }

        [Reactive]
        public ICommand Record { get; private set; }

        [Reactive]
        public bool Recording { get; set; }

        [Reactive]
        public SettingsViewModel Settings { get; set; }

        [Reactive]
        public Window? Window { get; set; }

        private SampleFactory Factory { get; }

        public void RunRecord()
        {
            Factory.StartRecord(Settings.RecordingDevices, WaveFormat.CreateIeeeFloatWaveFormat(Settings.SampleRate, Settings.Mono ? 1 : 2), Settings.Duration);
            Recording = true;
        }

        public async Task StopRecord()
        {
            Recording = false;
            var streams = await Factory.StopRecord();
        }
    }
}