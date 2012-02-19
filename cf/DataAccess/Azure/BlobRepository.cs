using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace cf.DataAccess.Azure
{
    public class BlobRepository
    {        
        public CloudStorageAccount StorageAccount { get; set; }
        public CloudBlobClient BlobClient { get; set; }

        public BlobRepository()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CfCloudStorage"].ConnectionString;
            StorageAccount = CloudStorageAccount.Parse(connectionString);
            BlobClient = StorageAccount.CreateCloudBlobClient();
            BlobClient.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.Zero);    
        }

        public string DownloadBlogAsText(string containerName, string fileName)
        {
            CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
            CloudBlob blob = container.GetBlobReference(fileName);
            return blob.DownloadText();
        }

		public CloudBlob GetBlobWithProperties(string containerName, string fileName)
		{
			CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
			CloudBlob blob = container.GetBlobReference(fileName);
			blob.FetchAttributes();
			return blob;
		}

		public CloudBlobContainer GetContainerWithProperties(string containerName)
		{
			CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
			container.FetchAttributes();
			return container;
		}

		public void Delete(string containerName, string fileName)
		{
			CloudBlobContainer container = BlobClient.GetContainerReference(containerName);
			CloudBlob blob = container.GetBlobReference(fileName);
			blob.Delete();
		}
    }
}
