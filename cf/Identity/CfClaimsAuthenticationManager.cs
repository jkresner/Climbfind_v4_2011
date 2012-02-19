using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Claims;

namespace cf.Identity
{
    public class CfClaimsAuthenticationManager : ClaimsAuthenticationManager
    {
        public override IClaimsPrincipal Authenticate(string resourceName, IClaimsPrincipal incomingPrincipal)
        {
            IClaimsIdentity currentIdentiy = incomingPrincipal.Identity as IClaimsIdentity;

            if (!incomingPrincipal.Identity.IsAuthenticated)
            {
                if (new cf.Identity.CfIdentityInflater().TryGetClaimsIdentity(out currentIdentiy))
                {
                    incomingPrincipal.Identities[0] = currentIdentiy;
                }
            }

            return incomingPrincipal;
        }
    }
}
