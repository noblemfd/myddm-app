﻿using DDM.API.Core.DTOs.v1.Admin.Request;
using DDM.API.Core.DTOs.v1.Admin.Response;
using DDM.API.Core.DTOs.v1.Merchant.Request;
using DDM.API.Infrastructure.Data.Identiity;
//using DDM.API.Core.DTOs.v1.Merchant.Response;
using DDM.API.Infrastructure.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.Services.v1.Abstract
{
    public interface IAdminService
    {
        //Task<IEnumerable<ApplicationUser>> GetAllActiveUsers();
        //Task<bool> Lock(long id);
        //Task<bool> Unlock(long id);
        Task<GenericResponseDto<AdminUserDto>> CreateAdminUserAsync(AdminCreateDto requestDto);
        Task<GenericResponseDto<AllMerchantListDto>> CreateMerchantAsync(MerchantCreateDto requestDto);
        Task<PagedResponse<AllMerchantListDto>> GetMerchantAsync(int page, int limit);
        Task<GenericResponseDto<AllMerchantListDto>> GetMerchantByIdAsync(long id);
        Task<PagedResponse<AllMerchantListDto>> GetMerchantWithUserAsync(int page, int limit);
        Task<GenericResponseDto<AllMerchantListDto>> GetMerchantWithUserByIdAsync(long id);
        Task<PagedResponse<AllMandateWithDetailListDto>> GetMandateWithDetailListAsync(int page, int limit);
        Task<PagedResponse<AllMandateListDto>> GetMandateCancelledAsync(int page, int limit);
        Task<PagedResponse<AllMandateListDto>> GetMandateCancelledByCustomerAsync(string custAccountNo, int page, int limit);
        Task<GenericResponseDto<AllMandateListDto>> GetMandateCancelledByCustomerRefAsync(string custAccountNo, string mandateRefNo);
        Task<PagedResponse<AllMandateListDto>> GetMandateApprovedAsync(int page, int limit);
        Task<PagedResponse<AllMandateListDto>> GetMandateApprovedByCustomerAsync(string custAccountNo, int page, int limit);
        Task<GenericResponseDto<AllMandateListDto>> GetMandateApprovedByCustomerRefAsync(string custAccountNo, string mandateRefNo);
        Task<PagedResponse<AllMandateDetailListDto>> GetMandatePaymentAsync(int page, int limit);
        Task<PagedResponse<AllMandateDetailListDto>> GetMandatePaymentByCutomerAsync(string custAccountNo, int page, int limit);
        Task<GenericResponseDto<AllMandateDetailListDto>> GetMandatePaymentByCustomerRefAsync(string custAccountNo, string mandateRefNo);
        //Task<GenericResponseDto<AllMerchantListDto>> UpdateMerchantAsync(long id, MerchantCreateDto requestDto);
        Task<GenericResponseDto<bool>> DeleteMerchantAsync(long id);
        Task<PagedResponse<AllMandateListDto>> GetAllMandateAsync(int page, int limit);
        Task<PagedResponse<AllMerchantListDto>> GetAllMandateByMerchantAsync(long merchantId, int page, int limit);
        Task<GenericResponseDto<AllMandateListDto>> GetMandateByIdAsync(long id);
        Task<PagedResponse<AllMandateDetailListDto>> GetAllMandateDetailListAsync(long mandateId, int page, int limit);
        //Task<PagedResponse<AllMandateWithDetailListDto>> GetAllMandateWithDetailListAsync(long mandateId, int page, int limit);
        //Task<PagedResponse<AllStaffListDto>> GetAllStaffAsync(int page, int limit);
        //Task<PagedResponse<AllUserListDto>> GetAllUserAsync(int page, int limit);
        //AdminDashboardDto GetAdminDashboard(string userName);
        List<AdminDashboardCountDto> GetDashboardFieldCount();
        Task<PagedResponse<AllMandateWithDetailListDto>> GetCompletedPaymentListAsync(int page, int limit);
        Task<PagedResponse<AllMandateListDto>> GetThisYearMandateAsync(int page, int limit);
        Task<PagedResponse<AllMandateListDto>> GetLatestMandateAsync(int page, int limit);
        List<AdminMonthlySumDto> GetMandateMonthlySum();
        List<AdminYearlySumDto> GetFiveYearMandate();
    }
}
