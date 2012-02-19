using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Claims;

namespace cf.Identity
{
    public class CfClaimsAuthorizationManager : ClaimsAuthorizationManager
    {
        // use this method to check requests/operations against your authorization policy
        public override bool CheckAccess(AuthorizationContext context)
        {
            return base.CheckAccess(context);
        }
    }
}
