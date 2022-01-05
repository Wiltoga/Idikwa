using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class BitmapAsset : MarkupExtension
    {
        public BitmapAsset()
        {
            ResourceKey = null;
        }

        public BitmapAsset(string? resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public string? ResourceKey { get; set; }

        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            if (ResourceKey is null)
                return null;

            Uri uri;

            // Allow for assembly overrides
            if (ResourceKey.StartsWith("avares://"))
            {
                uri = new Uri(ResourceKey);
            }
            else
            {
                var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
                uri = new Uri($"avares://{assemblyName}{ResourceKey}");
            }

            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var asset = assets.Open(uri);

            return new Bitmap(asset);
        }
    }
}