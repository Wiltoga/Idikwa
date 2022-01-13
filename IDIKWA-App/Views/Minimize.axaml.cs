using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IDIKWA_App
{
    public partial class Minimize : BlankButton
    {
        public Minimize()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}