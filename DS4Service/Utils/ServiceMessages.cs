
namespace DS4Service
{
    // TODO: complete messages
    public class ServiceMessages
    {
        public const string MESSAGE_START = "Service started";
        public const string MESSAGE_STOP = "Service stopped";

        public const string MESSAGE_CONTROLLER_CONNECT_USB = "Controller connected through USB";
        public const string MESSAGE_CONTROLLER_CONNECT_BT = "Controller connected through Bluetooth";
        public const string MESSAGE_CONTROLLER_DISCONNECT_USB = "Controller disconnected from USB";
        public const string MESSAGE_CONTROLLER_DISCONNECT_BT = "Controller disconnected from Bluetooth";
        public const string MESSAGE_CONTROLLER_BATTERY_CHANGE = "Battery charge changed";
        public const string MESSAGE_CONTROLLER_CHANGE_STATUS = "Change controller status request";

        public const string MESSAGE_CLIENT_CONNECTED = "Client connected to the service";
        public const string MESSAGE_CLIENT_DISCONNECTED = "Client disconnected from the service";

        public const string MESSAGE_CONTROLLERS_EXCLUSIVE_MODE = "Set exclusive mode";

        public const string MESSAGE_CONTROLLER_LED_COLOR_CHANGE = "Led color changed";
        public const string MESSAGE_CONTROLLER_LED_FLASH = "Set Led to flash when low battery";
        public const string MESSAGE_CONTROLLER_LED_BATTERY = "Set Led to be used as a battery indicator";

        public const string MESSAGE_CONTROLLER_TOUCH_VALUE = "Set touch sensitivity";
        public const string MESSAGE_CONTROLLER_TOUCH_TAP = "Set tap sensitivity";
        public const string MESSAGE_CONTROLLER_TOUCH_SCROLL = "Set scroll sensitivity";
        public const string MESSAGE_CONTROLLER_TOUCH_STARTUP = "Set touchpad to be enabled at startup";
        public const string MESSAGE_CONTROLLER_TOUCH_JITTER = "Set touchpad jitter compensation";
        public const string MESSAGE_CONTROLLER_TOUCH_RIGHT = "Set touchpad lower right click";

        public const string MESSAGE_CONTROLLER_RUMBLE_BOOST = "Set rumble boost";
        public const string MESSAGE_CONTROLLER_RUMBLE_HEAVY = "Set heavy rumble boost";
        public const string MESSAGE_CONTROLLER_RUMBLE_LIGHT = "Set light rumble boost";
        public const string MESSAGE_CONTROLLER_RUMBLE_SWAP = "Set swap of rumble inputs";

        public const string MESSAGE_CONTROLLER_TUNING_TRIGGER = "Set trigger threshold";
        public const string MESSAGE_CONTROLLER_TUNING_IDLE = "Set idle disconnection time";
        public const string MESSAGE_CONTROLLER_TUNING_REALTIME = "Set realtime changes";
        public const string MESSAGE_CONTROLLER_TUNING_FLUSH = "Set flush hid after each reading";
        public const string MESSAGE_CONTROLLER_TUNING_GYRO = "Set gyro/acceleration data";
    }
}
