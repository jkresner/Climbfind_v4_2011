using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Web.Mvc.ActionFilters;
using cf.Services;
using cf.Identity;
using cf.Entities.Enum;

namespace cf.Web.Controllers
{
   
	public class HomeController : Controller
	{
		public ActionResult Index() {

			if (CfIdentity.IsAuthenticated)
			{               
				if (ControllerContext.HttpContext.Request["wreply"] != null)
				{
					var returnUrl = ControllerContext.HttpContext.Request["wreply"];
					if (returnUrl.StartsWith(Stgs.WebRt)) { return Redirect(returnUrl); }
				}

				return Redirect("/my-climbing-feed");
			}
			else
			{
				//ViewBag.Thumbs = new UserService().GetHomePageProfileThumbs();
				var result = new PostService().GetPostForEverywhere(PostType.Unknown, ClientAppType.CfWeb);
				ViewBag.Posts = result;
				return View(); 
			}
		}

		public ActionResult Bugs() { return View(); }
		public ActionResult Vision() { return View(); }
		public ActionResult ModeratorsProject() { return View(); }
		public ActionResult Glossary() { return View(); }
		public ActionResult CF5() { return View(); }
		
		public ActionResult About() { return View(); }
		public ActionResult AboutSearch() { return View(); }
		public ActionResult AboutQR() { return View(); }
		public ActionResult AboutOpinions() { return View(); }
		public ActionResult Mobile() { return View(); }
		public ActionResult MobileIPhoneHelp() { return View(); }
		public ActionResult ModeratorReputation() { return View(); }
		public ActionResult WorldRockClimbingDatabase() { return View(); }
		public ActionResult Press() { return View(); }
		public ActionResult Partners() { return View(); }
		public ActionResult Credits() { return View(); }
		public ActionResult Growth() { return View(); }
		public ActionResult Team() { return View(); }
		public ActionResult ClimbingPartners() { return View(); }
		public ActionResult ClimbingTravel() { return View(); }
		public ActionResult ClimbingIndoor() { return View(); }
		public ActionResult TermsOfService() { return View(); }
		public ActionResult PrivacyPolicy() { return View(); }
		
		[HttpGet]public ActionResult ResetPassword() { return View(); }
		[HttpPost]public ActionResult ResetPassword(string email) 
		{
			try
			{
				var objConfig = new System.Collections.Specialized.NameValueCollection();
				objConfig.Add("connectionStringName", "cfEntitiesData");
				objConfig.Add("enablePasswordRetrieval", "false");
				objConfig.Add("enablePasswordReset", "true");
				objConfig.Add("requiresQuestionAndAnswer", "false");
				objConfig.Add("applicationName", "/");
				objConfig.Add("requiresUniqueEmail", "true");
				objConfig.Add("maxInvalidPasswordAttempts", "500");
				objConfig.Add("passwordAttemptWindow", "10");
				objConfig.Add("commandTimeout", "30");
				objConfig.Add("passwordFormat", "Hashed");
				objConfig.Add("name", "AspNetSqlMembershipProvider");
				objConfig.Add("minRequiredPasswordLength", "8");
				objConfig.Add("minRequiredNonalphanumericCharacters", "2");
				//objConfig.Add("passwordStrengthRegularExpression", "(?=^.{8,25}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+}{\\":;'?/>.<,])(?!.*\\s).*$"));

				var objSqlMembershipProvider = new System.Web.Security.SqlMembershipProvider();
				objSqlMembershipProvider.Initialize(objConfig["name"], objConfig);
				var field = objSqlMembershipProvider.GetType().GetField("_sqlConnectionString", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
				field.SetValue(objSqlMembershipProvider, Stgs.DbConnectionString);

				var user = objSqlMembershipProvider.GetUser(email, false);
				if (user == null) { throw new Exception("No user found for "+email); }

				var newPass = objSqlMembershipProvider.ResetPassword(user.UserName, "Auto reset");
				var profile = new UserService().GetProfileByID((Guid)user.ProviderUserKey);
				cf.Mail.MailMan.SendPasswordResetEmail(profile, newPass);

				return Json( new { Success = true } );
			}
			catch (Exception ex)
			{
				return Json( new { Success = false, Error = ex.Message } );
			}
		}

		public ActionResult AboutTheUpgrade() { return View(); }        
		
		public ActionResult WSFedReceive() 
		{ 
			var isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
			return Content("Received valid token: " + isAuthenticated.ToString()); 
		}
	}
}
