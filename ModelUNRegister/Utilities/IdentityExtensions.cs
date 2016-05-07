using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace ModelUNRegister.Utilities
{
    public static class IdentityExtensions
    {
        public static string GetActualName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("ActualName");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static bool IsAdministrator(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("IsAdministrator");
            return (claim != null) ? (claim.Value == "true") : false;
        }
    }
}