using DynamicData;
using NAudio.CoreAudioApi;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class SettingsViewModel : ReactiveObject
    {
        public ReadOnlyObservableCollection<DeviceViewModel> captureDevices;
        public ReadOnlyObservableCollection<DeviceViewModel> renderDevices;
        private readonly ReadOnlyObservableCollection<DeviceViewModel> recordingDevices;

        public SettingsViewModel()
        {
            Devices = new SourceCache<DeviceViewModel, string>(device => device.Device.ID);
            DeviceEnumerator = new MMDeviceEnumerator();
            DevicesVolume = new Dictionary<string, float>();
            var shared = Devices.Connect().Publish();
            shared
                .AutoRefresh(device => device.Recording)
                .Filter(device => device.Recording)
                .Bind(out recordingDevices)
                .Subscribe();
            shared
                .Filter(device => device.Device.DataFlow == DataFlow.Render)
                .Bind(out renderDevices)
                .Subscribe();
            shared
                .Filter(device => device.Device.DataFlow == DataFlow.Capture)
                .Bind(out captureDevices)
                .Subscribe();
            shared.Connect();
            try
            {
                Devices.Edit(sourceUpdater =>
                    sourceUpdater.AddOrUpdate(DeviceEnumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active)
                    .Select(device => new DeviceViewModel(device, false))));
            }
            catch (Exception)
            {
            }
            BitRate = 0;
            OutputPath = "";
            Duration = TimeSpan.Zero;
            Mono = false;
            SampleRate = 0;
        }

        public SettingsViewModel(Settings model) : this()
        {
            BitRate = model.BitRate;
            OutputPath = model.OutputPath;
            Duration = model.Duration;
            Mono = model.Mono;
            SampleRate = model.SampleRate;
            foreach (var device in model.RecordingDevices)
            {
                var availableDevice = Devices.Items.FirstOrDefault(d => d.Device.ID == device);
                if (availableDevice is not null)
                {
                    availableDevice.Recording = true;
                }
            }
            DevicesVolume = model.Volumes;
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
                    SampleRate = 44100,
                    Mono = false,
                    OutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "Idikwa")
                };
                try
                {
                    var defaultDevice = result.Devices.Items.First(d => result.DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications).ID == d.Device.ID);
                    defaultDevice.Recording = true;
                }
                catch (Exception)

                {
                }

                try
                {
                    var defaultDevice = result.Devices.Items.First(d => result.DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications).ID == d.Device.ID);
                    defaultDevice.Recording = true;
                }
                catch (Exception)
                {
                }
                return result;
            }
        }

        [Reactive]
        public int BitRate { get; set; }

        public ReadOnlyObservableCollection<DeviceViewModel> CaptureDevices => captureDevices;

        public SourceCache<DeviceViewModel, string> Devices { get; }

        [Reactive]
        public TimeSpan Duration { get; set; }

        public Settings Model => new Settings(BitRate, DevicesVolume, Duration, Mono, OutputPath, Devices.Items.Where(device => device.Recording).Select(device => device.Device.ID).ToList(), SampleRate);

        [Reactive]
        public bool Mono { get; set; }

        [Reactive]
        public string OutputPath { get; set; }

        public ReadOnlyObservableCollection<DeviceViewModel> RecordingDevices => recordingDevices;
        public ReadOnlyObservableCollection<DeviceViewModel> RenderDevices => renderDevices;

        [Reactive]
        public int SampleRate { get; set; }

        private MMDeviceEnumerator DeviceEnumerator { get; }
        private Dictionary<string, float> DevicesVolume { get; }

        public float GetDeviceVolume(MMDevice device)
        {
            if (DevicesVolume.ContainsKey(device.ID))
                return DevicesVolume[device.ID];
            return 1;
        }

        public void SetDeviceVolume(MMDevice device, float volume)
        {
            DevicesVolume[device.ID] = volume;
        }
    }
}