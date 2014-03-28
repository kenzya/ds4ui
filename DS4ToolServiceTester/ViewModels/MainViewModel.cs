using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Input;
using CommunicationLibrary;
using CoreLibrary;

namespace DS4ToolTester
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ISendService sp;
        private readonly ServiceController sc;
        private readonly ServiceInstaller si;
        private readonly EventLog el;
        private readonly string svcPath;

        /// <summary>
        /// Ctor
        /// </summary>
        public MainViewModel()
        {
            svcPath = Path.Combine(Environment.CurrentDirectory, "DS4ToolService.exe");

            el = new EventLog("Application", Environment.MachineName, Constants.SERVICE_NAME);
            el.EntryWritten += new EntryWrittenEventHandler(LogWritten);
            el.EnableRaisingEvents = true;

            si = new ServiceInstaller();
            si.InstallService(svcPath, Constants.SERVICE_NAME);

            DuplexChannelFactory<ISendService> pipeFactory = new DuplexChannelFactory<ISendService>(new PushService(),
                new NetNamedPipeBinding(), new EndpointAddress(Constants.PIPE_ADDRESS + "tester"));
            sp = pipeFactory.CreateChannel();

            sc = new ServiceController(Constants.SERVICE_NAME, Environment.MachineName);
            new ServiceControllerPermission(ServiceControllerPermissionAccess.Browse, Environment.MachineName, Constants.SERVICE_NAME).Assert();
            sc.Refresh();
            sc.WaitForStatus(ServiceControllerStatus.Running);
        }

        #region Controllers

        private ObservableCollection<ControllerViewModel> controllers;
        public ObservableCollection<ControllerViewModel> Controllers
        {
            get
            {
                if (controllers == null)
                {
                    controllers = new ObservableCollection<ControllerViewModel>();
                }
                return controllers;
            }
        }

        private string controllerId;
        public string ControllerId
        {
            get 
            {
                return controllerId;
            }
            set
            {
                if (controllerId != value)
                {
                    controllerId = value;
                    NotifyPropertyChanged(() => ControllerId);
                }
            }
        }
        
        private string controllerName;
        public string ControllerName
        {
            get
            {
                return controllerName;
            }
            set
            {
                if (controllerName != value)
                {
                    controllerName = value;
                    NotifyPropertyChanged(() => ControllerName);
                }
            }
        }

        private ConnectionTypes connectionType;
        public ConnectionTypes ConnectionType
        {
            get
            {
                return connectionType;
            }
            set
            {
                if (connectionType != value)
                {
                    connectionType = value;
                    NotifyPropertyChanged(() => ConnectionType);
                }
            }
        }

        private DelegateCommand addController;
        public ICommand AddController
        {
            get
            {
                if (addController == null)
                {
                    addController = new DelegateCommand(param => ExecuteAddController());
                }
                return addController;
            }
        }
        private void ExecuteAddController()
        {
            ControllerViewModel c = new ControllerViewModel(sp, si, ControllerId, ControllerName, ConnectionType);
            c.RequestRemove += ExecuteRemoveController;
            Controllers.Add(c);
        }
        private void ExecuteRemoveController(object sender, EventArgs e)
        {
            ControllerViewModel c = sender as ControllerViewModel;
            c.RequestRemove -= ExecuteRemoveController;
            Controllers.Remove(c);
        }

        #endregion

        #region Logger

        private CustomCollection<string> logs;
        public CustomCollection<string> Logs
        {
            get
            {
                if (logs == null)
                {
                    logs = new CustomCollection<string>();
                }
                return logs;
            }
        }

        private void LogWritten(object source, EntryWrittenEventArgs e)
        {
            Logs.Add(e.Entry.Message);
        }

        #endregion // LOGGER

        #region  Dispose Services

        public void DeleteService()
        {
            if (sc.Status == ServiceControllerStatus.Running)
                sc.Stop();

            sc.WaitForStatus(ServiceControllerStatus.Stopped);
            si.UnInstallService(Constants.SERVICE_NAME);
        }
        public void DeleteEventLog()
        {
            EventLog eventLog = new EventLog();
            eventLog.Source = Constants.SERVICE_NAME;
            eventLog.Log = "Application";
            if (!EventLog.SourceExists(eventLog.Source))
            {
                EventLog.DeleteEventSource(eventLog.Source, Environment.MachineName);
            }
        }

        #endregion
    }
}
