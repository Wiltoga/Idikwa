﻿using Avalonia.Controls;
using Avalonia.Xaml.Interactions.Core;
using DynamicData;
using NAudio.CoreAudioApi;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDIKWA_App
{
    public class SettingsViewModel : ReactiveObject
    {
        private readonly ReadOnlyObservableCollection<DeviceViewModel> allDevices;
        private readonly ObservableAsPropertyHelper<bool> canRecord;
        private readonly ReadOnlyObservableCollection<DeviceViewModel> captureDevices;
        private readonly ReadOnlyObservableCollection<DeviceViewModel> recordingDevices;
        private readonly ReadOnlyObservableCollection<DeviceViewModel> renderDevices;

        private TimeSpan duration;

        public SettingsViewModel()
        {
            Cultures = new ObservableCollection<CultureInfo>
            {
                new CultureInfo("en"),
                new CultureInfo("fr")
            };
            Culture = Cultures.First();
            Devices = new SourceCache<DeviceViewModel, string>(device => device.Device.ID);
            BrowseOutputDir = CommandHandler.Create(async () => await RunBrowseOutputDir());
            DeviceEnumerator = new MMDeviceEnumerator();
            DevicesConnect = Devices.Connect();
            DevicesConnect
                .AutoRefresh(device => device.Recording)
                .Filter(device => device.Recording)
                .Bind(out recordingDevices)
                .Subscribe();
            canRecord = DevicesConnect
                .AutoRefresh(device => device.Recording)
                .TrueForAny(device => device.WhenAnyValue(d => d.Recording), recording => recording).ToProperty(this, nameof(CanRecord));
            DevicesConnect
                .Filter(device => device.Device.DataFlow == DataFlow.Render)
                .Bind(out renderDevices)
                .Subscribe();
            DevicesConnect
                .Filter(device => device.Device.DataFlow == DataFlow.Capture)
                .Bind(out captureDevices)
                .Subscribe();
            DevicesConnect
                .Bind(out allDevices)
                .Subscribe();
            try
            {
                Devices.Edit(sourceUpdater =>
                    sourceUpdater.AddOrUpdate(DeviceEnumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active)
                    .Select(device => new DeviceViewModel(device, false, 100))));
            }
            catch (Exception)
            {
            }
            BitRate = 0;
            OutputPath = "";
            Duration = TimeSpan.Zero;
            SampleRate = 0;
        }

        public SettingsViewModel(Settings model) : this()
        {
            BitRate = model.BitRate;
            OutputPath = model.OutputPath;
            Duration = model.Duration;
            SampleRate = model.SampleRate;
            MasterVolume = model.MasterVolume;
            AdvancedEdition = model.Advanced;
            Mono = model.Mono;
            EulaAccepted = model.Eula;
            Culture = Cultures.FirstOrDefault(culture => culture.Name == model.Culture) ?? Culture;
            foreach (var device in model.RecordingDevices)
            {
                var availableDevice = Devices.Items.FirstOrDefault(d => d.Device.ID == device);
                if (availableDevice is not null)
                {
                    availableDevice.Recording = true;
                }
            }
            foreach (var device in model.Volumes)
            {
                var availableDevice = Devices.Items.FirstOrDefault(d => d.Device.ID == device.Key);
                if (availableDevice is not null)
                {
                    availableDevice.Volume = device.Value;
                }
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
                    AdvancedEdition = false,
                    BitRate = 64000,
                    SampleRate = 44100,
                    Mono = true,
                    MasterVolume = 100,
                    OutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Idikwa")
                };
                var currentCulture = CultureInfo.CurrentCulture;
                result.Culture = result.Cultures.FirstOrDefault(c => c.Name.ToLower() == currentCulture.Name.Substring(0, 2).ToLower()) ?? result.Culture;
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
        public bool AdvancedEdition { get; set; }

        public ReadOnlyObservableCollection<DeviceViewModel> AllDevices => allDevices;

        [Reactive]
        public int BitRate { get; set; }

        [Reactive]
        public ICommand BrowseOutputDir { get; private set; }

        public bool CanRecord => canRecord.Value;

        public ReadOnlyObservableCollection<DeviceViewModel> CaptureDevices => captureDevices;

        [Reactive]
        public CultureInfo Culture { get; set; }

        public ObservableCollection<CultureInfo> Cultures { get; }

        public TimeSpan Duration
        {
            get => duration;
            set
            {
                duration = value;
                this.RaisePropertyChanged(nameof(Duration));
                this.RaisePropertyChanged(nameof(DurationMinutes));
                this.RaisePropertyChanged(nameof(DurationSeconds));
            }
        }

        public int DurationMinutes
        {
            get => Duration.Minutes;
            set
            {
                Duration = new TimeSpan(0, value, Duration.Seconds);
            }
        }

        public int DurationSeconds
        {
            get => Duration.Seconds;
            set
            {
                Duration = new TimeSpan(0, Duration.Minutes, value);
            }
        }

        [Reactive]
        public bool EulaAccepted { get; set; }

        [Reactive]
        public int MasterVolume { get; set; }

        public Settings Model => new Settings(EulaAccepted, BitRate, MasterVolume, Devices.Items.Where(device => device.Volume < 100).ToDictionary(device => device.ID, device => device.Volume), Duration, Mono, OutputPath, Devices.Items.Where(device => device.Recording).Select(device => device.ID).ToList(), SampleRate, Culture.Name, AdvancedEdition);

        [Reactive]
        public bool Mono { get; set; }

        [Reactive]
        public string OutputPath { get; set; }

        public ReadOnlyObservableCollection<DeviceViewModel> RecordingDevices => recordingDevices;

        public ReadOnlyObservableCollection<DeviceViewModel> RenderDevices => renderDevices;

        [Reactive]
        public int SampleRate { get; set; }

        [Reactive]
        public Window? Window { get; set; }

        private MMDeviceEnumerator DeviceEnumerator { get; }

        private SourceCache<DeviceViewModel, string> Devices { get; }

        private IObservable<IChangeSet<DeviceViewModel, string>> DevicesConnect { get; }

        public void DurationMaxChanged()
        {
            if (DurationMinutes == 5)
                DurationSeconds = 0;
        }

        public void DurationMinChanged()
        {
            if (DurationMinutes == 0 && DurationSeconds == 0)
                DurationSeconds = 1;
        }

        public async Task RunBrowseOutputDir()
        {
            if (Window is not null)
            {
                var dialog = new OpenFolderDialog();
                dialog.Directory = OutputPath;
                dialog.Title = new Resx("outputDir").ProvideValue(null)?.ToString();
                if (await dialog.ShowAsync(Window) is string dir)
                {
                    OutputPath = dir;
                }
            }
        }
    }
}