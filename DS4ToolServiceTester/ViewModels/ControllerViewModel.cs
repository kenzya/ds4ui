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

        private string controllerId;
        public string ControllerId
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
                if (batteryValue == -1)
                    batteryValue = 100;
                return batteryValue;
            }
            set
            {
                if (batteryValue != value)
                {
                    batteryValue = value;
                    NotifyPropertyChanged(() => BatteryValue);

                    if (batteryValue % 10 == 0)
                        ExecuteBatteryChange();
                }
            }
        }

        public ControllerViewModel(ISendService sendService, ServiceInstaller serviceInstaller, string controllerId, string controllerName, ConnectionTypes connection)
        {
            this.sp = sendService;
            this.si = serviceInstaller;

            this.BatteryValue = -1;
            this.ControllerId = controllerId;
            this.ControllerName = controllerName;
            this.ControllerUsbChecked = connection == ConnectionTypes.USB;
            this.ControllerBtChecked = connection == ConnectionTypes.Bluetooth;

            if (ControllerUsbChecked)
            {
                ExecuteUsbChange();
            }
            else
            {
                ExecuteBtChange();
            }
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
                    controllerUsb = new DelegateCommand(param => ExecuteUsbChange(), param => CanExecuteControllerUsb);
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
                    controllerBt = new DelegateCommand(param => ExecuteBtChange(), param => CanExecuteControllerBt);
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

        private void ExecuteBatteryChange()
        {
            ControllerContract ct = ControllerContract.Create(ControllerId, ControllerName, ControllerUsbChecked, ControllerBtChecked, BatteryValue, ControllerMessage.CONTROLLER_BATTERY_CHANGE);
            sp.SendCommand(ct);
        }
        private void ExecuteUsbChange()
        {
            ControllerContract ct;

            if (controllerUsbChecked)
                ct = ControllerContract.Create(ControllerId, ControllerName, ControllerUsbChecked, ControllerBtChecked, BatteryValue, ControllerMessage.CONTROLLER_CONNECT_USB);
            else
                ct = ControllerContract.Create(ControllerId, ControllerName, ControllerUsbChecked, ControllerBtChecked, BatteryValue, ControllerMessage.CONTROLLER_DISCONNECT_USB);
            
            sp.SendCommand(ct);

            if (ControllerUsbChecked == false && ControllerBtChecked == false)
                OnRequestRemove();
        }
        private void ExecuteBtChange()
        {
            ControllerContract ct;

            if (controllerBtChecked)
                ct = ControllerContract.Create(ControllerId, ControllerName, ControllerUsbChecked, ControllerBtChecked, BatteryValue, ControllerMessage.CONTROLLER_CONNECT_BT);
            else
                ct = ControllerContract.Create(ControllerId, ControllerName, ControllerUsbChecked, ControllerBtChecked, BatteryValue, ControllerMessage.CONTROLLER_DISCONNECT_BT);

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
