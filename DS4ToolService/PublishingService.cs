using CommunicationLibrary;

namespace DS4ToolService
{
    public class PublishingService : IPublishingService
    {
        public void PushCommand(ControllerContract param)
        {
            foreach (IPublishingService subscriber in SubscribingService.GetSubscribers())
            {
                subscriber.PushCommand(param);
            }
        }
    }
}