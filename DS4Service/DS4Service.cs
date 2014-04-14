using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Windows;
using CommunicationLibrary;
using DS4Control;
using TranslationLibrary;

namespace DS4Service
{
    public partial class DS4Service : ServiceBase
    {        
        private readonly IPublishingService publisher;
        private readonly Control rootHub;
        private readonly StreamWriter logWriter;
        private readonly DS4Events events;

        string logFile = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + @"\DS4Service.log";

        public DS4Service()
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

            this.events = new DS4Events();

            SubscribingService subServ = new SubscribingService(this, this.EventLog);
            ServiceHost serviceHost = new ServiceHost(subServ, new Uri(Constants.PIPE_ADDRESS));
            serviceHost.AddServiceEndpoint(typeof(ISubscribingService), new NetNamedPipeBinding(), Constants.SERVICE_NAME);
            serviceHost.Open();
            publisher = new PublishingService();

            rootHub = new Control();
            rootHub.Debug += On_Debug;
            logWriter = File.AppendText(logFile);

            Global.Load();
            Global.loadCustomMapping(0);
            Global.ControllerStatusChange += ControllerStatusChange;
        }

        internal static void Main()
        {
            ServiceBase.Run(new DS4Service());
        }

        protected override void OnStart(string[] args)
        {
            rootHub.Start();            
            this.EventLog.WriteEntry(ServiceMessages.MESSAGE_START);
        }

        protected override void OnStop()
        {
            rootHub.Stop();
            this.EventLog.WriteEntry(ServiceMessages.MESSAGE_STOP);
        }

        protected void On_Debug(object sender, DebugEventArgs e)
        {
            logWriter.WriteLine(e.Time + ":\t" + e.Data);
            logWriter.Flush();
        }

        protected void ControllerStatusChange(object sender, StatusChangeEventArgs e)
        {
            ControllerContract c = ControllerContract.Create(e.DeviceId, e.UsbConnected, e.BtConnected, e.DeviceBattery, e.Message);
            OnCustomCommand(c);
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

                case ControllerMessage.CONTROLLERS_EXCLUSIVE_MODE:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLERS_EXCLUSIVE_MODE);
                    this.events.ChangeExclusive(param);
                    break;

                case ControllerMessage.CONTROLLER_LED_COLOR:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_LED_COLOR_CHANGE);
                    this.events.ChangeLedColor(param);
                    break;

                case ControllerMessage.CONTROLLER_LED_FLASH:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_LED_FLASH);
                    this.events.ChangeLedFlash(param);
                    break;

                case ControllerMessage.CONTROLLER_LED_BATTERY:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_LED_BATTERY);
                    this.events.ChangeLedBattery(param);
                    break;

                case ControllerMessage.CONTROLLER_TOUCH_VALUE:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_TOUCH_VALUE);
                    this.events.ChangeTouchValue(param);
                    break;

                case ControllerMessage.CONTROLLER_TOUCH_TAP:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_TOUCH_TAP);
                    this.events.ChangeTouchTap(param);
                    break;

                case ControllerMessage.CONTROLLER_TOUCH_SCROLL:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_TOUCH_SCROLL);
                    this.events.ChangeTouchScroll(param);
                    break;

                case ControllerMessage.CONTROLLER_TOUCH_STARTUP:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_TOUCH_STARTUP);
                    this.events.ChangeTouchStartup(param);
                    break;

                case ControllerMessage.CONTROLLER_TOUCH_JITTER:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_TOUCH_JITTER);
                    this.events.ChangeTouchJitter(param);
                    break;

                case ControllerMessage.CONTROLLER_TOUCH_RIGHT:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_TOUCH_RIGHT);
                    this.events.ChangeTouchRight(param);
                    break;

                case ControllerMessage.CONTROLLER_RUMBLE_BOOST:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_RUMBLE_BOOST);
                    this.events.ChangeRumbleBoost(param);
                    break;

                case ControllerMessage.CONTROLLER_RUMBLE_HEAVY:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_RUMBLE_HEAVY);
                    this.events.ChangeRumbleHeavy(param);
                    break;

                case ControllerMessage.CONTROLLER_RUMBLE_LIGHT:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_RUMBLE_LIGHT);
                    this.events.ChangeRumbleLight(param);
                    break;

                case ControllerMessage.CONTROLLER_RUMBLE_SWAP:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_RUMBLE_SWAP);
                    this.events.ChangeRumbleSwap(param);
                    break;

                case ControllerMessage.CONTROLLER_TUNING_LEFT:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_TUNING_TRIGGER);
                    this.events.ChangeTuningLeft(param);
                    break;

                case ControllerMessage.CONTROLLER_TUNING_RIGHT:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_TUNING_TRIGGER);
                    this.events.ChangeTuningRight(param);
                    break;

                case ControllerMessage.CONTROLLER_TUNING_IDLE:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_TUNING_IDLE);
                    this.events.ChangeTuningIdle(param);
                    break;

                case ControllerMessage.NONE:
                    this.EventLog.WriteEntry(ServiceMessages.MESSAGE_CONTROLLER_CHANGE_STATUS);
                    this.publisher.PushCommand(param);
                    break;
            }
        }
    }
}