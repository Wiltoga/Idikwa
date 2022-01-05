using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDIKWA_App
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            Test = CommandHandler.Create(() => Console.WriteLine("enregistrement..."));
        }

        [Reactive]
        public ICommand Test { get; private set; }
    }
}