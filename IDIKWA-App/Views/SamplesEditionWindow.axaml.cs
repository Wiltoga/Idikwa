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

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            if (DataContext is SamplesEditionViewModel viewmodel)
                viewmodel.Window = this;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is SamplesEditionViewModel viewmodel)
            {
                viewmodel.Save();
                Close();
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void SaveAsClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is SamplesEditionViewModel viewmodel)
            {
                await viewmodel.SaveAs();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (DataContext is SamplesEditionViewModel viewmodel)
            {
                viewmodel.Save();
            }
        }
    }
}