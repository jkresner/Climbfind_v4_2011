using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Text;
using cf.Entities;
using cf.DataAccess.Repositories;
using cf.Web.Models;
using NetFrameworkExtensions.Web.Mvc;
using cf.Web.Mvc.ActionFilters;
using cf.Caching;
using NetFrameworkExtensions.Net;
using NetFrameworkExtensions.Drawing;
using cf.Services;
using cf.Instrumentation;
using System.Web.Security;
using Microsoft.IdentityModel.Web;
using Microsoft.IdentityModel.Protocols.WSFederation;
using cf.Identity;
using cf.Content.Search;
using System.Drawing;
using cf.Mail;

namespace cf.Web.Controllers
{
	public class UtilityController : Controller
	{
		/// <summary>
		/// So JSK doesn't get error emails from stupid bots and people are confused when they try to execute the url with GET
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		[HttpGet]
		public ActionResult RequestProxy()
		{
			return new HttpStatusCodeWithBodyResult("PlaceNotFound", 404);
		}

		/// <summary>
		/// Useful for javascript ajax requests that need to go to domains outside the application
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult RequestProxy(string url)  
		{
			try
			{
				return new ProxyResult(new Uri(url));
			}
			catch (Exception ex)
			{
				CfTrace.Error(ex);
				return Json(new { Success = false, Error = ex.Message });
			}
		}

