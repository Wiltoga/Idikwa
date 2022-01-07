using NAudio.CoreAudioApi;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class SettingsViewModel : ReactiveObject
    {
        public SettingsViewModel()
        {
            RecordingDevices = new ObservableCollection<MMDevice>();
            DeviceEnumerator = new MMDeviceEnumerator();
            try
            {
                RecordingDevices.Add(DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications));
            }
            catch (Exception)
            {
            }
            try
            {
                RecordingDevices.Add(DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications));
            }
            catch (Exception)
            {
            }
            Duration = TimeSpan.FromSeconds(90);
        }

        ~SettingsViewModel()
        {
            DeviceEnumerator.Dispose();
        }

        [Reactive]
        public TimeSpan Duration { get; set; }

        [Reactive]
        public ObservableCollection<MMDevice> RecordingDevices { get; set; }

        private MMDeviceEnumerator DeviceEnumerator { get; }
    }
}