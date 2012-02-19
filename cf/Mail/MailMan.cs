using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Instrumentation;
using System.Web;
using NetFrameworkExtensions.Web;
using cf.Entities;
using System.Net.Mail;
using cf.Entities.Interfaces;
using cf.Dtos;

namespace cf.Mail
{
	public static partial class MailMan
	{
		static MailMan() { }

		public static void SendAppException(Exception ex)
		{
			var mailBody = string.Empty;
			var machine = Environment.MachineName;
			HttpRequest currentRequest = null;

			//-- We have to try assign this inside try/catch because using the Request
			//-- get Accessor throws an exception if the Request is not available
			try { currentRequest = HttpContext.Current.Request; }
			catch { }

			if (currentRequest == null)
			{
				mailBody = string.Format("Occurred: {0}<br />OnMachine: {1}<br />No HttpContext... <br /><b>Exception:</b><br/><div style='font-size:11px'>{2}</div><br />",
					DateTime.Now, machine, ex.ToString());
			}
			else //-- If the request is available we can do some more detailed stuff
			{
				var browser = HttpContext.Current.Request.Browser;
				var user = HttpContext.Current.User.Identity.Name;
				if (browser.Crawler) { user = "Crawler[" + browser.Type + "]"; }
				else if (cf.Identity.CfIdentity.IsAuthenticated) { user += "[" + cf.Identity.CfIdentity.UserID + "]"; }

				var host = HttpContext.Current.Request.UserHostAddress;
				var url = currentRequest.Url.ToString();
				var refer = (currentRequest.UrlReferrer == null) ? "none" : currentRequest.UrlReferrer.ToString();
				var httpMethod = currentRequest.HttpMethod;
				var httpVars = currentRequest.GetRequestQueryAndFormNameValuePairString();
				
				var sessionID = (((HttpContext.Current != null) && (HttpContext.Current.Session != null)) ? HttpContext.Current.Session.SessionID : string.Empty);

				const string emailBodyFormat = @"User: {0}<br />
												RequestUrl: {1}<br />
												ReferUrl: {2}<br />
												HttpMethod: {3}<br />
												HttpVars: {4}<br />
												Browser: {5}<br />                                                
												Occurred: {6}<br />
												Host: {7}<br />                                                												
												Machine: {8}<br />
												RequestID: {9}<br />
												SessionID: {10}<br />
												Message: {11}<br />
												<b>ExceptionStackTrace:</b><br/><div style='font-size:11px'>{12}</div><br />";

				var requestID = new Guid();

				mailBody = string.Format(emailBodyFormat, user, url, refer, httpMethod, httpVars, browser.Type, DateTime.Now, host, machine, requestID, sessionID, ex.Message, ex.StackTrace);
			}
			
			string subject = string.Format("[{0}] {1} {2}", CfTrace.Current.Name, ex.GetType(), DateTime.Now);
			var subscribers = GetEventTypeSubsribers(TraceCode.Exception);

			try
			{
				SMTP.SendAppEvent(subject, mailBody, subscribers);
			}
			catch (Exception failedSendEx)
			{
				string msg = string.Format("ExceptionEmailAndLoggingTraceListener failed to send email : {0}. Check configuration.", mailBody);

				throw new Exception(failedSendEx.Message);
			}
		}

		public static void SendAppEvent(TraceCode type, string eventDescription, string linkedToUsersEmail, Guid linkedToUserID, string receiverEmail, bool replyToUser)
		{
			string subject = string.Format("[{0}] {1} {2}", CfTrace.Current.Name, type, DateTime.Now);
			string body = string.Format("User: <a href='http://www.climbfind.com/climber/{2}'>{0}</a><br /><br />{1}", linkedToUsersEmail, eventDescription, linkedToUserID);

			var subscribers = GetEventTypeSubsribers(TraceCode.Exception);

			if (replyToUser == false) { SMTP.SendAppEvent(subject, body, subscribers); }
			else { SMTP.SendAppEvent(subject, body, subscribers, linkedToUsersEmail); }
		}
				
		private static string[] GetEventTypeSubsribers(TraceCode traceCode)
		{
			//-- todo replace this!!!
			return new string[] { "jkresner@yahoo.com.au" };
		}


		public static void SendUserMessageEmail(IUserBasicDetail to, IUserBasicDetail from, string messageContent)
		{
			string toEmail = to.Email;

			SMTP.PostSingleMail(new cfEmail(
					string.Format("{0} sent you a message", from.FullName),
					HtmlBodyGenerator.GetMessageBody(from.ID, "climber/" + from.ID, from.FullName, from.Avatar, messageContent),
					new MailAddress(toEmail, to.FullName), Stgs.MailMan));
		}

		public static void SendCommentEmail(IUserBasicDetail to, IUserBasicDetail by, Guid postID, string commentMessage)
		{
			string toEmail = to.Email;

			SMTP.PostSingleMail(new cfEmail(
					string.Format("{0} made a comment", by.FullName),
					HtmlBodyGenerator.GetCommentBody(postID, by.ID, "climber/" + by.ID, by.FullName, by.Avatar, commentMessage),
					new MailAddress(toEmail, to.FullName), Stgs.MailMan));
		}

		public static void SendPartnerCallEmail(IUserBasicDetail to, IUserBasicDetail by, CfCacheIndexEntry place,
			PartnerCall pc, string matchingSubscriptionPlaces)
		{
			string toEmail = to.Email;

			SMTP.PostSingleMail(new cfEmail(
					string.Format("{0}'s PartnerCall for {1}", by.FullName, place.Name),
					HtmlBodyGenerator.GetPartnerCallBody(pc.ID, by.ID, "climber/" + by.ID, by.FullName, 
						place.SlugUrl, place.Name, pc.StartDateTime, by.Avatar, pc.Comment, matchingSubscriptionPlaces),
					new MailAddress(toEmail, to.FullName), Stgs.MailMan));
		}

		public static void SendPasswordResetEmail(IUserBasicDetail to, string newPass)
		{
			string toEmail = to.Email;

			SMTP.PostSingleMail(new cfEmail(
					"Your Climbfind password has been reset",
					"Your new password is: " + newPass + "<br /><br />Once logged in your can change your password at https://accounts.climbfind.com/account/changepassword",
					new MailAddress(toEmail, to.FullName), Stgs.MailMan));
		}
	}
}