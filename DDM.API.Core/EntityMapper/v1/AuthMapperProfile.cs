using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DDM.API.Core.DTOs.v1.Authentication;
using DDM.API.Infrastructure.Data.Identiity;

namespace DDM.API.Core.EntityMappe.v1
{
    public class AuthMapperProfile : Profile
    {
        public AuthMapperProfile()
        {
            CreateMap<ApplicationUser, UserDto>();
        }
    }
}
