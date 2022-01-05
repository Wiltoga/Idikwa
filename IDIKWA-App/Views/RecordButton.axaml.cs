using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using ReactiveUI;
using System;
using System.Reactive;
using System.Windows.Input;

namespace IDIKWA_App
{
    public partial class RecordButton : UserControl
    {
        public static readonly StyledProperty<ICommand?> ClickProperty = AvaloniaProperty.Register<RecordButton, ICommand?>(nameof(Click), null);
        public static readonly StyledProperty<bool> RecordingProperty = AvaloniaProperty.Register<RecordButton, bool>(nameof(Recording), false);

        public RecordButton()
        {
            InitializeComponent();
        }

        public ICommand? Click { get => GetValue(ClickProperty); set => SetValue(ClickProperty, value); }
        public bool Recording { get => GetValue(RecordingProperty); set => SetValue(RecordingProperty, value); }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}