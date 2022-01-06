﻿using NAudio.CoreAudioApi;
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
            RecordingDevices.Add(DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications));
            RecordingDevices.Add(DeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications));
            Duration = TimeSpan.FromSeconds(90);
        }

        [Reactive]
        public TimeSpan Duration { get; set; }

        [Reactive]
        public ObservableCollection<MMDevice> RecordingDevices { get; set; }

        private MMDeviceEnumerator DeviceEnumerator { get; }
    }
}