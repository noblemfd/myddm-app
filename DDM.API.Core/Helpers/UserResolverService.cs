using DDM.API.Infrastructure.Data.Identiity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

    namespace DDM.API.Core.Helpers
    {
        public class UserResolverService
        {
            private readonly IHttpContextAccessor _httpContext;
            private readonly UserManager<ApplicationUser> _userManager;

            public UserResolverService(IHttpContextAccessor httpContext, UserManager<ApplicationUser> userManager)
            {
                _httpContext = httpContext;
                _userManager = userManager;
            }
            public string GetUserId()
            {
                var userId = _userManager.GetUserId(_httpContext.HttpContext.User);
                return userId;
                ////var userId = _userManager.GetUserId(_httpContext.HttpContext.User);
                //var user = _httpContext?.HttpContext?.User as ClaimsPrincipal;
                //var userId = user.Claims.ElementAt(0).Value;
                //return userId;
            }

            public string GetUserName()
            {
                var userName = _userManager.GetUserName(_httpContext.HttpContext.User);
                return userName;
            }

            ////Also, if you don't want to user the UserManager, you can simply make the UserResolverService class like this:
            //private readonly IHttpContextAccessor _httpContext;

            //public UserResolverService(IHttpContextAccessor httpContext)
            //{
            //    _httpContext = httpContext;
            //}
            //public string GetUserId()
            //{
            //    var user = _httpContext?.HttpContext?.User as ClaimsPrincipal;
            //    var userId = user.Claims.ElementAt(0).Value;
            //    return userId;
            //}
        }
    }
