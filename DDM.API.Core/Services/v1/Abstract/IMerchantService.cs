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
        Task<GenericResponseDto<MerchantUserListDto>> CreateMerchantUserAsync(MerchantUserCreateDto requestDto);
        Task<GenericResponseDto<MandateListDto>> CreateMerchantMandateAsync(MandateCreateDto requestDto);
        Task<PagedResponse<MerchantUserListDto>> GetMerchantUserAsync(int page, int limit);
        Task<GenericResponseDto<MerchantUserListDto>> GetMerchantUserByIdAsync(long id);
        Task<GenericResponseDto<MerchantUserListDto>> UpdateMerchantUserAsync(long id, MerchantUserUpdateDto requestDto);
        //Task<GenericResponseDto<bool>> DeleteMerchantUserAsync(long id);
        Task<GenericResponseDto<MandateListDto>> CancelMandateAsync(long id, MandateCancelDto requestDto);
        Task<GenericResponseDto<MandateListDto>> CancelMandateByCustomerRefAsync(string custAccountNo, string mandateRefNo, MandateCancelDto requestDto);
        //Task<GenericResponseDto<MandateListDto>> RestoreMandateAsync(long id);
        Task<PagedResponse<MandateListDto>> GetMandateCancelledAsync(int page, int limit);
        Task<PagedResponse<MandateListDto>> GetMandateCancelledByCustomerAsync(string custAccountNo, int page, int limit);
        Task<GenericResponseDto<MandateListDto>> GetMandateCancelledByCustomerRefAsync(string custAccountNo, string mandateRefNo);
        Task<PagedResponse<MandateListDto>> GetMandateAsync(int page, int limit);
        Task<PagedResponse<MandateDetailListDto>> GetMandateDetailListAsync(long mandateId, int page, int limit);
        Task<PagedResponse<MandateWithDetailListDto>> GetMandateWithDetailListAsync(int page, int limit);
        Task<PagedResponse<MandateListDto>> GetMandateApprovedAsync(int page, int limit);
        Task<PagedResponse<MandateListDto>> GetMandateApprovedByCustomerAsync(string custAccountNo, int page, int limit);
        Task<GenericResponseDto<MandateListDto>> GetMandateApprovedByCustomerRefAsync(string custAccountNo, string mandateRefNo);
        Task<PagedResponse<MandateDetailListDto>> GetMandatePaymentAsync(int page, int limit);
        Task<PagedResponse<MandateDetailListDto>> GetMandatePaymentByCutomerAsync(string custAccountNo, int page, int limit);
        Task<GenericResponseDto<MandateDetailListDto>> GetMandatePaymentByCustomerRefAsync(string custAccountNo, string mandateRefNo);
        Task<PagedResponse<MandateListDto>> GetMandateByCutomerAsync(string custAccountNo, int page, int limit);
        Task<GenericResponseDto<MandateListDto>> GetMandateByIdAsync(long id);
        Task<GenericResponseDto<MerchantProfileDto>> GetMerchantProfileAsync();
        Task<GenericResponseDto<MerchantListDto>> GetMerchantByIdAsync(long id);
        Task<List<MerchantDashboardCountDto>> GetDashboardFieldCount();
        Task<PagedResponse<MandateWithDetailListDto>> GetCompletedPaymentListAsync(int page, int limit);
        Task<PagedResponse<MandateListDto>> GetThisYearMandateAsync(int page, int limit);
        Task<PagedResponse<MandateListDto>> GetLatestMandateAsync(int page, int limit);
        Task<List<MerchantMonthlySumDto>> GetMandateMonthlySum();
        Task<List<MerchantYearlySumDto>> GetFiveYearMandate();
    }
}
