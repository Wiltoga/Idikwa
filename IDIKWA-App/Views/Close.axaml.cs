using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IDIKWA_App
{
    public partial class Close : BlankButton
    {
        public Close()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}