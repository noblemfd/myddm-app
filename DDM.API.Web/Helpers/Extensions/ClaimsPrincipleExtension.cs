using System.Linq;
using System.Security.Claims;

namespace DDM.API.Web.Helpers.Extensions
{
    public static class ClaimsPrincipleExtension
    {
        public static string RetrieveUsernameFromPrincipal(this ClaimsPrincipal user)
        {
            return user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        }
    }
}
