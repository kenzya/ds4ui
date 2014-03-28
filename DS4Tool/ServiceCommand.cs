using System.ServiceModel;
using CommunicationLibrary;
using MessengerLibrary;

namespace DS4Tool
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class ServiceCommand: IPublishingService
    {
        private readonly IMessengerManager MessengerManager;

        public ServiceCommand(IMessengerManager messenger)
        {
            MessengerManager = messenger;
        }

        public void PushCommand(ControllerContract param)
        {            
            MessengerManager.NotifyColleagues(AppMessages.CONTROLLER_CHANGE_STATUS, param);
        }
    }
}
