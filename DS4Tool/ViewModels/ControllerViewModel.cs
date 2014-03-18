using CommunicationLibrary;
using CoreLibrary;

namespace DS4Tool
{
    public class ControllerViewModel : ViewModelBase
    {
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

            App.AppManager.SetControllerIcon(ControllerContract.Create(this.Id, this.Name, this.IsUsbConnected, this.IsBluetoothConnected, this.BatteryValue, ShowIcon ?? false));
        }

        #endregion // Status

        #region Options

        private bool? showIcon;
        public bool? ShowIcon
        {
            get
            {
                if (showIcon == null)
                    showIcon = App.AppManager.GetControllerIcon(false, this.Name);
                return showIcon;
            }
            set
            {
                if (showIcon != value)
                {
                    showIcon = value;
                    NotifyPropertyChanged(() => ShowIcon);
                    App.AppManager.SetControllerIcon(ControllerContract.Create(this.Id, this.Name, this.IsUsbConnected, this.IsBluetoothConnected, this.BatteryValue, value ?? false));
                }
            }
        }

        #endregion

        #region Ctor

        public ControllerViewModel(ControllerContract controller)
        {
            Id = controller.Id;
            Name = controller.Name;
            IsUsbConnected = controller.IsUsbConnected;
            IsBluetoothConnected = controller.IsBluetoothConnected;
            BatteryValue = controller.BatteryValue;

            ChangeStatus(controller);
        }

        #endregion // Ctor
    }
}
