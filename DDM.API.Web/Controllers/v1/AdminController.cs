using DDM.API.Core.DTOs.v1.Admin.Request;
using DDM.API.Core.DTOs.v1.Admin.Response;
using DDM.API.Core.Services.v1.Abstract;
using DDM.API.Infrastructure.Entities.DTOs;
using DDM.API.Infrastructure.Entities.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDM.API.Web.Controllers.v1
{
    public class AdminController : BaseApiController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("register/myadmin")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<GenericResponseDto<AdminUserDto>>> Register(AdminCreateDto registrationRequest)
        {
            var response = await _adminService.CreateAdminUserAsync(registrationRequest);
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpPost("merchant/add")]
        [Authorize(Roles = UserRoles.Admin)]  //[FromForm] 
        public async Task<ActionResult<GenericResponseDto<AllMerchantListDto>>> AddMerchant(MerchantCreateDto request)
        {
            var response = await _adminService.CreateMerchantAsync(request);
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status201Created;
            return new JsonResult(response);
        }

        [HttpGet("merchants")]
        [Authorize(Roles = UserRoles.Admin)]  //[FromForm] 
        public async Task<ActionResult<PagedResponse<AllMerchantListDto>>> GetAllMerchants(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetMerchantAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("merchants/merchants-with-users")]
        [Authorize(Roles = UserRoles.Admin)]  //[FromForm] 
        public async Task<ActionResult<PagedResponse<AllMerchantListDto>>> GetAllMerchantWithUsers(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetMerchantWithUserAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("merchant/merchant-with-user/{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<GenericResponseDto<AllMerchantListDto>>> GetMerchantWithUser(long id)
        {
            var response = await _adminService.GetMerchantWithUserByIdAsync(id);
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/mandate-with-details")]
        [Authorize(Roles = UserRoles.Admin)]  //[FromForm] 
        public async Task<ActionResult<PagedResponse<AllMandateWithDetailListDto>>> GetMandateWithDetails(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetMandateWithDetailListAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/cancelled-mandates")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<PagedResponse<AllMandateListDto>>> GetMandateCancelled(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetMandateCancelledAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/cancelled-mandates-by-acctno/{custAccountNo}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<PagedResponse<AllMandateListDto>>> GetMandateCancelledByCustomer(string custAccountNo, int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetMandateCancelledByCustomerAsync(custAccountNo, fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/cancelled-mandates-by-acctno-refno/{custAccountNo}/{mandateRefNo}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<GenericResponseDto<AllMandateListDto>>> GetMandateCancelledByCustomerRef(string custAccountNo, string mandateRefNo)
        {
            var response = await _adminService.GetMandateCancelledByCustomerRefAsync(custAccountNo, mandateRefNo);
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/approved-mandates")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<PagedResponse<AllMandateListDto>>> GetMandateApproved(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetMandateApprovedAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/approved-mandates-by-acctno/{custAccountNo}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<PagedResponse<AllMandateListDto>>> GetMandateApprovedByCustomer(string custAccountNo, int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetMandateApprovedByCustomerAsync(custAccountNo, fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/approved-mandates-by-acctno-refno/{custAccountNo}/{mandateRefNo}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<GenericResponseDto<AllMandateListDto>>> GetMandateApprovedByCustomerRef(string custAccountNo, string mandateRefNo)
        {
            var response = await _adminService.GetMandateApprovedByCustomerRefAsync(custAccountNo, mandateRefNo);
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/mandate-payment")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<PagedResponse<AllMandateDetailListDto>>> GetMandatePayment(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetMandatePaymentAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/mandate-payment-by-acctno/{custAccountNo}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<PagedResponse<AllMandateDetailListDto>>> GetMandatePaymentByCutomer(string custAccountNo, int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetMandatePaymentByCutomerAsync(custAccountNo, fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/mandate-payment-by-acctno-refno/{custAccountNo}/{mandateRefNo}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<GenericResponseDto<AllMandateDetailListDto>>> GetMandatePaymentByCustomerRef(string custAccountNo, string mandateRefNo)
        {
            var response = await _adminService.GetMandatePaymentByCustomerRefAsync(custAccountNo, mandateRefNo);
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("merchant/{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<GenericResponseDto<AllMerchantListDto>>> GetMerchant(long id)
        {
            var response = await _adminService.GetMerchantByIdAsync(id);
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates")]
        [Authorize(Roles = UserRoles.Admin)]  //[FromForm] 
        public async Task<ActionResult<PagedResponse<AllMandateListDto>>> GetAllMandates(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetAllMandateAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        //[HttpGet, Route("Mandates/{merchantId}")]
        [HttpGet("mandates/mandates-by-merchant/{merchantId}")]
        [Authorize(Roles = UserRoles.Admin)]  //[FromForm] 
        public async Task<ActionResult<PagedResponse<AllMandateListDto>>> GetAllMandatesByMerchant(long merchantId, int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetAllMandateByMerchantAsync(merchantId, fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/{id}")]
        [Authorize(Roles = UserRoles.Admin)]  //[FromForm] 
        public async Task<ActionResult<GenericResponseDto<AllMandateListDto>>> GetMandateById(long id)
        {
            var response = await _adminService.GetMandateByIdAsync(id);
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/mandate-details/{mandateId}")]
        [Authorize(Roles = UserRoles.Admin)] 
        public async Task<ActionResult<PagedResponse<AllMandateDetailListDto>>> GetAllMandateDetails(long mandateId, int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetAllMandateDetailListAsync(mandateId, fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("dashboard/data-count")]
        [Authorize(Roles = UserRoles.Admin)]
        public List<AdminDashboardCountDto> GetDashboardFieldCount()
        {
            return _adminService.GetDashboardFieldCount();
        }

        [HttpGet("mandates/completed-payments")]
        [Authorize(Roles = UserRoles.Admin)]  //[FromForm] 
        public async Task<ActionResult<PagedResponse<AllMandateWithDetailListDto>>> GetCompletedPaymentList(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetCompletedPaymentListAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/this-year-mandate")]
        [Authorize(Roles = UserRoles.Admin)]  //[FromForm] 
        public async Task<ActionResult<PagedResponse<AllMandateListDto>>> GetThisYearMandate(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetThisYearMandateAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("dashboard/current-year-monthly-mandate")]
        [Authorize(Roles = UserRoles.Admin)]
        public List<AdminMonthlySumDto> GetDashboardMonthlyMandate()
        {
            return _adminService.GetMandateMonthlySum();
        }

        [HttpGet("dashboard/last-five-year-mandate")]
        [Authorize(Roles = UserRoles.Admin)]
        public List<AdminYearlySumDto> GetDashboardFiveYearMandate()
        {
            return _adminService.GetFiveYearMandate();
        }

        [HttpGet("mandates/latest-mandate")]
        [Authorize(Roles = UserRoles.Admin)]  //[FromForm] 
        public async Task<ActionResult<PagedResponse<AllMandateListDto>>> GetLatestMandate(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetLatestMandateAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }
    }
}
