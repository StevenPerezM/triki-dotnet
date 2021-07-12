using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Tricky.Core.Business.Interfaces;

namespace Tricky.Core.Services.SQSService
{
    public class SqsQueueService: IQueueService
    {
        private readonly IAmazonSQS _client;
        public SqsQueueService(IAmazonSQS client)
        {
            _client = client;
        }
        public void sendMessageAsync<T>(T requestObject, string sqsUrl, int delay)
        {
            var sendRequest = new SendMessageRequest
            {
                MessageBody = JsonConvert.SerializeObject(requestObject, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                QueueUrl = sqsUrl,
                DelaySeconds = delay
            };
             _client.SendMessageAsync(sendRequest);
        }
    }
}
