using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAlbums.Notifications
{
    [StorageAccount("AzureWebJobsStorage")]
    public class Functions
    {
        public static void ProcessQueueMessage([QueueTrigger("queue")] string message, ILogger logger)
        {
            logger.LogInformation(message);
        }
    }
}
