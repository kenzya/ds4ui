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

            SendService sndServ = new SendService(this);
            ServiceHost svcHost = new ServiceHost(sndServ, new Uri(Constants.PIPE_ADDRESS));
            svcHost.AddServiceEndpoint(typeof(ISendService), new NetNamedPipeBinding(), "tester");
            svcHost.Open();

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
            this.EventLog.WriteEntry("Change controller status request");
            this.publisher.PushCommand(param);
        }
    }
}
