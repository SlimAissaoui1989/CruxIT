using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.EntityFramework.BaseTypes
{
    public interface IEntityBase
    {
    }

    public interface IEntityBase<TKey> : IEntityBase
        where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
    }
}
