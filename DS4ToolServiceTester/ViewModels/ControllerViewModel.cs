using System.Windows.Input;
using CommunicationLibrary;
using CoreLibrary;
using System;

namespace DS4ToolTester
{
    public class ControllerViewModel : ViewModelBase
    {
        private readonly ISendService sp;
        private readonly ServiceInstaller si;

        private int controllerId;
        public int ControllerId
        {
            get
            {
                return controllerId;
            }
            private set
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
            private set
            {
                if (controllerName != value)
                {
                    controllerName = value;
                    NotifyPropertyChanged(() => ControllerName);
                }
            }
        }

        private int batteryValue;
        public int BatteryValue
        {
            get
            {
                return batteryValue;
            }
            set
            {
                if (batteryValue != value)
                {
                    batteryValue = value;
                    NotifyPropertyChanged(() => BatteryValue);

                    if (batteryValue % 10 == 0)
                        ExecuteControllerChange();
                }
            }
        }

        public ControllerViewModel(ISendService sendService, ServiceInstaller serviceInstaller, int controllerId, string controllerName, ConnectionTypes connection)
        {
            this.sp = sendService;
            this.si = serviceInstaller;

            this.ControllerId = controllerId;
            this.ControllerName = controllerName;
            this.ControllerUsbChecked = connection == ConnectionTypes.USB;
            this.ControllerBtChecked = connection == ConnectionTypes.Bluetooth;
            this.BatteryValue = 100;
        }

        private bool controllerUsbChecked;
        public bool ControllerUsbChecked
        {
            get
            {
                return controllerUsbChecked;
            }
            set
            {
                if (controllerUsbChecked != value)
                {
                    controllerUsbChecked = value;
                    NotifyPropertyChanged(() => ControllerUsbChecked);
                    NotifyPropertyChanged(() => ControllerUsbNotChecked);
                }
            }
        }
        public bool ControllerUsbNotChecked
        {
            get { return !ControllerUsbChecked; }
        }

        private DelegateCommand controllerUsb;
        public ICommand ControllerUsb
        {
            get
            {
                if (controllerUsb == null)
                {
                    controllerUsb = new DelegateCommand(param => ExecuteControllerChange(), param => CanExecuteControllerUsb);
                }
                return controllerUsb;
            }
        }
        private bool CanExecuteControllerUsb
        {
            get
            {
                return si.DoesServiceExist(Constants.SERVICE_NAME);
            }
        }
        
        private bool controllerBtChecked;
        public bool ControllerBtChecked
        {
            get
            {
                return controllerBtChecked;
            }
            set
            {
                if (controllerBtChecked != value)
                {
                    controllerBtChecked = value;
                    NotifyPropertyChanged(() => ControllerBtChecked);
                }
            }
        }

        private DelegateCommand controllerBt;
        public ICommand ControllerBt
        {
            get
            {
                if (controllerBt == null)
                {
                    controllerBt = new DelegateCommand(param => ExecuteControllerChange(), param => CanExecuteControllerBt);
                }
                return controllerBt;
            }
        }
        private bool CanExecuteControllerBt
        {
            get
            {
                return si.DoesServiceExist(Constants.SERVICE_NAME);
            }
        }

        private void ExecuteControllerChange()
        {
            ControllerContract ct = ControllerContract.Create(ControllerId, ControllerName, ControllerUsbChecked, ControllerBtChecked, BatteryValue);

            sp.SendCommand(ct);

            if (ControllerUsbChecked == false && ControllerBtChecked == false)
                OnRequestRemove();
        }

        public event EventHandler RequestRemove;
        public void OnRequestRemove()
        {
            EventHandler handler = this.RequestRemove;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
