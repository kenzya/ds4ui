using System;
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
        ISubscribingService SubscribingService;

        #endregion

        #region Status

        private string id;
        public string Id
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

            IconManager.SetIcon(Id, IsUsbConnected, BatteryValue, TuningIcon ?? false);
        }

        #endregion // Status

        #region Light Tab

        private int? lightRed;
        public int? LightRed
        {
            get
            {
                if (lightRed == null)
                    lightRed = int.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.LIGHT_RED));
                return lightRed;
            }
            set
            {
                if (lightRed != value)
                {
                    lightRed = value;
                    NotifyPropertyChanged(() => LightRed);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.LIGHT_RED, (value ?? 0).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, new Tuple<int, int, int>(value ?? 0, LightGreen ?? 0, LightBlue ?? 0), ControllerMessage.CONTROLLER_LED_COLOR));
                }
            }
        }

        private int? lightGreen;
        public int? LightGreen
        {
            get
            {
                if (lightGreen == null)
                    lightGreen = int.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.LIGHT_GREEN));
                return lightGreen;
            }
            set
            {
                if (lightGreen != value)
                {
                    lightGreen = value;
                    NotifyPropertyChanged(() => LightGreen);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.LIGHT_GREEN, (value ?? 0).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, new Tuple<int, int,int>(LightRed ?? 0, value ?? 0, LightBlue ?? 0), ControllerMessage.CONTROLLER_LED_COLOR));
                }
            }
        }

        private int? lightBlue;
        public int? LightBlue
        {
            get
            {
                if (lightBlue == null)
                    lightBlue = int.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.LIGHT_BLUE));
                return lightBlue;
            }
            set
            {
                if (lightBlue != value)
                {
                    lightBlue = value;
                    NotifyPropertyChanged(() => LightBlue);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.LIGHT_BLUE, (value ?? 0).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, new Tuple<int, int, int>(LightRed ?? 0, LightGreen ?? 0, value ?? 0), ControllerMessage.CONTROLLER_LED_COLOR));
                }
            }
        }

        private bool? lightFlash;
        public bool? LightFlash
        {
            get
            {
                if (lightFlash == null)
                    lightFlash = bool.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.LIGHT_FLASH));
                return lightFlash;
            }
            set
            {
                if (lightFlash != value)
                {
                    lightFlash = value;
                    NotifyPropertyChanged(() => LightFlash);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.LIGHT_FLASH, (value ?? false).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? false, ControllerMessage.CONTROLLER_LED_FLASH));
                }
            }
        }

        private bool? lightBattery;
        public bool? LightBattery
        {
            get
            {
                if (lightBattery == null)
                    lightBattery = bool.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.LIGHT_BATTERY));
                return lightBattery;
            }
            set
            {
                if (lightBattery != value)
                {
                    lightBattery = value;
                    NotifyPropertyChanged(() => LightBattery);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.LIGHT_BATTERY, (value ?? false).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? false, ControllerMessage.CONTROLLER_LED_BATTERY));
                }
            }
        }

        private bool? lightDischarge;
        public bool? LightDischarge
        {
            get
            {
                if (lightDischarge == null)
                    lightDischarge = bool.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.LIGHT_DISCHARGE));
                return lightDischarge;
            }
            set
            {
                if (lightDischarge != value)
                {
                    lightDischarge = value;
                    NotifyPropertyChanged(() => LightDischarge);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.LIGHT_DISCHARGE, (value ?? false).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? false, ControllerMessage.CONTROLLER_LED_DISCHARGE));
                }
            }
        }

        #endregion // Light Tab

        #region Touch Tab

        private int? touchValue;
        public int? TouchValue
        {
            get
            {
                if (touchValue == null)
                    touchValue = int.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.TOUCH_VALUE));
                return touchValue;
            }
            set
            {
                if (touchValue != value)
                {
                    touchValue = value;
                    NotifyPropertyChanged(() => TouchValue);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.TOUCH_VALUE, (value ?? 10).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? 10, ControllerMessage.CONTROLLER_TOUCH_VALUE));
                }
            }
        }

        private int? touchTap;
        public int? TouchTap
        {
            get
            {
                if (touchTap == null)
                    touchTap = int.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.TOUCH_TAP));
                return touchTap;
            }
            set
            {
                if (touchTap != value)
                {
                    touchTap = value;
                    NotifyPropertyChanged(() => TouchTap);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.TOUCH_TAP, (value ?? 0).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? 10, ControllerMessage.CONTROLLER_TOUCH_TAP));
                }
            }
        }

        private int? touchScroll;
        public int? TouchScroll
        {
            get
            {
                if (touchScroll == null)
                    touchScroll = int.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.TOUCH_SCROLL));
                return touchScroll;
            }
            set
            {
                if (touchScroll != value)
                {
                    touchScroll = value;
                    NotifyPropertyChanged(() => TouchScroll);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.TOUCH_SCROLL, (value ?? 0).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? 10, ControllerMessage.CONTROLLER_TOUCH_SCROLL));
                }
            }
        }

        private bool? touchStartup;
        public bool? TouchStartup
        {
            get
            {
                if (touchStartup == null)
                    touchStartup = bool.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.TOUCH_STARTUP));
                return touchStartup;
            }
            set
            {
                if (touchStartup != value)
                {
                    touchStartup = value;
                    NotifyPropertyChanged(() => TouchStartup);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.TOUCH_STARTUP, (value ?? false).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? false, ControllerMessage.CONTROLLER_TOUCH_STARTUP));
                }
            }
        }

        private bool? touchJitter;
        public bool? TouchJitter
        {
            get
            {
                if (touchJitter == null)
                    touchJitter = bool.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.TOUCH_JITTER));
                return touchJitter;
            }
            set
            {
                if (touchJitter != value)
                {
                    touchJitter = value;
                    NotifyPropertyChanged(() => TouchJitter);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.TOUCH_JITTER, (value ?? false).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? false, ControllerMessage.CONTROLLER_TOUCH_JITTER));
                }
            }
        }

        private bool? touchRight;
        public bool? TouchRight
        {
            get
            {
                if (touchRight == null)
                    touchRight = bool.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.TOUCH_RIGHT));
                return touchRight;
            }
            set
            {
                if (touchRight != value)
                {
                    touchRight = value;
                    NotifyPropertyChanged(() => TouchRight);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.TOUCH_RIGHT, (value ?? false).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? false, ControllerMessage.CONTROLLER_TOUCH_RIGHT));
                }
            }
        }

        #endregion // Touch Tab

        #region Rumble Tab

        private int? rumbleBoost;
        public int? RumbleBoost
        {
            get
            {
                if (rumbleBoost == null)
                    rumbleBoost = int.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.RUMBLE_BOOST));
                return rumbleBoost;
            }
            set
            {
                if (rumbleBoost != value)
                {
                    rumbleBoost = value;
                    NotifyPropertyChanged(() => RumbleBoost);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.RUMBLE_BOOST, (value ?? 0).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? 0, ControllerMessage.CONTROLLER_RUMBLE_BOOST));
                }
            }
        }

        private int? rumbleHeavy;
        public int? RumbleHeavy
        {
            get
            {
                if (rumbleHeavy == null)
                    rumbleHeavy = int.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.RUMBLE_HEAVY));
                return rumbleHeavy;
            }
            set
            {
                if (rumbleHeavy != value)
                {
                    rumbleHeavy = value;
                    NotifyPropertyChanged(() => RumbleHeavy);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.RUMBLE_HEAVY, (value ?? 0).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? 0, ControllerMessage.CONTROLLER_RUMBLE_HEAVY));
                }
            }
        }

        private int? rumbleLight;
        public int? RumbleLight
        {
            get
            {
                if (rumbleLight == null)
                    rumbleLight = int.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.RUMBLE_LIGHT));
                return rumbleLight;
            }
            set
            {
                if (rumbleLight != value)
                {
                    rumbleLight = value;
                    NotifyPropertyChanged(() => RumbleLight);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.RUMBLE_LIGHT, (value ?? 0).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? 0, ControllerMessage.CONTROLLER_RUMBLE_LIGHT));
                }
            }
        }

        private bool? rumbleSwap;
        public bool? RumbleSwap
        {
            get
            {
                if (rumbleSwap == null)
                    rumbleSwap = bool.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.RUMBLE_SWAP));
                return rumbleSwap;
            }
            set
            {
                if (rumbleSwap != value)
                {
                    rumbleSwap = value;
                    NotifyPropertyChanged(() => RumbleSwap);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.RUMBLE_SWAP, (value ?? false).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? false, ControllerMessage.CONTROLLER_RUMBLE_SWAP));
                }
            }
        }

        #endregion // Touchpad Tab

        #region Tuning Tab

        private int? tuningLeft;
        public int? TuningLeft
        {
            get
            {
                if (tuningLeft == null)
                    tuningLeft = int.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.TUNING_LEFT));
                return tuningLeft;
            }
            set
            {
                if (tuningLeft != value)
                {
                    tuningLeft = value;
                    NotifyPropertyChanged(() => TuningLeft);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.TUNING_LEFT, (value ?? 0).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? 0, ControllerMessage.CONTROLLER_TUNING_LEFT));
                }
            }
        }

        private int? tuningRight;
        public int? TuningRight
        {
            get
            {
                if (tuningRight == null)
                    tuningRight = int.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.TUNING_RIGHT));
                return tuningRight;
            }
            set
            {
                if (tuningRight != value)
                {
                    tuningRight = value;
                    NotifyPropertyChanged(() => TuningRight);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.TUNING_RIGHT, (value ?? 0).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? 0, ControllerMessage.CONTROLLER_TUNING_RIGHT));
                }
            }
        }

        private int? tuningIdle;
        public int? TuningIdle
        {
            get
            {
                if (tuningIdle == null)
                    tuningIdle = int.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.TUNING_IDLE));
                return tuningIdle;
            }
            set
            {
                if (tuningIdle != value)
                {
                    tuningIdle = value;
                    NotifyPropertyChanged(() => TuningIdle);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.TUNING_IDLE, (value ?? 0).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? 0, ControllerMessage.CONTROLLER_TUNING_IDLE));
                }
            }
        }

        private bool? tuningRealtime;
        public bool? TuningRealtime
        {
            get
            {
                if (tuningRealtime == null)
                    tuningRealtime = bool.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.TUNING_REALTIME));
                return tuningRealtime;
            }
            set
            {
                if (tuningRealtime != value)
                {
                    tuningRealtime = value;
                    NotifyPropertyChanged(() => TuningRealtime);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.TUNING_REALTIME, (value ?? false).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? false, ControllerMessage.CONTROLLER_TUNING_REALTIME));
                }
            }
        }

        private bool? tuningFlush;
        public bool? TuningFlush
        {
            get
            {
                if (tuningFlush == null)
                    tuningFlush = bool.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.TUNING_FLUSH));
                return tuningFlush;
            }
            set
            {
                if (tuningFlush != value)
                {
                    tuningFlush = value;
                    NotifyPropertyChanged(() => TuningFlush);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.TUNING_FLUSH, (value ?? false).ToString());
                    IconManager.SetIcon(Id, IsUsbConnected, BatteryValue, value ?? false);
                }
            }
        }

        private bool? tuningGyro;
        public bool? TuningGyro
        {
            get
            {
                if (tuningGyro == null)
                    tuningGyro = bool.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.TUNING_GYRO));
                return tuningGyro;
            }
            set
            {
                if (tuningGyro != value)
                {
                    tuningGyro = value;
                    NotifyPropertyChanged(() => TuningGyro);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.TUNING_GYRO, (value ?? false).ToString());
                    SubscribingService.SendCommand(ControllerContract.Create(Id, value ?? false, ControllerMessage.CONTROLLER_TUNING_GYRO));
                }
            }
        }

        private bool? tuningIcon;
        public bool? TuningIcon
        {
            get
            {
                if (tuningIcon == null)
                    tuningIcon = bool.Parse(ControllerConfigurationManager.GetData(Id, ControllerOptions.TUNING_ICON));
                return tuningIcon;
            }
            set
            {
                if (tuningIcon != value)
                {
                    tuningIcon = value;
                    NotifyPropertyChanged(() => TuningIcon);

                    ControllerConfigurationManager.SetData(Id, ControllerOptions.TUNING_ICON, (value ?? false).ToString());
                    IconManager.SetIcon(Id, IsUsbConnected, BatteryValue, value ?? false);
                }
            }
        }

        #endregion //Tuning Tab

        #region Ctor

        public ControllerViewModel(INotifyIconManager iconManager, IControllerConfigurationManager controllerConfigurationManager, ISubscribingService subscribingService,
            ControllerContract controller)
        {
            IconManager = iconManager;
            ControllerConfigurationManager = controllerConfigurationManager;
            SubscribingService = subscribingService;

            Id = controller.Id;
            Name = controller.Name;
            
            ChangeStatus(controller);
        }

        #endregion // Ctor
    }
}
