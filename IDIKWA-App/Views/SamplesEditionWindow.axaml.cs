using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace IDIKWA_App
{
    public partial class SamplesEditionWindow : Window
    {
        public SamplesEditionWindow()
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

        private void SaveDownClick(object sender, RoutedEventArgs e)
        {
            if (sender is Control control && control.Tag is ContextMenu menu)
            {
                menu.Open(control);
            }
        }
    }
}