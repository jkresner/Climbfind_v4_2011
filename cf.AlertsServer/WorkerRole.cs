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
using cf.Instrumentation;
using cf.Caching;
using cf.Caching.WazMemcached;

namespace cf.AlertsServer
{
	/// <summary>
	/// Hold a reference to the most recent message for debugging exceptions
	/// </summary>
	public static class LastQueueMessage { public static string MessageAsString { get; set; } }


	public class WorkerRole : RoleEntryPoint
	{        
		public override bool OnStart()
		{
			//-- Wait for the cf.CacheServer to load first
			System.Threading.Thread.Sleep(100000);

			try
			{
				CfTrace.InitializeTraceSource(new Instrumentation.CfTraceSource("Cf.AlertsServer"));

				CfCacheIndex.Initialize(new Level2MemcachedCacheIndex());
				CfPerfCache.Initialize(new Level2MemcachedPerfCache());
				// Set the maximum number of concurrent connections 
				//ServicePointManager.DefaultConnectionLimit = 12;
			}
			catch (Exception ex)
			{
				CfTrace.Error(ex);
			}

			return base.OnStart();
		}

		public override void Run()
		{
			Trace.WriteLine("cf.AlertsServer entry point called", "Information");

			while (true)
			{
				try
				{
					Thread.Sleep(3000); //-- Sleep for 3 seconds before trying to process again
					AlertsProcessor.TryProcessMessageAlerts();
					AlertsProcessor.TryProcessCommentAlerts();
					AlertsProcessor.TryProcessPartnerCallAlerts();
				}
				catch (Exception ex)
				{
					//CfTrace.Error(ex);
					cf.Mail.MailMan.SendAppEvent(TraceCode.Exception, string.Format("{0}, {1}", LastQueueMessage.MessageAsString, ex), "system@climbfind.com", Guid.Empty, "jkresner@yahoo.com.au", false);
				}
			}            
		}
	}
}
