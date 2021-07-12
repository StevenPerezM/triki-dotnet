using System.Threading.Tasks;

namespace Tricky.Core.Business.Interfaces
{
    public interface IQueueService
    {
        void sendMessageAsync<T>(T requestObject, string sqsUrl, int delay);
    }
}
