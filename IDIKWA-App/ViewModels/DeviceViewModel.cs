using NAudio.CoreAudioApi;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class DeviceViewModel : ReactiveObject
    {
        public DeviceViewModel(MMDevice device, bool recording, int volume)
        {
            Device = device;
            Recording = recording;
            Volume = volume;
        }

        [Reactive]
        public MMDevice Device { get; private set; }

        [Reactive]
        public bool Recording { get; set; }

        [Reactive]
        public int Volume { get; set; }
    }
}