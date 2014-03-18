using System.ServiceModel;

namespace CommunicationLibrary
{
    [ServiceContract(CallbackContract = typeof(IPublishingService))]
    public interface ISubscribingService
    {
        [OperationContract]
        void SendCommand(int command, object param);

        [OperationContract(IsOneWay = true)]
        void Subscribe();

        [OperationContract(IsOneWay = true)]
        void Unsubscribe();
    }
}
