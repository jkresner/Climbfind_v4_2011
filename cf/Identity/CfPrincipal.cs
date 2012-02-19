using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using cf.Entities;
using Microsoft.IdentityModel.Claims;

namespace cf.Identity
{
    public class CfPrincipal
    {
        public IClaimsIdentity Identity { get { return System.Web.HttpContext.Current.User.Identity as IClaimsIdentity; } }

        public Guid UserID { get { return CfIdentity.UserID; } }
        public string FullName { get { return CfIdentity.FullName; } }
        public bool IsGodUser { get { return UserID == Stgs.JskID; } }
        public bool IsSystemUser { get { return UserID == Stgs.SystemID; } }
        public bool IsGodOrSystemUser { get { return IsGodUser || IsGodOrSystemUser; } }
        
        public static ModProfile ModDetails { get { return (ModProfile)System.Web.HttpContext.Current.Items["modProfile"]; } }
        public int Reputation { get { return ModDetails.Reputation; } }
        public int PlacesAdded { get { return ModDetails.PlacesAdded; } }
        public int ClimbsAdded { get { return ModDetails.ClimbsAdded; } }
        public int VerifiedEdits { get { return ModDetails.VerifiedEdits; } } 

        public List<string> Roles { get; set; }

        public static bool CurrentIsAuthenticated { get { return System.Web.HttpContext.Current.User.Identity.IsAuthenticated; } }
        public static ClaimsPrincipal Current { get { return System.Web.HttpContext.Current.User as ClaimsPrincipal; } }
        public static bool IsSeniorMod { get { return new CfPrincipal().IsInRole("ModAdmin,ModSenior"); } }
        public static bool IsMod { get { return new CfPrincipal().IsInRole("ModCommunity,ModAdmin,ModSenior"); } }

        public CfPrincipal()
        {
            Roles = new List<string>();
        }

        public bool IsInRole(string roles)
        {
            foreach (var role in roles.Split(','))
            {
                if (Current.IsInRole(role)) { return true; }
                if (ModDetails != null && ModDetails.Role == role) { return true; }
            }
            return false;
        }

        public static bool IsGod()
        {
            return CfIdentity.UserID == Stgs.JskID;
        }
        
        public static void AttachModProfile(ModProfile modProfile)
        {
            if (modProfile.ID != CfIdentity.UserID) { throw new ArgumentException("CfPrincipal InflateModDetails: modProfile.ID != ProfileID"); }
            System.Web.HttpContext.Current.Items["modProfile"] = modProfile;
        }

        public IClaimsPrincipal Copy() { throw new NotImplementedException(); }
        public ClaimsIdentityCollection Identities { get { throw new NotImplementedException(); } }
    }
}
