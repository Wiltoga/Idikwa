using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    [Serializable]
    public record Settings(int BitRate, float MasterVolume, Dictionary<string, float> Volumes, TimeSpan Duration, bool Mono, string OutputPath, List<string> RecordingDevices, int SampleRate, string Culture);
}