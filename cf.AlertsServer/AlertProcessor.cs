using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using cf.Dtos.Cloud;
using cf.DataAccess.Azure;
using NetFrameworkExtensions;
using System.Web.Script.Serialization;
using cf.Services;

namespace cf.AlertsServer
{
    public static partial class AlertsProcessor
    {
        static readonly QueueRepository queueRepo;
        static readonly AlertsService alrSvc;
        
        static AlertsProcessor()
        {
            queueRepo = new QueueRepository();
            alrSvc = new AlertsService();
        }

        public static bool TryProcessMessageAlerts() { 
            return ExecuteAlertOperation<MessageAlertWorkItem>(AlertsService.MessagesQueueName, dto => alrSvc.ProcessMessageWorkItem(dto));
        }

        public static bool TryProcessCommentAlerts() {
            return ExecuteAlertOperation<CommentAlertWorkItem>(AlertsService.CommentQueueName, dto => alrSvc.ProcessCommentWorkItem(dto));
        }

        public static bool TryProcessPartnerCallAlerts() { 
            return ExecuteAlertOperation<PartnerCallAlertWorkItem>(AlertsService.PartnerCallQueueName, dto => alrSvc.ProcessPartnerCallWorkItem(dto));
        }

        /// <summary>
        /// Grab the queue message and delete it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static bool ExecuteAlertOperation<T>(string queueName, Action<T> operation)
        {
            CloudQueueMessage queueMsg = null;
            if (queueRepo.GetMessage(queueName, out queueMsg))
            {
                if (queueMsg.DequeueCount < 3) //-- Try process twice and then just delete the message
                {
                    LastQueueMessage.MessageAsString = string.Format("Dequeue[{0}] {1}", queueMsg.DequeueCount, queueMsg.AsString);
                    var dto = new JavaScriptSerializer().Deserialize<T>(queueMsg.AsString);
                    operation(dto);
                }
                queueRepo.DeleteMessage(queueName, queueMsg);
                return true;
            }

            return false;
        }
    }
}
