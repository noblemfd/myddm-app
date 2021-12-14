using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DDM.API.Core.DTOs.v1.Merchant.Request;
using DDM.API.Core.DTOs.v1.Merchant.Response;
using DDM.API.Infrastructure.Entities.Models;

namespace DDM.API.Core.ProfileMapping.v1
{
    public class MerchantMapperProfile : Profile
    {
        public MerchantMapperProfile()
        {
            CreateMap<MandateCreateDto, Mandate>();

            CreateMap<Merchant, MerchantProfileDto>()
                .ForMember(e => e.UserName, options => options.MapFrom(e => e.User != null ? e.User.UserName : null))
                .ForMember(e => e.MobileNumber, options => options.MapFrom(e => e.User != null ? e.User.MobileNumber : null));

            CreateMap<Mandate, MandateListDto>();

            CreateMap<MandateDetail, MandateDetailListDto>();

            CreateMap<Mandate, MandateWithDetailListDto>();
        }
    }
}
