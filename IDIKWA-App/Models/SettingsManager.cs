using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class SettingsManager
    {
        private static string SettingsFile { get; } = "settings.json";
        private static string SettingsFilePath => Path.Combine(SettingsPath, SettingsFile);
#if DEBUG
        private static string SettingsPath { get; } = ".";
#elif PORTABLE
        private static string SettingsPath { get; } = "data";
#else
        private static string SettingsPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Idikwa");
#endif

        public static void Save(Settings settings)
        {
            try
            {
                Directory.CreateDirectory(SettingsPath);
                File.WriteAllText(SettingsFilePath, JsonConvert.SerializeObject(settings, Formatting.Indented));
            }
            catch (Exception)
            {
            }
        }

        public static Settings? Load()
        {
            try
            {
                return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFilePath));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}