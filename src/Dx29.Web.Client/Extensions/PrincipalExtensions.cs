using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;

namespace Dx29.Web
{
    static public class PrincipalExtensions
    {
        static public IList<string> GetRoles(this ClaimsPrincipal principal)
        {
            return principal.Claims.Where(r => r.Type == "role").OrderBy(r => r.Value).Select(r => r.Value).ToArray();
        }
    }
}
