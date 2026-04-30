using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.EntityFramework.BaseTypes
{
    public abstract class SoftDeleteEntity<TKey> : Entity<TKey>, ISoftDeleteEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual int? DeletedUserId { get; set; }
        public virtual string? DeletedUserName { get; set; }
        public virtual DateTime? DeletedDate { get; set; }
        public virtual bool IsDeleted { get; set; }
    }
}
