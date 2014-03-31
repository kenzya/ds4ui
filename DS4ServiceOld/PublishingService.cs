using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationLibrary;

namespace DS4Service
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
