using DDM.API.Core.DTOs.v1.Merchant.Request;
using DDM.API.Core.DTOs.v1.Merchant.Response;
using DDM.API.Core.Services.v1.Abstract;
using DDM.API.Infrastructure.Entities.DTOs;
using DDM.API.Infrastructure.Entities.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public MerchantController(IMerchantService merchantService)
        {
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
            var httpContext = new HttpContextAccessor();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            var username = httpContext.HttpContext.User.Identity.Name;
          //  int.Parse(((ClaimsIdentity)HttpContext.User.Identity).ValueFromType("UserId"));
            //var userId = Convert.ToInt64(HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            //  var userId = long.Parse(httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //  var userId = long.Parse(httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //var response = await _merchantService.GetMerchantProfileAsync(userId);
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
    }
}
