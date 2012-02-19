using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using cf.Dtos;
using cf.DataAccess.AdoNet2;
using System.Data.SqlClient;
using Microsoft.WindowsAzure.Diagnostics.Management;
using System.ServiceModel;
using cf.Caching.WazMemcached;
using cf.Instrumentation;
using cf.Mail;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using System.Configuration;

namespace cf.CacheServer
{
	public class WorkerRole : RoleEntryPoint
	{
		/// <summary>
		/// Cloud storage client
		/// </summary>
		private CloudStorageAccount CfCloudStorage;
		
		/// <summary>ServiceHost object for internal communication endpoints</summary>
		private ServiceHost serviceHost;

		private void StartServiceHost()
		{
			var refreshService = new RefreshService();
			refreshService.NotificationRecieved += new RefreshNotificationRecievedEventHandler(ServiceHost_RefreshNotificationRecieved);

			serviceHost = new ServiceHost(refreshService);

			this.serviceHost.Faulted += (sender, e) =>
			{
				Trace.TraceError("Service Host fault occured");
				serviceHost.Abort();
				Thread.Sleep(500);
				this.StartServiceHost();
			};

			try
			{
				var binding = new NetTcpBinding(SecurityMode.None, false);

				var refreshServiceHostEndPoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[CacheConstants.RefreshEndpointName];

				serviceHost.AddServiceEndpoint(typeof(IRefreshService), binding,
					String.Format(CacheConstants.RefreshEndpointAddressFormat, refreshServiceHostEndPoint.IPEndpoint));

				serviceHost.Open();
				Trace.TraceInformation("Service Host Opened");
			}
			catch (TimeoutException timeoutException) { Trace.TraceError("Service Host open failure, Time Out: " + timeoutException.Message); }
			catch (CommunicationException communicationException) { Trace.TraceError("Service Host open failure, Communication Error: " + communicationException.Message); }
			Trace.WriteLine("Service Host Started", "Information");
		}

		public delegate void RefreshNotificationRecievedEventHandler(object sender, RefreshMessage e);
		void ServiceHost_RefreshNotificationRecieved(object sender, RefreshMessage e)
		{
			MailMan.SendAppEvent(TraceCode.AppBuildCache, "Receive refresh request notification, refreshing cache server", "jkresner@yahoo.com.au", Guid.Empty, "jkresner@yahoo.com.au", false);
			PopuluteCfCacheIndex();
		}

		[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true, AddressFilterMode = AddressFilterMode.Any)]
		public class RefreshService : IRefreshService
		{
			public event RefreshNotificationRecievedEventHandler NotificationRecieved;

			public void RefreshCacheIndex(RefreshMessage sender) { this.OnNotificationRecieved(sender); }
			protected virtual void OnNotificationRecieved(RefreshMessage e)
			{
				RefreshNotificationRecievedEventHandler handler = NotificationRecieved;
				if (handler != null) { handler(this, e); } //-- Invokes the handler (in our actual worker role)
			}
		}

		void MemcachedServer_Crashed(object sender, EventArgs e)
		{
			MailMan.SendAppEvent(TraceCode.AppEnd, "Memcached server failed!! Recycling cf.CacheServer worker role", "jkresner@yahoo.com.au", Stgs.SystemID, "jkresner@yahoo.com.au", false);
			RoleEnvironment.RequestRecycle();
		}

		Process proc;
		public override bool OnStart()
		{
			CfTrace.InitializeTraceSource(new Instrumentation.CfTraceSource("Cf.CacheServer"));
			
			try
			{
				var connectionString = ConfigurationManager.ConnectionStrings["CfCloudStorage"].ConnectionString;
				CfCloudStorage = CloudStorageAccount.Parse(connectionString);

				//System.Diagnostics.Trace.WriteLine("cf.CacheServer Started: "+DateTime.UtcNow.ToString());
				var startTime = DateTime.UtcNow;
				var stopwatch = new Stopwatch();
				stopwatch.Start();            
			
				proc = WazMemcachedHelpers.StartMemcachedServer("Memcached", 640);
				proc.Exited += new EventHandler(MemcachedServer_Crashed);
				var index = PopuluteCfCacheIndex();

				stopwatch.Stop();

				//System.Diagnostics.Trace.WriteLine(string.Format("cf.CacheServer index [{0}], Ready after {1}ms", index.Count, stopwatch.ElapsedMilliseconds));

				StartServiceHost(); //Listen for manual cache refreshes
			}
			catch (Exception ex)
			{
				CfTrace.Error(ex);
			}

			return base.OnStart();
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Run()
		{
			while (true)
			{
				try
				{
					//var mClient = WazMemcachedHelpers.CreateProtobufferClient(CacheConstants.CacheRole, CacheConstants.CacheEndpoint);
					//ServerStats stats = mClient.Stats();
					//MailMan.SendAppEvent(TraceCode.AppBuildCache, "Hourly cache cache + lucene index", "jkresner@yahoo.com.au", Guid.Empty, "jkresner@yahoo.com.au", false);
					Thread.Sleep(1000 * 3600); //-- Sleep for an hour and then refresh the index
					Trace.WriteLine("CacheServer cache refresh", "Information");
					PopuluteCfCacheIndex();
				}
				catch (Exception ex)
				{
					CfTrace.Error(ex);
				}
			}
		}

		private List<CfCacheIndexEntry> PopuluteCfCacheIndex()
		{
			//MailMan.SendAppEvent(TraceCode.AppBuildCache, "Refreshing cache server index cache + lucene index", "jkresner@yahoo.com.au", Guid.Empty, "jkresner@yahoo.com.au", false);
			
			var client = WazMemcachedHelpers.CreateProtobufferClient("cf.CacheServer", "Memcached");
			var index = new cf.DataAccess.Repositories.CFCacheIndexEntryRepository().GetAll().ToList();
			client.FlushAll(); //-- do this after GetAll() to minimize the possibility of the cache being down...
			foreach (var entry in index) { AddIndexEntryToCache(entry, client); }

			//-- while we're here.. let's take care of lucene too
			RebuildLuceneIndex();

			return index;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="client"></param>
		private void AddIndexEntryToCache(CfCacheIndexEntry entry, MemcachedClient client)
		{
			var key = "ci" + entry.ID.ToString("N");
			client.Store(StoreMode.Set, key, entry);
		}

		/// <summary>
		/// 
		/// </summary>
		private void RebuildLuceneIndex()
		{
			try
			{
				var dir = new Lucene.Net.Store.Azure.AzureDirectory(CfCloudStorage, "SearchCatalog", new Lucene.Net.Store.RAMDirectory());
			
				//-- The magic code... delete all the blobs before rebuilding the index				
				foreach (var b in dir.BlobContainer.ListBlobs())
				{
					dir.BlobContainer.GetBlobReference(b.Uri.ToString()).Delete();
				}

				new cf.Content.Search.SearchManager().BuildIndex(dir);
			}
			catch (Exception ex)
			{
				CfTrace.Error(ex);
			}
		}
	}
}
