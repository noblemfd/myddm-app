using DDM.API.Infrastructure.Data.Identiity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DDM.API.Web.Helpers.Extensions
{
    public static class UserManagerExtension
    {
        //This will help to get the address associated with email instead of injecting appIdentityContext
        public static async Task<ApplicationUser> FindUserByClaimsPrincipleWithMerchantAsync(this UserManager<ApplicationUser> input, ClaimsPrincipal user)
        {
            var username = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            //return await input.Users.Include(x => x.Address).SingleOrDefaultAsync(x => x.Email == email);
            return await input.Users.SingleOrDefaultAsync(x => x.UserName == username);
        }

        public static async Task<ApplicationUser> FindByEmailUsernameClaimsPrinciple(this UserManager<ApplicationUser> input, ClaimsPrincipal user)
        {
            var username = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            return await input.Users.SingleOrDefaultAsync(x => x.UserName == username);
        }
    }
}
