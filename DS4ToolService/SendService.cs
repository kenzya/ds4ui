using System.ServiceModel;
using CommunicationLibrary;

namespace DS4ToolService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class SendService : ISendService
    {
        DS4ToolService service;

        public SendService(DS4ToolService s)
        {
            service = s;
        }

        public void SendCommand(ControllerContract param)
        {
            service.OnCustomCommand(param);
        }
    }
}