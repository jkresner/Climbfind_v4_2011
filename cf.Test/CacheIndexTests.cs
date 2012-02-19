using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cf.Services;
using cf.Entities;
using cf.Caching;
using cf.Identity;
using cf.DataAccess.Repositories;
using cf.DataAccess.Azure;
using Microsoft.IdentityModel.Claims;
using Microsoft.WindowsAzure;
using System.Configuration;
using cf.Instrumentation;

namespace cf.Test
{
	[TestClass]
	public class CacheIndexTests
	{
		private CloudStorageAccount CfCloudStorage;
		private BlobRepository blobRepo;
		
		[TestInitialize]
		public void Initialize()
		{
			var connectionString = ConfigurationManager.ConnectionStrings["CfCloudStorage"].ConnectionString;
			CfCloudStorage = CloudStorageAccount.Parse(connectionString);
			blobRepo = new BlobRepository();
			CfTrace.InitializeTraceSource(new Instrumentation.CfTraceSource("Cf.CacheServer"));
		}

		[TestMethod]
		public void Can_Read_Cache_Index_IntoMemory()
		{
			var index = new cf.DataAccess.Repositories.CFCacheIndexEntryRepository().GetAll().ToList();
			Assert.IsTrue(index.Count >  1000);
		}
		
		[TestMethod]
		public void Can_Clear_And_Rewrite_Search_Index_ToCloudStorage()
		{
			var dir = new Lucene.Net.Store.Azure.AzureDirectory(CfCloudStorage, "SearchCatalog", new Lucene.Net.Store.RAMDirectory());
			foreach (var b in dir.BlobContainer.ListBlobs())
			{
				dir.BlobContainer.GetBlobReference(b.Uri.ToString()).Delete();
			}
			new cf.Content.Search.SearchManager().BuildIndex(dir);

			var expected = blobRepo.GetBlobWithProperties("searchcatalog", "_0.cfs");
			Assert.IsTrue(expected.Properties.LastModifiedUtc > DateTime.UtcNow.AddMinutes(-1));
		}
	}
}
