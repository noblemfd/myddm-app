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

namespace DDM.API.Core.Services.v1.Concrete
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly DDMDbContext _context;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration configuration,
            IMapper mapper,
            DDMDbContext context

        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            _context = context;
        }

        public async Task<GenericResponseDto<object>> LoginUser(LoginRequestDto request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            var response = new GenericResponseDto<object>();

            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
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
                    expires = token.ValidTo
                };
                response.StatusCode = 200;
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

        public async Task<GenericResponseDto<UserDto>> GetCurrentUserAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var response = new GenericResponseDto<UserDto>();
            if (user != null)
            {
                response.Result = _mapper.Map<UserDto>(user);
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
