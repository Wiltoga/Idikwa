using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace IDIKWA_App
{
    public partial class EulaWindow : Window
    {
        public EulaWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void AcceptClick(object sender, RoutedEventArgs e)
        {
            Close(true);
        }

        private void DenyClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}