using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDIKWA_App
{
    public static class CommandHandler
    {
        public static ICommand Create(Action action)
        {
            return new CustomCommand(action);
        }

        public static ICommand Create(Action<object?> action)
        {
            return new CustomCommand(action);
        }

        private class CustomCommand : ICommand
        {
            private Action? action;
            private Action<object?>? parameterAction;

            public CustomCommand(Action action)
            {
                this.action = action;
                parameterAction = null;
            }

            public CustomCommand(Action<object?> action)
            {
                parameterAction = action;
                this.action = null;
            }

            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                if (parameterAction is not null)
                    parameterAction(parameter);
                else if (action is not null)
                    action();
            }
        }
    }
}