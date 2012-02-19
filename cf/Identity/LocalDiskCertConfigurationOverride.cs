//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.IdentityModel.Web;
//using Microsoft.IdentityModel.Tokens;

//namespace cf.Identity
//{
//    public class LocalDiskCertServiceConfigOverride
//    {
//        public const string certName = "";
//        //public const string certPass = "";
        
//        public static void AttachLocalDiskCert(object sender, Microsoft.IdentityModel.Web.Configuration.ServiceConfigurationCreatedEventArgs e)
//        {
//            //-- If we're in dev, ignore trusted certificate errros that cause exceptions
//            if (Environment.MachineName.ToLower() == "jonathon-pc")
//            {
//                System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
//                {
//                    return true;
//                };
//            }
            
//            var file = string.Format("{0}\\{1}\\{2}", System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "App_Data\\certificates", certName);
//            var x509cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(file, certPass);

//            var _configuration = e.ServiceConfiguration;
//            _configuration.ServiceCertificate = x509cert;

//            var certificates = new List<System.IdentityModel.Tokens.SecurityToken> { new System.IdentityModel.Tokens.X509SecurityToken(
//                    _configuration.ServiceCertificate) };

//            var encryptedSecurityTokenHandler =
//                    (from handler in _configuration.SecurityTokenHandlers
//                     where handler is Microsoft.IdentityModel.Tokens.EncryptedSecurityTokenHandler
//                     select handler).First() as Microsoft.IdentityModel.Tokens.EncryptedSecurityTokenHandler;

//            _configuration.ServiceTokenResolver = encryptedSecurityTokenHandler.Configuration.ServiceTokenResolver =
//                    System.IdentityModel.Selectors.SecurityTokenResolver.CreateDefaultSecurityTokenResolver(certificates.AsReadOnly(), false);


//            List<CookieTransform> sessionTransforms =
//                new List<CookieTransform>( new CookieTransform[]  {
//                    new DeflateCookieTransform(), 
//                    new RsaEncryptionCookieTransform(e.ServiceConfiguration.ServiceCertificate),
//                    new RsaSignatureCookieTransform(e.ServiceConfiguration.ServiceCertificate)  
//            });
            
//            SessionSecurityTokenHandler sessionHandler = new SessionSecurityTokenHandler(sessionTransforms.AsReadOnly());

//            e.ServiceConfiguration.SecurityTokenHandlers.AddOrReplace(sessionHandler);
//        }
//    }
//}
