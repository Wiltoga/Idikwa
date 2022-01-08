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
            AvailableDevices = new ObservableCollection<MMDevice>();
            DeviceEnumerator = new MMDeviceEnumerator();
            try
            {
                foreach (var item in DeviceEnumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active))
                {
                    AvailableDevices.Add(item);
                }
            }
            catch (Exception)
            {
            }
        }

        ~SettingsViewModel()
        {
            DeviceEnumerator.Dispose();
        }

        public static SettingsViewModel Default
        {
            get
            {
                var result = new SettingsViewModel
                {
                    Duration = TimeSpan.FromSeconds(90),
                    BitRate = 96000,
                    SampleRate = 44100
                };
                try
                {
                    result.RecordingDevices.Add(result.DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications));
                }
                catch (Exception)
                {
                }
                try
                {
                    result.RecordingDevices.Add(result.DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications));
                }
                catch (Exception)
                {
                }
                return result;
            }
        }

        [Reactive]
        public ObservableCollection<MMDevice> AvailableDevices { get; set; }

        [Reactive]
        public int BitRate { get; set; }

        [Reactive]
        public TimeSpan Duration { get; set; }

        [Reactive]
        public ObservableCollection<MMDevice> RecordingDevices { get; set; }

        [Reactive]
        public int SampleRate { get; set; }

        private MMDeviceEnumerator DeviceEnumerator { get; }
    }
}