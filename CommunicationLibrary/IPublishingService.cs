using System.ServiceModel;

namespace CommunicationLibrary
{
    /// <summary>
    /// Interface of the ServiceContract implemented by the service
    /// </summary>
    [ServiceContract]
    public interface IPublishingService
    {
        /// <summary>
        /// Send a new Contract to the subscribed clients
        /// </summary>
        /// <param name="param">Controller's contract</param>
        [OperationContract(IsOneWay = true)]
        void PushCommand(ControllerContract param);
    }
}
