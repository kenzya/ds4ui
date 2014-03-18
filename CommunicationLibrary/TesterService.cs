using System.ServiceModel;

namespace CommunicationLibrary
{
    [ServiceContract(CallbackContract = typeof(IPushService))]
    public interface ISendService
    {
        [OperationContract]
        void SendCommand(ControllerContract param);
    }

    [ServiceContract]
    public interface IPushService
    {
        [OperationContract(IsOneWay = true)]
        void PushCommand(ControllerContract param);
    }
}