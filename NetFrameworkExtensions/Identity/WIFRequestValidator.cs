using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Util;
using System.Web;
using Microsoft.IdentityModel.Protocols.WSFederation;

namespace NetFrameworkExtensions.Identity
{
    /// <summary>
    /// When the STS posts a security token with special characters to our relying party we application the default asp .net request validator
    /// throws an exception because the request is perceived as a security threat
    /// </summary>
    public class WIFRequestValidator : RequestValidator
    {
        /// <summary>
        /// Checks to see if the request is from our STS server and if so allows and request to pass validation
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <param name="requestValidationSource"></param>
        /// <param name="collectionKey"></param>
        /// <param name="validationFailureIndex"></param>
        /// <returns></returns>
        protected override bool IsValidRequestString(HttpContext context,
            string value, RequestValidationSource requestValidationSource,
            string collectionKey, out int validationFailureIndex)
        {
            validationFailureIndex = 0;
            
            if (requestValidationSource == RequestValidationSource.Form && 
                collectionKey.Equals(WSFederationConstants.Parameters.Result, StringComparison.Ordinal))
            {
                SignInResponseMessage message = WSFederationMessage.CreateFromFormPost(context.Request) as SignInResponseMessage;
                if (message != null)
                    return true;
            }
            
            return base.IsValidRequestString(context, value, requestValidationSource, collectionKey, out validationFailureIndex);
        }
    }
}
