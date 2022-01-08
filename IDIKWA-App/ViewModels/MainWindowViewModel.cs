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

namespace IDIKWA_App
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            Settings = SettingsViewModel.Default;
            Record = CommandHandler.Create(() =>
            {
                if (Recording)
                    StopRecord();
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

        public void RunRecord()
        {
            Recording = true;
        }

        public void StopRecord()
        {
            Recording = false;
        }
    }
}