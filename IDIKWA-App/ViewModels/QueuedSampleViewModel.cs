using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class QueuedSampleViewModel : ReactiveObject
    {
        public QueuedSampleViewModel(IEnumerable<RecordViewModel> records)
        {
            Records = records;
            Title = DateTime.Now.ToString("HH:mm:ss");
        }

        public IEnumerable<RecordViewModel> Records { get; }
        public string Title { get; }
    }
}