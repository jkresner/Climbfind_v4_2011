using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using cf.Instrumentation;

namespace cf.DataAccess.Azure
{
    public class QueueRepository
    {
        static TimeSpan FourMins = new TimeSpan(0, 4, 0);
        
        public CloudStorageAccount StorageAccount { get; set; }
        public CloudQueueClient QueueClient { get; set; }

        public QueueRepository()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CfCloudStorage"].ConnectionString;
            StorageAccount = CloudStorageAccount.Parse(connectionString);
            QueueClient = StorageAccount.CreateCloudQueueClient();
            QueueClient.RetryPolicy = RetryPolicies.Retry(4, TimeSpan.Zero);
        }

        // Create a queue. 
        // Return true on success, false if already exists, throw exception on error.

        public bool CreateQueue(string queueName)
        {
            try
            {
                CloudQueue queue = QueueClient.GetQueueReference(queueName);
                queue.Create();
                return true;
            }
            catch (StorageClientException ex)
            {
                if ((int)ex.StatusCode == 409)
                {
                    return false;
                }

                throw;
            }
        }

        // Delete a queue. 
        // Return true on success, false if not found, throw exception on error.

        public bool DeleteQueue(string queueName)
        {
            try
            {
                CloudQueue queue = QueueClient.GetQueueReference(queueName);
                queue.Delete();
                return true;
            }
            catch (StorageClientException ex)
            {
                if ((int)ex.StatusCode == 404)
                {
                    return false;
                }

                throw;
            }
        }

        // Retrieve the next message from a queue. 
        // Return true on success (message available), false if no message or no queue, throw exception on error.

        public bool GetMessage(string queueName, out CloudQueueMessage message)
        {
            message = null;

            try
            {
                CloudQueue queue = QueueClient.GetQueueReference(queueName);
                message = queue.GetMessage(FourMins);
                return message != null;
            }
            catch (StorageClientException ex)
            {
                if ((int)ex.StatusCode == 404)
                {
                    return false;
                }

                throw;
            }
        }


        // Create or update a blob. 
        // Return true on success, false if already exists, throw exception on error.

        public bool PutMessage(string queueName, CloudQueueMessage message)
        {
            try
            {
                CloudQueue queue = QueueClient.GetQueueReference(queueName);
                queue.AddMessage(message);
                return true;
            }
            catch (StorageClientException ex)
            {
                CfTrace.Error(ex);

                return false;
                
                //-- no throw because if this fails, prefer not to break the app for the user
                //throw;
            }
        }

        // Delete a previously read message. 
        // Return true on success, false if already exists, throw exception on error.

        public bool DeleteMessage(string queueName, CloudQueueMessage message)
        {
            try
            {
                CloudQueue queue = QueueClient.GetQueueReference(queueName);
                if (message != null)
                {
                    queue.DeleteMessage(message);
                }
                return true;
            }
            catch (StorageClientException ex)
            {
                if ((int)ex.StatusCode == 404)
                {
                    return false;
                }

                throw;
            }
        }


        // Clear all messages from a queue. 
        // Return true on success, false if already exists, throw exception on error.

        public bool ClearMessages(string queueName)
        {
            try
            {
                CloudQueue queue = QueueClient.GetQueueReference(queueName);
                queue.Clear();
                return true;
            }
            catch (StorageClientException ex)
            {
                if ((int)ex.StatusCode == 404)
                {
                    return false;
                }

                throw;
            }
        }
    }
}
