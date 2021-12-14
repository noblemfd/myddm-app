using DDM.API.Core.DTOs.v1.Authentication;
using DDM.API.Core.Services.v1.Abstract;
using DDM.API.Infrastructure.Data.Application;
using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDM.API.Web.Controllers.v1
{
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;
        private readonly DDMDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthController(DDMDbContext context, UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            _context = context;
            _userManager = userManager;
            _authService = authService;
        }

        //[HttpPost, Route("Login")]
        [HttpPost("login")]
        public async Task<ActionResult<GenericResponseDto<object>>> Login(LoginRequestDto loginRequest)
        {
            var response = await _authService.LoginUser(loginRequest);
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        [HttpGet("current-profile")]
        [Authorize()]
        public async Task<ActionResult<GenericResponseDto<UserDto>>> CurrentProfile()
        {
            var httpContext = new HttpContextAccessor();
            var username = httpContext.HttpContext.User.Identity.Name;

            var response = await _authService.GetCurrentUserAsync(username);
            Response.StatusCode = response.StatusCode ?? StatusCodes.Status200OK;
            return new JsonResult(response);
        }

        //[HttpPost("change-password")]
        //[Authorize()]
        //public async Task<ActionResult<ApplicationUser>> ChangePassword(PasswordChangeDto passwordChange)
        //{
        //    var httpContext = new HttpContextAccessor();
        //    var username = httpContext.HttpContext.User.Identity.Name;

        //    var response = await _authService.PasswordChange(username);
        //    Response.StatusCode = response.StatusCode ?? StatusCodes.Status200OK;
        //    return new JsonResult(response);

        //}
    }
}
