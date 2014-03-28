using System.Runtime.Serialization;

namespace CommunicationLibrary
{
    /// <summary>
    /// DataContract used to communicate the controller status from service and client
    /// </summary>
    /// TODO: investigate on sending separate contract instead of the entire status
    [DataContract]
    public class ControllerContract
    {
        /// <summary>
        /// Message to be sent with the data, mostly used for eventlog messages
        /// </summary>
        [DataMember]
        public ControllerMessage Message { get; set;}

        /// <summary>
        /// Id of the controller
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Name assigned to the controller
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Flagged when the controller is connected through USB
        /// </summary>
        [DataMember]
        public bool IsUsbConnected { get; set; }

        /// <summary>
        /// Flagged when the controller is connected through Bluetooth
        /// </summary>
        [DataMember]
        public bool IsBluetoothConnected { get; set; }

        /// <summary>
        /// Battery charge. It's a value from 0 (empty) to 100 (full)
        /// </summary>
        [DataMember]
        public int BatteryValue { get; set; }

        /// <summary>
        /// Flagged when the controller's icon should be visible on TaskBar
        /// </summary>
        [DataMember]
        public bool IsIconVisible { get; set; }

        /// <summary>
        /// Create a new Contract to be sent to the pipe
        /// </summary>
        public static ControllerContract Create(string id, string name, bool isUsbConnected, bool isBluetoothConnected, int batteryValue, ControllerMessage message = ControllerMessage.NONE, bool isIconVisible = false)
        {
            return new ControllerContract
            {
                Message = message,
                Id = id,
                Name = name,
                IsUsbConnected = isUsbConnected,
                IsBluetoothConnected = isBluetoothConnected,
                BatteryValue = batteryValue,
                IsIconVisible = isIconVisible
            };
        }
    }
}
