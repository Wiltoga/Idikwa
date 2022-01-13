using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using System;
using System.Windows.Input;

namespace IDIKWA_App
{
    public partial class BlankButton : ContentControl, IStyleable
    {
        public static readonly StyledProperty<ICommand?> CommandProperty = AvaloniaProperty.Register<BlankButton, ICommand?>(nameof(Command), null);
        public static readonly DirectProperty<BlankButton, bool> IsPressedProperty = AvaloniaProperty.RegisterDirect<BlankButton, bool>(nameof(IsPressed), o => o.isPressed);
        public static readonly StyledProperty<object?> ParameterProperty = AvaloniaProperty.Register<BlankButton, object?>(nameof(Parameter), null);
        private bool isPressed;

        public BlankButton() : base()
        {
        }

        public event EventHandler? Click;

        public ICommand? Command { get => GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        public bool IsPressed
        {
            get => isPressed;
            private set
            {
                if (isPressed != value)
                {
                    SetAndRaise(IsPressedProperty, ref isPressed, value);
                }
            }
        }

        public object? Parameter { get => GetValue(ParameterProperty); set => SetValue(ParameterProperty, value); }
        Type IStyleable.StyleKey => typeof(ContentControl);

        protected override void OnPointerLeave(PointerEventArgs e)
        {
            IsPressed = false;
            base.OnPointerLeave(e);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
            {
                e.Handled = true;
                IsPressed = true;
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if (IsPressed && e.InitialPressMouseButton == MouseButton.Left)
            {
                e.Handled = true;
                IsPressed = false;
                if (Command?.CanExecute(Parameter) is true)
                    Command.Execute(Parameter);
                Click?.Invoke(this, new EventArgs());
            }
            base.OnPointerReleased(e);
        }
    }
}