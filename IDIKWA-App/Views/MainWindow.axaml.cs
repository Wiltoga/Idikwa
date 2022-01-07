using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Xaml.Interactions.Custom;
using System;

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
    }
}