using Avalonia;
using Avalonia.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDIKWA_App
{
    internal class CommandAction : AvaloniaObject, IAction
    {
        public static readonly StyledProperty<ICommand?> CommandProperty = AvaloniaProperty.Register<CommandAction, ICommand?>(nameof(Command), null);
        public static readonly StyledProperty<object?> ParameterProperty = AvaloniaProperty.Register<CommandAction, object?>(nameof(Parameter), null);

        public ICommand? Command { get => GetValue(CommandProperty); set => SetValue(CommandProperty, value); }
        public object? Parameter { get => GetValue(ParameterProperty); set => SetValue(ParameterProperty, value); }

        public object? Execute(object? sender, object? parameter)
        {
            if (Command is not null && Command.CanExecute(Parameter))
            {
                Command.Execute(Parameter);
            }
            return null;
        }
    }
}