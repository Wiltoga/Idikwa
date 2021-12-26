using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDIKWA_App
{
    public class LocalFactory : IFactory
    {
        public async Task<string> SaveAsync(Record record)
        {
            return await Task.FromResult("");
        }
    }
}