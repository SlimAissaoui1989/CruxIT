using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.Exceptions
{
    public enum CxExceptionTypes
    {
        NotFound = 404,
        BadRequest = 400,
        Unauthorized = 401,
        Forbid = 403,
        UnprocessableEntity = 422,
        InternalServerError = 500,
    }
}
