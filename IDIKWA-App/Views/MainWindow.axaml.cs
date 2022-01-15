using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Xaml.Interactions.Custom;
using System;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            if (DataContext is MainWindowViewModel viewmodel)
                viewmodel.Window = this;
        }

        protected override async void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
#if !PORTABLE
            if (App.InitialSettings?.Eula is not true)
#endif
            {
                var eula = new EulaWindow();
                if (await eula.ShowDialog<bool?>(this) is not true)
                    Close();
                else
            if (DataContext is MainWindowViewModel viewmodel)
                {
                    viewmodel.Settings.EulaAccepted = true;
                    SettingsManager.Save(viewmodel.Settings.Model);
                }
            }
            {
                if (DataContext is MainWindowViewModel viewmodel)
                {
                    _ = new ApiHandler(viewmodel).StartAsync();
                }
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Minimize(object sender, EventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize(object sender, EventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Close(object sender, EventArgs e)
        {
            Close();
        }

        protected override async void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (DataContext is MainWindowViewModel viewmodel)
                await viewmodel.Exit();
        }
    }
}