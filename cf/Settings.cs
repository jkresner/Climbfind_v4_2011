using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace cf
{
	/// <summary>
	/// This class is purely for strongly type shorthand clear concise access to settings across the whole web application
	/// </summary>
	/// <example>
	/// Usage in a view '@Stgs.StaticRoot
	/// </example>
	public static class Stgs
	{
		public static readonly string WebRt;
		public static readonly string UploadRt;
		public static readonly string StaticRt;
		public static readonly string ImgsRt;
		public static readonly string SvcRt;
		public static readonly string MapSvcRelativeUrl;
		public static readonly string SearchSvcRelativeUrl;
		public static readonly string AWSAccessKey;
		public static readonly string AWSSecretKey;
		public static readonly string DefaultMapInfoImage;
		public static readonly Guid JskID = new Guid("a9646cc3-18cb-4a62-8402-5263ba8b3476");
		public static readonly Guid SystemID = new Guid("aaaaaaaa-18cb-4a62-8402-5263ba8b3476");
		public static readonly Guid MyFeedID = new Guid("00000000-0000-0000-0000-000000000002");

		public static readonly TimeZoneInfo SiteTimeZone;

		public static byte ModRepIncrementAddPlace = 10;
		public static byte ModRepIncrementAddPoi = 4;
		public static byte ModRepIncrementAddClimb = 3;
		public static byte ModRepDeletePlace = 0;
		public static byte ModRepEdit = 0;
		public static byte ModRepVerifyEdit = 1;
		public static byte ModRepEditVerified = 3;
		public static byte ModRepSetImage = 0;
		public static byte ModRepVerifyImage = 1;
		public static byte ModRepImageVerified = 2;
		public static byte ModRepDownVote = 15;

		public static bool IsDevelopmentEnvironment { get { return 
			!Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment.IsAvailable ||
			ConfigurationManager.AppSettings["MailServer"] != "smtp.climbfind.com"; } }

		public static string DbConnectionString { get; set; }

		public static MailAddress MailMan { get; set; }
		public static SmtpClient MailServer { get; set; }

		public static string DebugMailDir { get; set; }

		public static NetworkCredential CFUrbanairshipCredentials;
		public static NetworkCredential PGUrbanairshipCredentials;

		static Stgs()
		{
			WebRt = ConfigurationManager.AppSettings["WebRoot"];
			UploadRt = ConfigurationManager.AppSettings["UploadRoot"];           
			StaticRt = ConfigurationManager.AppSettings["StaticRoot"];
			ImgsRt = ConfigurationManager.AppSettings["ImagesRoot"];
			SvcRt = ConfigurationManager.AppSettings["SvcRoot"];
			MapSvcRelativeUrl = ConfigurationManager.AppSettings["MapSvcRelativeUrl"];
			SearchSvcRelativeUrl = ConfigurationManager.AppSettings["SearchSvcRelativeUrl"];
			AWSAccessKey = ConfigurationManager.AppSettings["AWSAccessKey"];
			AWSSecretKey = ConfigurationManager.AppSettings["AWSSecretKey"];

			DefaultMapInfoImage = string.Format("{0}/map/defaultInfo.png", StaticRt);

			MailMan = new MailAddress(
				ConfigurationManager.AppSettings["MailManAddress"],
				ConfigurationManager.AppSettings["MailManName"]);

			MailServer = new SmtpClient(ConfigurationManager.AppSettings["MailServer"]);
			MailServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailManAddress"], 
				ConfigurationManager.AppSettings["MailPassword"]);

			var timeZoneString = ConfigurationManager.AppSettings["SiteTimeZone"];
			if (!string.IsNullOrWhiteSpace(timeZoneString)) { SiteTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneString); }

			var efConnection = new System.Data.EntityClient.EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["cfEntitiesData"].ConnectionString);
			DbConnectionString = efConnection.ProviderConnectionString;

			var cfUrbanCreds = ConfigurationManager.AppSettings["CFUrbanairshipCredentials"];
			if (cfUrbanCreds != null)
			{
				CFUrbanairshipCredentials = new NetworkCredential(cfUrbanCreds.Split(',')[0], cfUrbanCreds.Split(',')[1]);
				var pgUrbanCreds = ConfigurationManager.AppSettings["PGUrbanairshipCredentials"];
				PGUrbanairshipCredentials = new NetworkCredential(pgUrbanCreds.Split(',')[0], pgUrbanCreds.Split(',')[1]);
			}

			if (IsDevelopmentEnvironment)
			{
				DebugMailDir = AppDomain.CurrentDomain.BaseDirectory
							.Replace("\\bin\\Debug", "")
							.Replace("\\bin\\Release", "")
							+ @"\..\mailtemplates\";
			}
		}

		public const string BlankPNG = "http://static.climbfind.com/ui/blank.png";
	}
}