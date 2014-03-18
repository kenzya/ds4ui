using System.ServiceModel;

namespace CommunicationLibrary
{
    [ServiceContract]
    public interface IPublishingService
    {
        [OperationContract(IsOneWay = true)]
        void PushCommand(ControllerContract param);
    }
}
