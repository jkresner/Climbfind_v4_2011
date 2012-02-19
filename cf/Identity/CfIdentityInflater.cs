using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Claims;
using System.Web;
using Microsoft.IdentityModel.Web;
using System.Xml;
using System.IO;
using NetFrameworkExtensions.Identity;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace cf.Identity
{
    public class CfIdentityInflater
    {
        /// <summary>
        /// Tries to retrieve the clients ClaimsIdentity from the current request context.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>True when a valid identity was found - otherwise false.</returns>
        public virtual bool TryGetClaimsIdentity(out IClaimsIdentity identity)
        {
            identity = null;

            // check header first - authorization and x-authorization
            var authZheader = HttpContext.Current.Request.Headers["cf-Authorization"];
            if (!string.IsNullOrEmpty(authZheader))
            {
                try
                {
                    if (authZheader.StartsWith("cfST="))
                    {
                        var tokenString = authZheader.Substring("cfST=".Length);
                        var samlToken = FederatedAuthentication.ServiceConfiguration.SecurityTokenHandlers.ReadToken(new XmlTextReader(new StringReader(tokenString)));
                        identity = FederatedAuthentication.ServiceConfiguration.SecurityTokenHandlers.ValidateToken(samlToken).First();
                    }
                    else
                    {
                        throw new AccessViolationException("Client usage of cf-Auth is invalid. This request has been logged & legal action will be taken if the client illegally access Climbfinds systems");
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public static X509Certificate2 cert = FederatedAuthentication.ServiceConfiguration.ServiceCertificate;
        public static System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();

        /// <summary>
        /// Tries to retrieve the clients ClaimsIdentity from the current request context.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>True when a valid identity was found - otherwise false.</returns>
        public virtual bool TryGetSwtClaimsIdentity(out IClaimsIdentity identity, out SimpleWebToken swttoken)
        {
            identity = null;

            // check header first - authorization and x-authorization
            var authZheader = HttpContext.Current.Request.Headers["cf-Authorization"];
            if (!string.IsNullOrEmpty(authZheader))
            {
                try
                {
                    if (authZheader.StartsWith("cfST="))
                    {
                        var encryptedBase64TokenString = authZheader.Substring("cfST=".Length);
                        var encryptedTokenBytes = Convert.FromBase64String(encryptedBase64TokenString);
                        var encryptedTokenString = enc.GetString(encryptedTokenBytes);
                        var tokenString = cf.Identity.DHDRSA.DecryptWithSymmetricAid((RSACryptoServiceProvider)cert.PrivateKey, encryptedTokenString);

                        tokenString = tokenString.Replace("_ws", "http%3a%2f%2fschemas.xmlsoap.org%2fws%2f2005%2f05%2fidentity%2fclaims%2f").
                            Replace("_ms", "http%3a%2f%2fschemas.microsoft.com%2fws%2f2008%2f06%2fidentity%2fclaims%2f").
                            Replace("_ma", "http%3a%2f%2fschemas.microsoft.com%2fws%2f2008%2f06%2fidentity%2fauthenticationmethod%2f").
                            Replace("_cf", "http%3a%2f%2fclimbfind.com%2fclaims%2f").
                            Replace("_ct", "http%3a%2f%2faccounts.climbfind.com%2ftrust");

                        swttoken = new SimpleWebToken(HttpUtility.UrlDecode(tokenString));
                        identity = swttoken.ToClaimsIdentity();
                    }
                    else
                    {
                        throw new AccessViolationException("Client usage of cf-Auth is invalid. This request has been logged & legal action will be taken if the client illegally access Climbfinds systems");
                    }

                    return true;
                }
                catch
                {
                    swttoken = null;
                    return false;
                }
            }
            else
            {
                swttoken = null;
            }

            return false;
        }
    }
}
