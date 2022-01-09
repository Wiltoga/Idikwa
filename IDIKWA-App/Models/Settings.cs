using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    [Serializable]
    public record Settings(int BitRate, Dictionary<string, float> Volumes, TimeSpan Duration, string OutputPath, List<string> RecordingDevices, int SampleRate, string Culture);
}