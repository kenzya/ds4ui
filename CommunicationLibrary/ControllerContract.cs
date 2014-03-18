using System.Runtime.Serialization;

namespace CommunicationLibrary
{
    [DataContract]
    public class ControllerContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsUsbConnected { get; set; }

        [DataMember]
        public bool IsBluetoothConnected { get; set; }

        [DataMember]
        public int BatteryValue { get; set; }

        [DataMember]
        public bool IsIconVisible { get; set; }

        public static ControllerContract Create(int id, string name, bool isUsbConnected, bool isBluetoothConnected, int batteryValue, bool isIconVisible = false)
        {
            return new ControllerContract
            {
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
