using DDM.API.Core.DTOs.v1.Merchant.Request;
using DDM.API.Core.DTOs.v1.Merchant.Response;
using DDM.API.Core.Services.v1.Abstract;
using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.DTOs;
using DDM.API.Infrastructure.Entities.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DDM.API.Web.Controllers.v1
{
    public class MerchantController : BaseApiController
    {
        private readonly IMerchantService _merchantService;
        //private readonly UserManager<ApplicationUser> _userManager;
        public MerchantController(IMerchantService merchantService)
        {
            //_userManager = userManager;
            _merchantService = merchantService;
        }

        [HttpPost("mandate/add")]
        [Authorize(Roles = UserRoles.Merchant)]  //[FromForm] 
        public async Task<ActionResult<GenericResponseDto<MandateListDto>>> AddMandate(MandateCreateDto request)
        {
            var response = await _merchantService.CreateMerchantMandateAsync(request);
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status201Created;
            return new JsonResult(response);
        }

        [HttpGet("mandates")]
        [Authorize(Roles = UserRoles.Merchant)]  //[FromForm] 
        public async Task<ActionResult<PagedResponse<MandateListDto>>> GetMandates(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _merchantService.GetMandateAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/mandate-details/{mandateId}")]
        [Authorize(Roles = UserRoles.Merchant)]
        public async Task<ActionResult<PagedResponse<MandateDetailListDto>>> GetMandateDetails(long mandateId, int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _merchantService.GetMandateDetailListAsync(mandateId, fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/mandate-with-details")]
        [Authorize(Roles = UserRoles.Merchant)]  //[FromForm] 
        public async Task<ActionResult<PagedResponse<MandateWithDetailListDto>>> GetMandateWithDetails(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _merchantService.GetMandateWithDetailListAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/approved-mandates")]
        [Authorize(Roles = UserRoles.Merchant)]
        public async Task<ActionResult<PagedResponse<MandateListDto>>> GetMandateApproved(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _merchantService.GetMandateApprovedAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/approved-mandates-by-acctno/{custAccountNo}")]
        [Authorize(Roles = UserRoles.Merchant)]
        public async Task<ActionResult<PagedResponse<MandateListDto>>> GetMandateApprovedByCustomer(string custAccountNo, int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _merchantService.GetMandateApprovedByCustomerAsync(custAccountNo, fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/approved-mandates-by-acctno-refno/{custAccountNo}/{mandateRefNo}")]
        [Authorize(Roles = UserRoles.Merchant)]
        public async Task<ActionResult<GenericResponseDto<MandateListDto>>> GetMandateApprovedByCustomerRef(string custAccountNo, string mandateRefNo)
        {
            var response = await _merchantService.GetMandateApprovedByCustomerRefAsync(custAccountNo, mandateRefNo);
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/mandate-payment")]
        [Authorize(Roles = UserRoles.Merchant)]
        public async Task<ActionResult<PagedResponse<MandateDetailListDto>>> GetMandatePayment(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _merchantService.GetMandatePaymentAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/mandate-payment-by-acctno/{custAccountNo}")]
        [Authorize(Roles = UserRoles.Merchant)]
        public async Task<ActionResult<PagedResponse<MandateDetailListDto>>> GetMandatePaymentByCutomer(string custAccountNo, int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _merchantService.GetMandatePaymentByCutomerAsync(custAccountNo, fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/mandate-payment-by-acctno-refno/{custAccountNo}/{mandateRefNo}")]
        [Authorize(Roles = UserRoles.Merchant)]
        public async Task<ActionResult<GenericResponseDto<MandateDetailListDto>>> GetMandatePaymentByCustomerRef(string custAccountNo, string mandateRefNo)
        {
            var response = await _merchantService.GetMandatePaymentByCustomerRefAsync(custAccountNo, mandateRefNo);
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/mandate-by-customer/{custAccountNo}")]
        [Authorize(Roles = UserRoles.Merchant)]
        public async Task<ActionResult<PagedResponse<MandateListDto>>> GetMandateByCutomers(string custAccountNo, int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _merchantService.GetMandateByCutomerAsync(custAccountNo, fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/{id}")]
        [Authorize(Roles = UserRoles.Merchant)]  //[FromForm] 
        public async Task<ActionResult<GenericResponseDto<MandateListDto>>> GetMandateById(long id)
        {
            var response = await _merchantService.GetMandateByIdAsync(id);
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("profile")]
        [Authorize(Roles = UserRoles.Merchant)]  //[FromForm] 
        public async Task<ActionResult<GenericResponseDto<MerchantProfileDto>>> GetMerchantProfile()
        {
            var response = await _merchantService.GetMerchantProfileAsync();
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("merchant/{id}")]
        [Authorize(Roles = UserRoles.Merchant)]
        public async Task<ActionResult<GenericResponseDto<MerchantListDto>>> GetMerchant(long id)
        {
            var response = await _merchantService.GetMerchantByIdAsync(id);
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("dashboard/data-count")]
        [Authorize(Roles = UserRoles.Merchant)]
        public List<MerchantDashboardCountDto> GetDashboardFieldCount()
        {
            return _merchantService.GetDashboardFieldCount();
        }

        [HttpGet("mandates/completed-payments")]
        [Authorize(Roles = UserRoles.Merchant)]  //[FromForm] 
        public async Task<ActionResult<PagedResponse<MandateWithDetailListDto>>> GetCompletedPaymentList(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _merchantService.GetCompletedPaymentListAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/this-year-mandate")]
        [Authorize(Roles = UserRoles.Merchant)]  //[FromForm] 
        public async Task<ActionResult<PagedResponse<MandateListDto>>> GetThisYearMandate(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _merchantService.GetThisYearMandateAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("mandates/latest-mandate")]
        [Authorize(Roles = UserRoles.Merchant)]  //[FromForm] 
        public async Task<ActionResult<PagedResponse<MandateListDto>>> GetLatestMandate(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _merchantService.GetLatestMandateAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("dashboard/current-year-monthly-mandate")]
        [Authorize(Roles = UserRoles.Merchant)]
        public List<MerchantMonthlySumDto> GetDashboardMonthlyMandate()
        {
            return _merchantService.GetMandateMonthlySum();
        }

        [HttpGet("dashboard/last-five-year-mandate")]
        [Authorize(Roles = UserRoles.Merchant)]
        public List<MerchantYearlySumDto> GetDashboardFiveYearMandate()
        {
            return _merchantService.GetFiveYearMandate();
        }
    }
}
