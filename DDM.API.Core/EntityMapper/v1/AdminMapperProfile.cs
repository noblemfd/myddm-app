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
                .ForMember(e => e.User, options => options.Ignore()).ReverseMap();

            CreateMap<Merchant, AllMerchantListDto>()
                .ForMember(e => e.UserName, options => options.MapFrom(e => e.User != null ? e.User.UserName : null))
                .ForMember(e => e.MobileNumber, options => options.MapFrom(e => e.User != null ? e.User.MobileNumber : null)).ReverseMap();

            CreateMap<AdminCreateDto, ApplicationUser>()
                .ForMember(u => u.PasswordHash, options => options.Ignore())
                .ForMember(u => u.UserName, options => options.MapFrom(x => x.UserName)).ReverseMap();

            CreateMap<Mandate, AllMandateListDto>().ReverseMap();

            CreateMap<Mandate, AllMandateWithDetailListDto>().ReverseMap();

            CreateMap<MandateDetail, AllMandateDetailListDto>().ReverseMap();

            CreateMap<Merchant, AllMerchantListDto>().ReverseMap();

            CreateMap<ApplicationUser, AllUserListDto>().ReverseMap();
            CreateMap<MerchantUser, AllMerchantUserListDto>().ReverseMap();
        }
    }
}
