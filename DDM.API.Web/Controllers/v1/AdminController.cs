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
        public async Task<ActionResult<PagedResponse<AllMerchantListDto>>> GetEAllMerchants(int? page, int? limit)
        {
            var fullPage = page ?? 1;
            var pageSize = limit ?? 10;

            var response = await _adminService.GetMerchantAsync(fullPage, pageSize);
            Response.StatusCode = response.Error != null ? response.Error.ErrorCode : StatusCodes.Status200OK;
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
        [HttpGet("mandates/{merchantId}")]
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
    }
}
