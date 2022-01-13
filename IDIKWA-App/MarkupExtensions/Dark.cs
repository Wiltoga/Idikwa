using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class Dark : MarkupExtension, IValueConverter
    {
        private static readonly Color dark = Color.FromArgb(255, 20, 20, 20);
        private static readonly Color light = Color.FromArgb(255, 255, 255, 255);

        public Dark(Color c) => Color = c;

        public Dark() => Color = Colors.White;

        public Color Color { get; set; }

        public static Color GetDarkTheme(Color c)
        {
            var hsl = GetHSL(c);
            double luminosity = 1 - hsl.Item3;
            double haloLuminosity = 1 - GetHSL(light).Item3;
            double themeBackgroundLuminosity = GetHSL(dark).Item3;

            if (luminosity < haloLuminosity)
                return GetRGB(c.A, hsl.Item1, hsl.Item2, themeBackgroundLuminosity * luminosity / haloLuminosity);
            else
                return GetRGB(c.A, hsl.Item1, hsl.Item2, (1.0 - themeBackgroundLuminosity) * (luminosity - 1.0) / (1.0 - haloLuminosity) + 1.0);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush(Conversion(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new SolidColorBrush(Conversion(Color));
        }

        private static (double, double, double) GetHSL(Color c)
        {
            double r = c.R / 255.0;
            double g = c.G / 255.0;
            double b = c.B / 255.0;
            double max = Math.Max(Math.Max(r, g), b);
            double min = Math.Min(Math.Min(r, g), b);
            double lum = (max + min) / 2;
            double satur, hue;
            if (min == max)
                satur = 0;
            else if (lum < .5)
                satur = (max - min) / (max + min);
            else
                satur = (max - min) / (2.0 - max - min);
            if (r == max)
                hue = (g - b) / (max - min);
            else if (g == max)
                hue = 2.0 + (b - r) / (max - min);
            else
                hue = 4.0 + (r - g) / (max - min);
            hue = (hue * 60 + 360) % 360;
            return (hue, satur, lum);
        }

        private static Color GetRGB(byte alpha, double hue, double satur, double lum)
        {
            double v;
            hue /= 360;
            double r, g, b;
            r = lum;   // default to gray
            g = lum;
            b = lum;
            v = (lum <= 0.5) ? (lum * (1.0 + satur)) : (lum + satur - lum * satur);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;
                m = lum + lum - v;
                sv = (v - m) / v;
                hue *= 6.0;
                sextant = (int)hue;
                fract = hue - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;

                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;

                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;

                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;

                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;

                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            var rgb = Color.FromArgb(alpha, System.Convert.ToByte(r * 255.0f), System.Convert.ToByte(g * 255.0f), System.Convert.ToByte(b * 255.0f));
            return rgb;
        }

        private Color Conversion(object? value)
        {
            if (value is Color c)
            {
                return GetDarkTheme(c);
            }
            else if (value is string str)
            {
                return GetDarkTheme(Color.Parse(str));
            }
            else
            {
                return Colors.White;
            }
        }
    }
}