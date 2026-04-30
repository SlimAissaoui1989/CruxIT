using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.API.DTO
{
    public class CxExceptionResponse
    {
        public string? Message { get; set; } = null!;
        public string? MessageResourceKey { get; set; }
        public object?[]? MessageResourceArgs { get; set; }
    }
}
