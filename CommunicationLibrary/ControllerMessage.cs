
namespace CommunicationLibrary
{
    public enum ControllerMessage
    {
        NONE = 0,
        CONTROLLER_CONNECT_USB = 1,
        CONTROLLER_CONNECT_BT = 2,
        CONTROLLER_DISCONNECT_USB = 3,
        CONTROLLER_DISCONNECT_BT = 4,
        CONTROLLER_DISCONNECT = 5,
        CONTROLLER_BATTERY_CHANGE = 6,
        CONTROLLER_TOUCHPAD_CHANGE = 7
    }
}
