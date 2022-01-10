using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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
    }
}
