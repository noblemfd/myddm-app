using DDM.API.Core.DTOs.v1.Admin.Request;
using DDM.API.Core.DTOs.v1.Admin.Response;
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
    public interface IAdminService
    {
        Task<GenericResponseDto<AdminUserDto>> CreateAdminUserAsync(AdminCreateDto requestDto);
        Task<GenericResponseDto<AllMerchantListDto>> CreateMerchantAsync(MerchantCreateDto requestDto);
        Task<PagedResponse<AllMerchantListDto>> GetMerchantAsync(int page, int limit);
        Task<GenericResponseDto<AllMerchantListDto>> GetMerchantByIdAsync(long id);
        Task<PagedResponse<AllMandateWithDetailListDto>> GetMandateWithDetailListAsync(int page, int limit);
        //Task<GenericResponseDto<AllMerchantListDto>> UpdateMerchantAsync(long id, MerchantCreateDto requestDto);
        Task<GenericResponseDto<bool>> DeleteMerchantAsync(long id);
        Task<PagedResponse<AllMandateListDto>> GetAllMandateAsync(int page, int limit);
        Task<PagedResponse<AllMerchantListDto>> GetAllMandateByMerchantAsync(long merchantId, int page, int limit);
        Task<GenericResponseDto<AllMandateListDto>> GetMandateByIdAsync(long id);
        Task<PagedResponse<AllMandateDetailListDto>> GetAllMandateDetailListAsync(long mandateId, int page, int limit);
        //Task<PagedResponse<AllMandateWithDetailListDto>> GetAllMandateWithDetailListAsync(long mandateId, int page, int limit);
        //Task<PagedResponse<AllStaffListDto>> GetAllStaffAsync(int page, int limit);
        //Task<PagedResponse<AllUserListDto>> GetAllUserAsync(int page, int limit);
        AdminDashboardDto GetAdminDashboard(string userName);
    }
}
