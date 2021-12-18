using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class Resx : MarkupExtension
    {
        public Resx()
        {
            Key = null;
        }

        public Resx(string key) => Key = key;

        public string? Key { get; set; }

        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            if (Key is null)
                throw new InvalidOperationException("The resource key can not be null.");
            return locales.AppResources.ResourceManager.GetObject(Key);
        }
    }
}