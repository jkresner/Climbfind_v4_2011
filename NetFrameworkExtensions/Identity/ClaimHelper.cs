using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace NetFrameworkExtensions.Identity
{
    /// <summary>
    /// Helps us to easily request claim values from our current Identity, which should implement IClaimsIdentity
    /// </summary>
    public static class ClaimHelper
    {
        /// <summary>
        /// Get single claim from the given IIdenity
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public static Claim GetClaimFromIdentity(IIdentity identity, string claimType)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            var claimsIdentity = identity as IClaimsIdentity;

            if (claimsIdentity == null)
            {
                throw new ArgumentException("Cannot convert identity to IClaimsIdentity", "identity");
            }

            return claimsIdentity.Claims.SingleOrDefault(c => c.ClaimType == claimType);
        }

        /// <summary>
        /// Get a single claim from our current Principal
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public static Claim GetClaimsFromPrincipal(IPrincipal principal, string claimType)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            var claimsPrincipal = principal as IClaimsPrincipal;

            if (claimsPrincipal == null)
            {
                throw new ArgumentException("Cannot convert principal to IClaimsPrincipal.", "principal");
            }

            return GetClaimFromIdentity(claimsPrincipal.Identities[0], claimType);
        }

        /// <summary>
        /// Get the claim off the current Thread....
        /// </summary>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public static Claim GetCurrentUserClaim(string claimType)
        {
            return GetClaimsFromPrincipal(HttpContext.Current.User, claimType);
        }

        /// <summary>
        /// Gets only the value of a specific claim from the current user user
        /// </summary>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public static string GetCurrentUserClaimValue(string claimType)
        {
            Claim claim = GetCurrentUserClaim(claimType);
            if (claim != null)
            {
                return claim.Value;
            }
            return string.Empty;
        }
    }
}
