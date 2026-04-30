using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.EntityFramework.BaseTypes
{
    public abstract class EntityBase<TKey> : IEntityBase<TKey>
        where TKey : IEquatable<TKey>
    {
        [Key]
        public virtual TKey Id { get; set; } = default!;
    }
}
