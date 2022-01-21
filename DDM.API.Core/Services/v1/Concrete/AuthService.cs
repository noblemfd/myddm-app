using AutoMapper;
using DDM.API.Core.Services.v1.Abstract;
using DDM.API.Infrastructure.Data.Application;
using Microsoft.Extensions.Configuration;
using DDM.API.Infrastructure.Data.Identiity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDM.API.Core.DTOs.v1.Authentication;
using DDM.API.Infrastructure.Entities.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using DDM.API.Core.Helpers;
//using Microsoft.AspNetCore.Http;

namespace DDM.API.Core.Services.v1.Concrete
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly DDMDbContext _context;
        private readonly UserResolverService _userResolverService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration configuration,
            IMapper mapper,
            DDMDbContext context,
            UserResolverService userResolverService
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            _context = context;
            _userResolverService = userResolverService;
        }

        public async Task<GenericResponseDto<object>> LoginUser(LoginRequestDto request)
        {
            //var httpContext = new HttpContextAccessor();
            var user = await _userManager.FindByNameAsync(request.UserName);
            var role = await _userManager.GetRolesAsync(user);
            //var username = httpContext.HttpContext.User.Identity.Name;
            var response = new GenericResponseDto<object>();

            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                  //  new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                foreach (var userRole in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                response.Result = new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    user = _mapper.Map<UserDto>(user),
                    roles,
                    //username,
                    expires = token.ValidTo
                };
                response.StatusCode = 200;
                response.Message = "Successfully Logged In";
                user.LastLogin = DateTime.Now;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    response.Error = new ErrorResponseDto() { ErrorCode = 500, Message = ex.Message };
                }
                return response;
            }

            response.StatusCode = 400;
            response.Error = new ErrorResponseDto { ErrorCode = 400, Message = "Invalid Username or Password!" };

            return response;
        }
        public async Task<GenericResponseDto<object>> PasswordChange(PasswordChangeDto request)
        {
            var userName = _userResolverService.GetUserName();
            var user = await _userManager.FindByNameAsync(userName);
            var response = new GenericResponseDto<object>();
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    var error = string.Join<IdentityError>(", ", result.Errors.ToArray());
                    response.Error = new ErrorResponseDto { ErrorCode = 500, Message = "Failed to change password because of the following errors: " + error };
                }
                else
                {
                    response.StatusCode = 200;
                    response.Message = "Successfully Changed Password";
                    response.Result = _mapper.Map<UserDto>(user);
                }
            }
            else
            {
                response.Error = new ErrorResponseDto { ErrorCode = 400, Message = "This Username is not registered!" };
                response.StatusCode = 400;
            }
            return response;
        }
        public async Task<GenericResponseDto<object>> MustChangePassword(MustChangePasswordDto request)
        {
            var userName = _userResolverService.GetUserName();
            var user = await _userManager.FindByNameAsync(userName);
            var response = new GenericResponseDto<object>();
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    response.Error = new ErrorResponseDto() { ErrorCode = 500, Message = ex.Message };
                }
                if (!result.Succeeded)
                {
                    var error = string.Join<IdentityError>(", ", result.Errors.ToArray());
                    response.Error = new ErrorResponseDto { ErrorCode = 500, Message = "Failed to change password because of the following errors: " + error };
                }
                else
                {
                    response.StatusCode = 200;
                    response.Message = "Successfully Changed Password";
                    response.Result = _mapper.Map<UserDto>(user);
                }
            }
            else
            {
                response.Error = new ErrorResponseDto { ErrorCode = 400, Message = "This Username is not registered!" };
                response.StatusCode = 400;
            }
            return response;
        }
        //public async Task<GenericResponseDto<object>> MustChangePassword(MustChangePasswordDto request)
        //{
        //    var userName = _userResolverService.GetUserName();
        //    var user = await _userManager.FindByNameAsync(userName);
        //    var response = new GenericResponseDto<object>();
        //    if (user != null)
        //    {
        //        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        //        response.Result = new
        //        {
        //            user = _mapper.Map<UserDto>(user)
        //        };
        //        if (result.Succeeded)
        //        {
        //            response.StatusCode = 200;
        //            response.Message = "Successfully Changed Password";
        //            user.IsPasswordChanged = true;
        //        }
        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (Exception ex)
        //        {
        //            response.Error = new ErrorResponseDto() { ErrorCode = 500, Message = ex.Message };
        //        }
        //        return response;
        //    }
        //    else
        //    {
        //        response.Error = new ErrorResponseDto { ErrorCode = 400, Message = "This Username is not registered!" };
        //        response.StatusCode = 400;
        //    }
        //    return response;
        //}

        public async Task<GenericResponseDto<UserDto>> GetCurrentUserAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var response = new GenericResponseDto<UserDto>();
            if (user != null)
            {
                response.Result = _mapper.Map<UserDto>(user);
                response.Message = "Successfully Retrieved Current User";
                response.StatusCode = 200;
            }
            else
            {
                response.Error = new ErrorResponseDto() { ErrorCode = 401, Message = "You are not logged into the system!" };
                response.StatusCode = 401;
            }
            return response;
        }

        //public async Task<GenericResponseDto<object>> PasswordChange(PasswordChangeDto request)
        //{
        //    ApplicationUser user = await _userManager.FindByIdAsync(request.UserName);
        //    var response = new GenericResponseDto<object>();
        //    if (user == null)
        //    {
        //        response.Error = new ErrorResponseDto() { ErrorCode = 401, Message = "User not valid!" };
        //        response.StatusCode = 401;
        //    }
        //    bool yesFound = await _userManager.CheckPasswordAsync(user, request.OldPassword);
        //    if (!yesFound)
        //    {
        //        response.Error = new ErrorResponseDto() { ErrorCode = 401, Message = "Incorrect old Password!" };
        //        response.StatusCode = 401;
        //    }
        //    try
        //    {
        //        var RemoveResult = await _userManager.RemovePasswordAsync(user);
        //        if (RemoveResult.Succeeded)
        //        {
        //            var passwordChange = await _userManager.AddPasswordAsync(user, request.ConfirmNewPassword);
        //            if (passwordChange.Succeeded)
        //            {
        //                response.StatusCode = 200;;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Error = new ErrorResponseDto()
        //        {
        //            ErrorCode = 500,
        //            Message = ex.Message
        //        };
        //    }
        //    response.StatusCode = 400;
        //    response.Error = new ErrorResponseDto { ErrorCode = 400, Message = "Invalid Credentials!" };

        //    return response;
        //}
    }
}
