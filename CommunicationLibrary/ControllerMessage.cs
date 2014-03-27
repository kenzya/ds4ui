using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationLibrary
{
    public enum ControllerMessage
    {
        NONE = 0,
        CONTROLLER_CONNECT_USB = 1,
        CONTROLLER_CONNECT_BT = 2,
        CONTROLLER_DISCONNECT_USB = 3,
        CONTROLLER_DISCONNECT_BT = 4,
        CONTROLLER_BATTERY_CHANGE = 5
    }
}
