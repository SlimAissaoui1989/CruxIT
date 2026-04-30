using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.Events
{
    public delegate Task CxAsyncEventHandler(object sender, EventArgs e);
}
