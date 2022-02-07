using DDM.API.Infrastructure.Data.Identiity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Core.Services.v1.Abstract
{
    public interface IUserService
    {
        //Task<string> GetRoleNameById(long roleId);
        ////Task<string> GetRoleNameByName(string roleName);
        //Task<long> GetCurrentUserId();
        Task<ApplicationUser> GetUserById(long id);
        //Task<IEnumerable<string>> GetUserRoles();
    }
}
