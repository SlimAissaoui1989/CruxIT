using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.EntityFramework.BaseTypes
{
    public interface IEntity<TKey> : IEntityBase<TKey>
        where TKey : IEquatable<TKey>
    {
        int CreatedUserId { get; set; }
        string? CreatedUserName { get; set; }
        DateTime CreatedDate{ get; set; }
        int? LastUpdatedUserId { get; set; }
        string? LastUpdatedUserName{ get; set; }
        DateTime? LastUpdatedDate{ get; set; }
    }
}
