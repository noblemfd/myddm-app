using DDM.API.Core.DTOs.v1.Merchant.Request;
using DDM.API.Core.DTOs.v1.Merchant.Response;
using DDM.API.Infrastructure.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.Services.v1.Abstract
{
    public interface IMerchantService
    {
        Task<GenericResponseDto<MandateListDto>> CreateMerchantMandateAsync(MandateCreateDto requestDto);
        //Task<GenericResponseDto<bool>> CancelMandateAsync(long id);
        Task<PagedResponse<MandateListDto>> GetMandateAsync(int page, int limit);
        Task<PagedResponse<MandateDetailListDto>> GetMandateDetailListAsync(long mandateId, int page, int limit);
        Task<PagedResponse<MandateWithDetailListDto>> GetMandateWithDetailListAsync(int page, int limit);
        Task<PagedResponse<MandateListDto>> GetMandateByCutomerAsync(string custAccountNo, int page, int limit);
        Task<GenericResponseDto<MandateListDto>> GetMandateByIdAsync(long id);
        Task<GenericResponseDto<MerchantProfileDto>> GetMerchantProfileAsync();
        Task<GenericResponseDto<MerchantListDto>> GetMerchantByIdAsync(long id);
        List<MerchantDashboardCountDto> GetDashboardFieldCount();
        Task<PagedResponse<MandateWithDetailListDto>> GetCompletedPaymentListAsync(int page, int limit);
        Task<PagedResponse<MandateListDto>> GetThisYearMandateAsync(int page, int limit);
        Task<PagedResponse<MandateListDto>> GetLatestMandateAsync(int page, int limit);
        //Task<MandateListDto> GetMonthlyMandateAsync();
        //Task<MandateListDto> GetFiveYearMandateAsync();
    }
}
