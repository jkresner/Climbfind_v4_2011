using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens;
using System.Collections.Specialized;
using Microsoft.IdentityModel.Claims;
using System.Collections.ObjectModel;
using NetFrameworkExtensions;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace NetFrameworkExtensions.Identity
{
    /// <summary>
    /// Parses an SWT token. See http://groups.google.com/group/oauth-wrap-wg.
    /// </summary>
    public class SimpleWebToken : SecurityToken
    {
        public const string OasisTokenProfile = "http://schemas.xmlsoap.org/ws/2009/11/swt-token-profile-1.0";

        private DateTime validFrom = DateTime.UtcNow;

        public string Audience { get; private set; }
        public NameValueCollection Claims { get; private set; }
        public DateTime ExpiresOn { get; private set; }
        public string Issuer { get; private set; }
        public string RawToken { get; private set; }

        public SimpleWebToken(string rawToken)
        {
            this.RawToken = rawToken;
            this.Parse();
        }

        public bool IsExpired
        {
            get
            {
                var expiresOn = this.ExpiresOn.ToEpochTime();
                var currentTime = DateTime.UtcNow.ToEpochTime();

                return currentTime > expiresOn;
            }
        }

        /// <summary>
        /// Converts the SimpleWebToken to a ClaimsIdentity
        /// </summary>
        public ClaimsIdentity ToClaimsIdentity(string nameClaimType = null, string roleClaimType = null)
        {
            var claims = this.Claims
                .AllKeys
                .SelectMany(key => this.Claims.GetValues(key)
                    .Select(value => new { Key = key, Value = value }))
                .Select(keyValue => new Claim(keyValue.Key, keyValue.Value));

            return new ClaimsIdentity(claims, "OAUTH-SWT",
                nameClaimType ?? ClaimTypes.Name, roleClaimType ?? ClaimTypes.Role);
        }

        public override string ToString()
        {
            return this.RawToken;
        }

        private void Parse()
        {
            this.Claims = new NameValueCollection();

            if (RawToken.StartsWith("wrap_access_token=")) { RawToken = RawToken.Replace("wrap_access_token=", ""); }

            foreach (var rawNameValue in RawToken.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (rawNameValue.StartsWith("HMACSHA256="))
                    continue;

                var nameValue = rawNameValue.Split('=');

                if (nameValue.Length != 2)
                    throw new InvalidSecurityTokenException(string.Format(
                        "Invalid token contains a name/value pair missing an = character: '{0}'", rawNameValue));

                var key = HttpUtility.UrlDecode(nameValue[0]);

                if (this.Claims.AllKeys.Contains(key))
                    throw new InvalidSecurityTokenException("Duplicated name token.");

                var values = HttpUtility.UrlDecode(nameValue[1]);

                switch (key)
                {
                    case Saml2Constants.Elements.Audience:
                        this.Audience = values;
                        break;
                    case "ExpiresOn":
                        this.ExpiresOn = ulong.Parse(values).ToDateTimeFromEpoch();
                        break;
                    case Saml2Constants.Elements.Issuer:
                        this.Issuer = values;
                        break;
                    default:
                        foreach (var value in values.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            this.Claims.Add(key, value);
                        }
                        break;
                }
            }
        }

        /* SecurityToken */
        public override DateTime ValidFrom { get { return this.validFrom; } }
        public override DateTime ValidTo { get { return this.ExpiresOn; } }
        public override string Id { get { throw new NotImplementedException(); } }
        public override ReadOnlyCollection<SecurityKey> SecurityKeys { get { return new List<SecurityKey>().AsReadOnly(); } }
    }
}
