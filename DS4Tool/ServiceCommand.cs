using CommunicationLibrary;
using System.ServiceModel;

namespace DS4Tool
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class ServiceCommand: IPublishingService
    {
        public void PushCommand(ControllerContract param)
        {
            App.AppManager.NotifyControllerChange(param);
        }
    }
}
