using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Runtime.Serialization;
using Microsoft.IdentityModel.Claims;
using System.Web;

namespace cf.Identity
{
	public static class CfIdentity
	{
		private static IPrincipal _user;
		public static IPrincipal User { get { 
			if (HttpContext.Current != null)
			{
				return HttpContext.Current.User;
			}
			else
			{
				return _user;
			}
		} }

		static CfIdentity() { }

		/// <summary>
		/// Inject in a different Pricipal (useful for testing)
		/// </summary>
		/// <param name="user"></param>
		public static void Inject(IPrincipal user)
		{
			_user = user;
		}

		/// <summary>
		/// Name is like 'Username' provided by the authentication system
		/// </summary>
		public static string DisplayName { get { return GetCfClaimValue("displayname"); } }
		public static string FullName { get { return GetCfClaimValue("fullname"); } }
		public static string FacebookID { get { return GetCfClaimValue("facebookid"); } }
		public static string Email { get { return GetWSTrustClaimValue("name"); } } //-- note the claim name = users email
		public static Guid UserID { get { if (IsAuthenticated) { return Guid.Parse(GetCfClaimValue("userid")); } else { return Guid.Empty; } } }

		public static bool IsAuthenticated { get { if (Current == null) { return false; } else { return Current.IsAuthenticated; } } }
		public static IClaimsIdentity Current { get { return User.Identity as ClaimsIdentity; } }
		
		private static string GetCfClaimValue(string claimType) { return GetClaimValue(CfClaims.BaseUri, claimType); }
		private static string GetWSTrustClaimValue(string claimType) { return GetClaimValue(CfClaims.BaseSoapUri, claimType); }
		private static string GetClaimValue(string baseUri, string claimType)
		{
			if (!Current.IsAuthenticated) { return "notauthenticated"; }

			if (Current.Claims == null)
			{
				throw new ArgumentNullException("identity is null");
			}

			if (Current.Claims.Count == 0)
			{
				throw new ArgumentException("No claims on idenity", "identity");
			}

			return Current.Claims.SingleOrDefault(c => c.ClaimType == baseUri + claimType).Value;
		}
	}
}
