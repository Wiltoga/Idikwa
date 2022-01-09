using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.Globalization;

namespace IDIKWA_App
{
    public class App : Application
    {
        public static Settings? InitialSettings { get; private set; }

        public override void Initialize()
        {
            InitialSettings = SettingsManager.Load();
            if (InitialSettings is not null)
            {
                CultureInfo.CurrentUICulture = new CultureInfo(InitialSettings.Culture);
            }
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