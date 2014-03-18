using CommunicationLibrary;
using System;
using System.ServiceModel;

namespace DS4ToolTester
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class PushService : IPushService
    {
        public void PushCommand(ControllerContract param)
        {
            throw new NotImplementedException();
        }
    }
}
