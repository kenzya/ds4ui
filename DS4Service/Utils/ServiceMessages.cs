
namespace DS4Service
{
    public class ServiceMessages
    {
        public const string MESSAGE_START = "Service started";
        public const string MESSAGE_STOP = "Service stopped";

        public const string MESSAGE_CONTROLLER_CONNECT_USB = "Controller connected through USB";
        public const string MESSAGE_CONTROLLER_CONNECT_BT = "Controller connected through Bluetooth";
        public const string MESSAGE_CONTROLLER_DISCONNECT_USB = "Controller disconnected from USB";
        public const string MESSAGE_CONTROLLER_DISCONNECT_BT = "Controller disconnected from Bluetooth";

        public const string MESSAGE_CONTROLLER_BATTERY_CHANGE = "Battery charge changed";

        public const string MESSAGE_CLIENT_CONNECTED = "Client connected to the service";
        public const string MESSAGE_CLIENT_DISCONNECTED = "Client disconnected to the service";

        public const string MESSAGE_CONTROLLERS_EXCLUSIVE_ENABLED = "Exclusive mode enabled";
        public const string MESSAGE_CONTROLLERS_EXCLUSIVE_DISABLED = "Exclusive mode disabled";
    }
}
