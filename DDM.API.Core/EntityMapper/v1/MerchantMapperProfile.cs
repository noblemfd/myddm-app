using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DDM.API.Core.DTOs.v1.Admin.Response;
using DDM.API.Core.DTOs.v1.Merchant.Request;
using DDM.API.Core.DTOs.v1.Merchant.Response;
using DDM.API.Infrastructure.Entities.Models;

namespace DDM.API.Core.ProfileMapping.v1
{
    public class MerchantMapperProfile : Profile
    {
        public MerchantMapperProfile()
        {
            CreateMap<MerchantUserCreateDto, MerchantUser>()
                //.ForMember(u => u.PasswordHash, options => options.Ignore())
                .ForMember(e => e.User, options => options.Ignore()).ReverseMap();

            CreateMap<MerchantUserUpdateDto, MerchantUser>()
                //.ForMember(u => u.PasswordHash, options => options.Ignore())
                .ForMember(e => e.User, options => options.Ignore()).ReverseMap();

            CreateMap<MandateCreateDto, Mandate>().ReverseMap();

            CreateMap<MandateCancelDto, Mandate>().ReverseMap();

            CreateMap<Merchant, MerchantProfileDto>()
                .ForMember(e => e.UserName, options => options.MapFrom(e => e.User != null ? e.User.UserName : null))
                .ForMember(e => e.MobileNumber, options => options.MapFrom(e => e.User != null ? e.User.MobileNumber : null)).ReverseMap();

            CreateMap<Mandate, MandateListDto>();

            CreateMap<MandateDetail, MandateDetailListDto>().ReverseMap();

            CreateMap<Mandate, MandateWithDetailListDto>().ReverseMap();

            CreateMap<MandateDetail, MandateDetailListDto>().ReverseMap();

            CreateMap<Merchant, MerchantListDto>().ReverseMap();
            CreateMap<MerchantUser, AllMerchantUserListDto>().ReverseMap();
        }
    }
}