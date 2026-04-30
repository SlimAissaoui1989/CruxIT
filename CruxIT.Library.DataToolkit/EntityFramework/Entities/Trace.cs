using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruxIT.Library.DataToolkit.EntityFramework.Enums;
using CruxIT.Library.DataToolkit.EntityFramework.BaseTypes;

namespace CruxIT.Library.DataToolkit.EntityFramework.Entities
{
    public class Trace: EntityIdentityIntKeyBase
    {
        [MaxLength(100)]
        public virtual string TableName { get; set; } = null!;

        public virtual string TableId { get; set; } = null!;

        public virtual int UserId { get; set; }

        [MaxLength(255)]
        public virtual string UserName { get; set; } = null!;

        [MaxLength(20)]
        public virtual string? IpAdress {  get; set; }

        [MaxLength(100)]
        public virtual string? Browser { get; set; }

        [MaxLength(100)]
        public virtual string? OS { get; set; }

        [MaxLength(100)]
        public virtual string? Device { get; set; }

        public virtual DateTime ActionDate { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public virtual ActionTypes ActionType { get; set; }

        public virtual string Value { get; set; } = null!;
    }
}
