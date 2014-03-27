using CommunicationLibrary;
using ControllerConfigurationLibrary;
using CoreLibrary;
using NotifyIconLibrary;

namespace DS4Tool
{
    public class ControllerViewModel : ViewModelBase
    {
        #region dependency

        INotifyIconManager IconManager;
        IControllerConfigurationManager ControllerConfigurationManager;

        #endregion

        #region Status

        private int id;
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if (id != value)
                {
                    id = value;
                    NotifyPropertyChanged(() => Id);
                }
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged(() => Name);                    
                }
            }
        }

        private bool isUsbConnected;
        public bool IsUsbConnected
        {
            get
            {
                return isUsbConnected;
            }
            set
            {
                if (isUsbConnected != value)
                {
                    isUsbConnected = value;
                    NotifyPropertyChanged(() => IsUsbConnected);                    
                }
            }
        }

        private bool isBluetoothConnected;
        public bool IsBluetoothConnected
        {
            get
            {
                return isBluetoothConnected;
            }
            set
            {
                if (isBluetoothConnected != value)
                {
                    isBluetoothConnected = value;                    
                    NotifyPropertyChanged(() => IsBluetoothConnected);
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
                }
            }
        }

        private string status;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                if ( status != value)
                {
                    status = value;
                    NotifyPropertyChanged(() => Status);
                }
            }
        }

        public void ChangeStatus(ControllerContract controller)
        {
            IsUsbConnected = controller.IsUsbConnected;
            IsBluetoothConnected = controller.IsBluetoothConnected;
            BatteryValue = controller.BatteryValue;

            if (IsUsbConnected)
                Status = "Charging ...";
            else
                Status = "Battery " + BatteryValue + "%";

            IconManager.SetIcon(Id, IsUsbConnected, BatteryValue, ShowIcon ?? false);
        }

        #endregion // Status

        #region Options

        private bool? showIcon;
        public bool? ShowIcon
        {
            get
            {
                if (showIcon == null)
                    showIcon = bool.Parse(ControllerConfigurationManager.GetData(ControllerOptions.SHOW_ICON, Name));
                return showIcon;
            }
            set
            {
                if (showIcon != value)
                {
                    showIcon = value;
                    NotifyPropertyChanged(() => ShowIcon);

                    ControllerConfigurationManager.SetData(ControllerOptions.SHOW_ICON, Name, (value ?? false).ToString());
                    IconManager.SetIcon(Id, IsUsbConnected, BatteryValue, value ?? false);
                }
            }
        }

        #endregion

        #region Ctor

        public ControllerViewModel(INotifyIconManager iconManager, IControllerConfigurationManager controllerConfigurationManager, ControllerContract controller)
        {
            IconManager = iconManager;
            ControllerConfigurationManager = controllerConfigurationManager;

            Id = controller.Id;
            Name = controller.Name;
            
            ChangeStatus(controller);
        }

        #endregion // Ctor
    }
}
