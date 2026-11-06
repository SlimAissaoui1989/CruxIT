using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.EntityFramework.BaseTypes
{
    public abstract class Entity<TKey> : EntityBase<TKey>, IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual Guid? CreatedUserId { get; set; }
        public virtual string? CreatedUserName { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual Guid? LastUpdatedUserId { get; set; }
        public virtual string? LastUpdatedUserName { get; set; }
        public virtual DateTime? LastUpdatedDate { get; set; }
    }
}
