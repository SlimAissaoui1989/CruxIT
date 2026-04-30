using AutoMapper;
using CruxIT.Library.API.DTO;
using CruxIT.Library.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruxIT.Library.API.Profiles
{

    public class CxExceptionProfile : Profile
    {
        public CxExceptionProfile()
        {
            CreateMap<CxException, CxExceptionResponse>();
        }
    }
}
