using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.EntityFramework.BaseTypes
{
    public interface ISoftDeleteEntity<TKey> : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        int? DeletedUserId { get; set; }
        string? DeletedUserName { get; set; }
        DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
