using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IDIKWA_App
{
    public partial class Maximize : BlankButton
    {
        public Maximize()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}