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
            Record = CommandHandler.Create(() =>
            {
                if (Recording)
                    StopRecord();
                else
                    RunRecord();
            });
            Recording = false;
        }

        [Reactive]
        public ICommand Record { get; private set; }

        [Reactive]
        public bool Recording { get; set; }

        public void RunRecord()
        {
            Recording = true;
        }

        public void StopRecord()
        {
            Recording = false;
        }
    }
}