		private string SanitizeSearchTerm(string original)
		{
			string sanitized = original.Replace(".", "").Replace("<", "").Replace(">", "").Replace(":", "").Replace(";", "").Replace("*", "").Replace("!", "")
				.Replace("'", "").Replace(@"/", "").Replace(@"\", "").Replace(@"(", "").Replace(@")", "").Replace(@"[", "").Replace(@"]", "")
				.Replace("{", "").Replace("}", "");
			
			//-- protect from injection attacks
			if (sanitized.Length > 30) { sanitized = sanitized.Substring(0, 30); }

			//http://stackoverflow.com/questions/685193/a-word-that-do-so-the-asp-net-mvc-routing-crashes
			if (sanitized.ToLower() == "con") { sanitized = " con"; }
			
			return sanitized;
		}

		/// <summary>
		/// Our site wide search action method user for places AND users
		/// </summary>
		/// <param name="qsearch"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Search(string qsearch)
		{           
			if (!CfIdentity.Current.IsAuthenticated)
			{
				return Json(new List<SearchEngineResult>() { new SearchEngineResult("Login to use search", 10,
					"Search areas, crags, routes, climbs, indoor climbing & more", "/login")});
			}

			return new ProxyResult(new Uri(string.Format("{0}{1}term/{2}", Stgs.SvcRt, Stgs.SearchSvcRelativeUrl, SanitizeSearchTerm(qsearch))));
		}

		/// <summary>
		/// Our method for ONLY places
		/// </summary>
		/// <param name="psearch"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult SearchPlaces(string psearch)
		{
			return new ProxyResult(new Uri(string.Format("{0}{1}place/{2}", Stgs.SvcRt, Stgs.SearchSvcRelativeUrl, SanitizeSearchTerm(psearch))));
		}

		[HttpPost, CfAuthorize]
		public ViewResult SearchLocations(string lsearch)
		{
			return new ProxyResult(new Uri(string.Format("{0}{1}location/{2}", Stgs.SvcRt, Stgs.SearchSvcRelativeUrl, SanitizeSearchTerm(lsearch))));
		}

		[HttpPost, CfAuthorize]
		public ViewResult SearchProvinces(string psearch)
		{
			return new ProxyResult(new Uri(string.Format("{0}{1}province/{2}", Stgs.SvcRt, Stgs.SearchSvcRelativeUrl, SanitizeSearchTerm(psearch))));
		}

		[HttpPost, CfAuthorize]
		public ViewResult SearchClimbingAreas(string asearch)
		{
			return new ProxyResult(new Uri(string.Format("{0}{1}climbing-area/{2}", Stgs.SvcRt, Stgs.SearchSvcRelativeUrl, SanitizeSearchTerm(asearch))));
		} 

		/// <summary>
		/// Our site wide search action method
		/// </summary>
		/// <param name="qsearch"></param>
		/// <returns></returns>
		public ViewResult Unauthorized()
		{
			return View();
		}
		
		[CfAuthorize]
		public ActionResult LogOn()
		{
			return RedirectToAction("Index", "Home");
		}

		[CfAuthorize] public ActionResult Tests() { return View(); }
		[CfAuthorize] public ActionResult ThrowError() { throw new Exception("Testing " + DateTime.Now.ToString()); }
		[CfAuthorize] public ActionResult SendMessageEmail() 
		{
			//var toID = new Guid("a071283a-7625-4d73-9bf6-8e95d534e78c");
			//var msg = "This is a message to contact and test if the messaging emails are going through on " + DateTime.Now.ToString();

			//ViewBag.TestDetails = string.Format("<p></b>{0}</b></p><hr />{1}", Stgs.MailServer.Host,
			//    HtmlBodyGenerator.GetMessageBody(CfIdentity.UserID, "From", CfIdentity.DisplayName, "865b5f79-a3e.jpg", msg));

			//var conversation = new ConversationService().SendMessage(toID, msg);
			ViewBag.TestResult = "Must uncomment to run test";

			return View("Tests"); 
		}
		[CfAuthorize]
		public ActionResult SendHtmlTestEmail()
		{
			//var htmltest = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"App_Data\mailtemplates\test.htm");
			//var html = string.Format(htmltest, DateTime.Now);
			
			//SMTP.PostSingleMail(new cf.Mail.cfEmail("Message from Jonathon Kresner", html, new System.Net.Mail.MailAddress("contact@climbfind.com"), 
			//    Stgs.MailMan, Stgs.MailMan));

			//ViewBag.TestDetails = string.Format("<p></b>{0}</b></p><hr />{1}", Stgs.MailServer.Host, html);
			ViewBag.TestResult = "Must uncomment to run test";
			
			return View("Tests");
		}

		public ActionResult LogOff()
		{
			if (this.User.Identity.IsAuthenticated)
			{
				FederatedAuthentication.WSFederationAuthenticationModule.SignOut(false);

				string issuer = FederatedAuthentication.WSFederationAuthenticationModule.Issuer;
				var signOut = new SignOutRequestMessage(new Uri(issuer));
				return new RedirectResult(signOut.WriteQueryString());
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}

		/// <summary>
		/// Our site wide search action method
		/// </summary>
		/// <param name="qsearch"></param>
		/// <returns></returns>
		public ViewResult PageNotFound()
		{
			return new HttpStatusCodeWithBodyResult("PageNotFound", 404);
		}

		/// <summary>
		/// Our site wide search action method
		/// </summary>
		public JsonResult GetYouTubeData(string youTubeUrl)
		{
			string videoID = string.Empty;
			var videoUri = new Uri(youTubeUrl);

			if (youTubeUrl.StartsWith("http://www.youtube.com/") ||
				youTubeUrl.StartsWith("https://www.youtube.com/")) { videoID = HttpUtility.ParseQueryString(videoUri.Query).Get("v"); }
			else if (youTubeUrl.StartsWith("http://youtu.be/")) { videoID = youTubeUrl.Replace("http://youtu.be/",""); }
			else { throw new Exception("Not a valid you tube url"); }
			
			var youTubeData = new MediaService().GetYouTubeVideoData(videoID);

			return Json(youTubeData);
		}


		/// <summary>
		/// Our site wide search action method
		/// </summary>
		public JsonResult GetVimeoData(string vimeoUrl)
		{
			if (!vimeoUrl.StartsWith("http://www.vimeo.com/") && !vimeoUrl.StartsWith("http://vimeo.com/"))
			{
				throw new Exception("Not a valid vimeo url");
			}

			//http://www.vimeo.com/20278551?ab = /20278551
			var videoUriWithoutQuery = new Uri(vimeoUrl).AbsolutePath;

			var videoID = videoUriWithoutQuery.Replace("/","");

			var vimeoData = new MediaService().GetVimeoVideoData(videoID);

			return Json(vimeoData);
		}
	}
}
