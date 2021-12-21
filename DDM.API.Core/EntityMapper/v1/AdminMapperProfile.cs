using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DDM.API.Core.DTOs.v1.Admin.Request;
using DDM.API.Core.DTOs.v1.Admin.Response;
using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.Models;

namespace DDM.API.Core.EntityMapper.v1
{
    public class AdminMapperProfile :  Profile
    {
        public AdminMapperProfile()
        {
            CreateMap<MerchantCreateDto, Merchant>()
                .ForMember(e => e.User, options => options.Ignore());

            CreateMap<Merchant, AllMerchantListDto>()
                .ForMember(e => e.UserName, options => options.MapFrom(e => e.User != null ? e.User.UserName : null))
                .ForMember(e => e.MobileNumber, options => options.MapFrom(e => e.User != null ? e.User.MobileNumber : null));

            CreateMap<AdminCreateDto, ApplicationUser>()
                .ForMember(u => u.PasswordHash, options => options.Ignore())
                .ForMember(u => u.UserName, options => options.MapFrom(x => x.UserName));

            CreateMap<Mandate, AllMandateListDto>();

            CreateMap<MandateDetail, AllMandateDetailListDto>();
        }
    }
}
