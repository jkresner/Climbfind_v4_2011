using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using cf.Instrumentation;
using cf.Caching;
using cf.Content.Search;
using System.Web.Routing;
using cf.Identity;
using System.ServiceModel.Activation;
using cf.Caching.WazMemcached;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure;
using Lucene.Net.Store;
using System.Configuration;

namespace cf.Svc
{
	public class Global : System.Web.HttpApplication
	{
		/// <summary>
		/// Search Engine
		/// </summary>
		public static CfLuceneIndexSearcher SiteSearchEngine { get { return _siteSearchEngine; } } private static CfLuceneIndexSearcher _siteSearchEngine;

		protected void Application_Start(object sender, EventArgs e)
		{
			CfTrace.InitializeTraceSource(new Instrumentation.CfTraceSource("Cf.Svc"));
			CfTrace.Current.Information(TraceCode.AppStart, "Cf.Svc Started {0:MMM dd HH.mm.ss}", DateTime.Now);
			
			//-- Setup caching depending on if we're running in the cloud or not
			//InitializeEnvironmentCacheAndSearchProviders();
			
			RegisterRoutes();

			Microsoft.IdentityModel.Web.FederatedAuthentication.ServiceConfigurationCreated += new EventHandler
				<Microsoft.IdentityModel.Web.Configuration.ServiceConfigurationCreatedEventArgs>(RsaServerfarmSessionCookieTransform.OnServiceConfigurationCreated);

			CfTrace.Current.Information(TraceCode.AppStartEnd, "Cf.Svc Start ended {0:MMM dd HH.mm.ss}", DateTime.Now);
		}

		private static object _gate = new object();
		private static bool _initialized = false;
		private static DateTime? lastIndexRefresh = null;

		/// <summary>
		/// Had to move azure role initialization here
		/// See http://social.msdn.microsoft.com/Forums/en-US/windowsazuredevelopment/thread/10d042da-50b1-4930-b0c0-aff22e4144f9 
		/// and http://social.msdn.microsoft.com/Forums/en-US/windowsazuredevelopment/thread/ab6d56dc-154d-4aba-8bde-2b7f7df121c1/#89264b8c-7e25-455a-8fd6-20f547ab545b
		/// </summary>
		protected void Application_BeginRequest()
		{
			//-- Search index is updated every hour + time taken to build cache and index on the cache server
			var indexNeedsUpdating = lastIndexRefresh.HasValue && lastIndexRefresh > DateTime.UtcNow.AddMinutes(65);  
			
			if (_initialized && !indexNeedsUpdating) 
			{
				return;
			}

			lock (_gate)
			{
				if (!_initialized)
				{
					InitializeEnvironmentCacheAndSearchProviders();
					_initialized = true;
				}
				else if (indexNeedsUpdating)
				{
					_siteSearchEngine.Dispose();
					var connectionString = ConfigurationManager.ConnectionStrings["CfCloudStorage"].ConnectionString;
					var cloudStorage = CloudStorageAccount.Parse(connectionString);
					var dir = new Lucene.Net.Store.Azure.AzureDirectory(cloudStorage, "SearchCatalog", new RAMDirectory());
					_siteSearchEngine = new CfLuceneIndexSearcher(dir);
					Mail.MailMan.SendAppEvent(TraceCode.AppBuildSearchIndex, "Search index reloaded", "", Stgs.JskID, "jkresner@yahoo.com.au", false);
					lastIndexRefresh = DateTime.UtcNow;
				}
			}
		}

		private void InitializeEnvironmentCacheAndSearchProviders()
		{
			CfTrace.Current.Information(TraceCode.AppBuildSearchIndex, "Started {0:MMM dd HH.mm.ss}", DateTime.Now);
			
			if (!RoleEnvironment.IsAvailable)
			{
				CfCacheIndex.Initialize(); 
				CfPerfCache.Initialize();
				
				new SearchManager().BuildIndex(null);
				var directoryPath = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\";
				var dir = Lucene.Net.Store.FSDirectory.Open(new System.IO.DirectoryInfo(directoryPath));
				
				_siteSearchEngine = new CfLuceneIndexSearcher(dir);
			}
			else 
			{ 
				CfCacheIndex.Initialize(new Level2MemcachedCacheIndex()); 
				CfPerfCache.Initialize(new Level2MemcachedPerfCache());


				var connectionString = ConfigurationManager.ConnectionStrings["CfCloudStorage"].ConnectionString;
				var cloudStorage = CloudStorageAccount.Parse(connectionString);
				var dir = new Lucene.Net.Store.Azure.AzureDirectory(cloudStorage, "SearchCatalog", new RAMDirectory());
				_siteSearchEngine = new CfLuceneIndexSearcher(dir);

				lastIndexRefresh = DateTime.UtcNow;
			}

			CfTrace.Current.Information(TraceCode.AppBuildSearchIndex, "Ended {0:MMM dd HH.mm.ss}", DateTime.Now);
		}


		void Application_End(object sender, EventArgs e)
		{
			//_siteSearchEngine.Dispose();
			CfTrace.Current.Information(TraceCode.AppEnd, "Cf.Svc End {0:MM.DD.HH.mm.ss}", DateTime.Now);
		}

		void Application_Error(Object sender, EventArgs e)
		{
			try
			{
				string relativeUrl = HttpContext.Current.Request.RawUrl;
				Exception exception = Server.GetLastError().GetBaseException();

				if (exception.ShouldRecord(relativeUrl))
				{
					CfTrace.Error(exception);
					Response.Write(exception.ToString());
				}

				Server.ClearError();
			}
			catch (Exception ex)
			{
				//TODO: write to text file
				Response.Write("<span class='note'>GLOBAL ENDPOINT: PLEASE EMAIL jkresner@yahoo.com.au IMMEDIATELY IF YOU SEE THIS SCREEN!!!!!!!!!!!!</span>" + ex.ToString());
			}
		}

		private void RegisterRoutes()
		{
			RouteTable.Routes.Add(new ServiceRoute("v0/search", new WebServiceHostFactory(), typeof(v0.SearchSvc)));
			RouteTable.Routes.Add(new ServiceRoute("v0/mobile", new WebServiceHostFactory(), typeof(v0.MobileSvc)));
			RouteTable.Routes.Add(new ServiceRoute("v0/map", new WebServiceHostFactory(), typeof(v0.MapSvc)));

			RouteTable.Routes.Add(new ServiceRoute("v1/search", new WebServiceHostFactory(), typeof(v1.SearchSvc)));
			RouteTable.Routes.Add(new ServiceRoute("v1/mobile", new WebServiceHostFactory(), typeof(v1.MobileSvc)));
		}
	}
}