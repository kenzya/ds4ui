using System.ServiceModel;

namespace CommunicationLibrary
{
    /// <summary>
    /// ServiceContract implemented by the clients.
    /// It's a secondary channel used for testing purpose
    /// </summary>
    /// TODO: used only for DS4ToolService, delete it
    [ServiceContract(CallbackContract = typeof(IPushService))]
    public interface ISendService
    {
        /// <summary>
        /// Send message to the service
        /// </summary>
        [OperationContract]
        void SendCommand(ControllerContract param);
    }

    /// <summary>
    /// ServiceContract implemented by the service.
    /// It's a secondary channel used only for testing purpose
    /// </summary>
    /// TODO: used only for DS4ToolService, delete it
    [ServiceContract]
    public interface IPushService
    {
        /// <summary>
        /// Send message to the clients
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void PushCommand(ControllerContract param);
    }
}