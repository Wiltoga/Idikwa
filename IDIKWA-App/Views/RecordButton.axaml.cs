using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System;
using System.Reactive;
using System.Windows.Input;

namespace IDIKWA_App
{
    public partial class RecordButton : UserControl
    {
        public static readonly StyledProperty<ICommand?> CommandProperty = AvaloniaProperty.Register<RecordButton, ICommand?>(nameof(Command), null);

        public RecordButton()
        {
            InitializeComponent();
        }

        public ICommand? Command { get => GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}