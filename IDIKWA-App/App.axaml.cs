using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.Globalization;

namespace IDIKWA_App
{
    public class App : Application
    {
        public override void Initialize()
        {
#if DEBUG
            CultureInfo.CurrentUICulture = new CultureInfo("fr");
            CultureInfo.CurrentCulture = new CultureInfo("fr");
#endif
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}