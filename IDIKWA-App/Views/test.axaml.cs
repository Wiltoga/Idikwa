using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IDIKWA_App.Views
{
    public partial class test : UserControl
    {
        public test()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
