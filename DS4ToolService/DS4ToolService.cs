using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceProcess;
using CommunicationLibrary;

namespace DS4ToolService
{
    public partial class DS4ToolService : ServiceBase 
    {
        private readonly IPublishingService publisher;

        public DS4ToolService()
        {
            this.ServiceName = Constants.SERVICE_NAME;
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = false;

            this.EventLog.Source = Constants.SERVICE_NAME;
            this.EventLog.Log = "Application";
            if (!EventLog.SourceExists(this.EventLog.Source))
            {
                EventLog.CreateEventSource(this.EventLog.Source, this.EventLog.Log);
            }

            SubscribingService subServ = new SubscribingService(this.EventLog);
            ServiceHost serviceHost = new ServiceHost(subServ, new Uri(Constants.PIPE_ADDRESS));
            serviceHost.AddServiceEndpoint(typeof(ISubscribingService), new NetNamedPipeBinding(), Constants.SERVICE_NAME);
            serviceHost.Open();
            publisher = new PublishingService();
        }

        internal static void Main()
        {
            ServiceBase.Run(new DS4ToolService());
        }

        protected override void OnStart(string[] args)
        {
            this.EventLog.WriteEntry(ServiceMessages.MESSAGE_START);
        }
        protected override void OnStop()
        {
            this.EventLog.WriteEntry(ServiceMessages.MESSAGE_STOP);
        }

        internal void OnCustomCommand(ControllerContract param)
        {
            switch (param.Message)
            { 
                case ControllerMessage.CONTROLLER_CONNECT_USB:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_CONNECT_USB);
                    this.publisher.PushCommand(param);
                    break;

                case ControllerMessage.CONTROLLER_CONNECT_BT:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_CONNECT_BT);
                    this.publisher.PushCommand(param);
                    break;

                case ControllerMessage.CONTROLLER_DISCONNECT_USB:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_DISCONNECT_USB);
                    this.publisher.PushCommand(param);
                    break;

                case ControllerMessage.CONTROLLER_DISCONNECT_BT:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_DISCONNECT_BT);
                    this.publisher.PushCommand(param);
                    break;

                case ControllerMessage.CONTROLLER_BATTERY_CHANGE:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_BATTERY_CHANGE);
                    this.publisher.PushCommand(param);
                    break;

                case ControllerMessage.NONE:
                    this.EventLog.WriteEntry("Change controller status request");
                    this.publisher.PushCommand(param);
                    break;
            }
        }
    }
}
