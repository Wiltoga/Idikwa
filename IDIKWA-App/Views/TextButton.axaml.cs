using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Windows.Input;

namespace IDIKWA_App
{
    public partial class TextButton : UserControl
    {
        public static readonly StyledProperty<ICommand?> ClickProperty = AvaloniaProperty.Register<TextButton, ICommand?>(nameof(Click), null);
        public static readonly StyledProperty<object?> ParameterProperty = AvaloniaProperty.Register<TextButton, object?>(nameof(Parameter), null);
        public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty.Register<TextButton, string?>(nameof(Text), null);

        private bool pressed;

        public TextButton()
        {
            InitializeComponent();
            pressed = false;
        }

        public ICommand? Click { get => GetValue(ClickProperty); set => SetValue(ClickProperty, value); }
        public object? Parameter { get => GetValue(ParameterProperty); set => SetValue(ParameterProperty, value); }
        public string? Text { get => GetValue(TextProperty); set => SetValue(TextProperty, value); }

        protected override void OnPointerLeave(PointerEventArgs e)
        {
            base.OnPointerLeave(e);
            pressed = false;
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            pressed = true;
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (pressed)
            {
                pressed = false;
                if (Click is not null && Click.CanExecute(Parameter))
                    Click.Execute(Parameter);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}