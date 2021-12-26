using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public interface IFactory
    {
        public Task<string> SaveAsync(Record record);
    }
}