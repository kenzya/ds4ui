using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ServiceModel;
using CommunicationLibrary;

namespace DS4Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SubscribingService : ISubscribingService
    {
        private readonly DS4Service service;
        private readonly EventLog eventLog;
        private static Collection<IPublishingService> subscribers;

        public SubscribingService(DS4Service service, EventLog logger)
        {
            this.service = service;
            this.eventLog = logger;
            subscribers = new Collection<IPublishingService>();
        }

        internal static IEnumerable<IPublishingService> GetSubscribers()
        {
            lock (typeof(SubscribingService))
            {
                return subscribers;
            }
        }

        public void Subscribe()
        {
            lock (typeof(SubscribingService))
            {
                IPublishingService subscriber = OperationContext.Current.GetCallbackChannel<IPublishingService>();
                if (!subscribers.Contains(subscriber))
                {
                    subscribers.Add(subscriber);
                    eventLog.WriteEntry(ServiceMessages.MESSAGE_CLIENT_CONNECTED);
                }
            }
        }
        public void Unsubscribe()
        {
            lock (typeof(SubscribingService))
            {
                IPublishingService subscriber = OperationContext.Current.GetCallbackChannel<IPublishingService>();
                subscribers.Remove(subscriber);
                eventLog.WriteEntry(ServiceMessages.MESSAGE_CLIENT_DISCONNECTED);
            }
        }

        public void SendCommand(ControllerContract param)
        {
            service.OnCustomCommand(param);
        }
    }
}
