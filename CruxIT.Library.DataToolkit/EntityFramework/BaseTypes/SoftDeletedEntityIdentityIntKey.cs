using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.DataToolkit.EntityFramework.BaseTypes
{
    public abstract class SoftDeletedEntityIdentityIntKey : SoftDeleteEntity<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get => base.Id; set => base.Id = value; }
    }
}
