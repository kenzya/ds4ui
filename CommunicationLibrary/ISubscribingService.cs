using System.ServiceModel;

namespace CommunicationLibrary
{
    /// <summary>
    /// Interface of the ServiceContract implemented by the clients
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IPublishingService))]
    public interface ISubscribingService
    {
        /// <summary>
        /// Send a command to the service
        /// </summary>
        [OperationContract]
        void SendCommand(ControllerContract contract);

        /// <summary>
        /// Subscribe to receive messages from the service
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Subscribe();

        /// <summary>
        /// Unsubscribe from the list of clients susbscribed to the service
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Unsubscribe();
    }
}
