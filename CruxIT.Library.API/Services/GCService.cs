using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.API.Services
{
    public class GCService : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            // Ensure that GC is only triggered after this service is disposed
            //GC.Collect(2, GCCollectionMode.Optimized);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            await Task.CompletedTask;
        }
    }
}
