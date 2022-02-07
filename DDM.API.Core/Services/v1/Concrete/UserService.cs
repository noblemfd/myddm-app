using AutoMapper;
using DDM.API.Core.Helpers;
using DDM.API.Core.Services.v1.Abstract;
using DDM.API.Infrastructure.Data.Application;
using DDM.API.Infrastructure.Data.Identiity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.Services.v1.Concrete
{
    public class UserService : IUserService
    {
        private readonly DDMDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserResolverService _userResolverService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly PasswordHasher<ApplicationUser> _passwordHasher;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public UserService(DDMDbContext context, IMapper mapper, UserResolverService userResolverService, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _mapper = mapper;
            _userResolverService = userResolverService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            //_passwordHasher = passwordHasher;
            _roleManager = roleManager;
        }
        //public async Task<long> GetCurrentUserId()
        //{
        //    //var userName = _userResolverService.GetUserName();
        //    var userName = await this.GetCurrentUserName();

        //    var currentUserId = _context
        //        .ApplicationUser
        //        .FirstOrDefault(x => x.UserName == userName)
        //        .Id;
        //    return currentUserId;
        //}
        public Task<ApplicationUser> GetUserById(long id)
        {
            var user = _context.Users
                .SingleOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }
        //public Task<string> GetRoleNameById(long roleId)
        //{
        //    var resourceRoleName = _context.Roles
        //        .FirstOrDefault(r => r.Id == roleId)
        //        .Name;

        //    return Task.FromResult(resourceRoleName);
        //}
        //public async Task<IEnumerable<string>> GetUserRoles()
        //{
        //    var user = await this._userManager.FindByEmailAsync(await this.GetCurrentUserName());
        //    var userRoles = await _userManager.GetRolesAsync(user);

        //    return userRoles;
        //}
        //private Task<string> GetCurrentUserName()
        //{
        //    var currentUser = _httpContextAccessor
        //        .HttpContext
        //        .User
        //        .Identities
        //        .FirstOrDefault()
        //        .Name;

        //    return Task.FromResult(currentUser);
        //}
    }
}
