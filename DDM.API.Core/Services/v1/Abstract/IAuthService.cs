using DDM.API.Core.DTOs.v1.Authentication;
using DDM.API.Infrastructure.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.Services.v1.Abstract
{
    public interface IAuthService
    {
        Task<GenericResponseDto<object>> LoginUser(LoginRequestDto request);
        //Task<GenericResponseDto<UserDto>> CreateUserAsync(RegistrationRequestDto requestDto);
        Task<GenericResponseDto<UserDto>> GetCurrentUserAsync(string UserName);
        //Task<GenericResponseDto<object>> PasswordChange(PasswordChangeDto request);
    }
}
