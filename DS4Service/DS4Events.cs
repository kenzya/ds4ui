using System;
using System.Diagnostics;
using CommunicationLibrary;
using DS4Control;

namespace DS4Service
{
    internal class DS4Events
    {
        internal DS4Events()
        { }

        internal void ChangeExclusive(ControllerContract param)
        {
            bool? value = param.Value as bool?;

            Process[] rundll32 = Process.GetProcessesByName("rundll32");
            foreach (Process rundll32Instance in rundll32)
            {
                foreach (ProcessModule module in rundll32Instance.Modules)
                {
                    if (module.FileName.Contains("joy.cpl"))
                    {
                        module.Dispose();
                    }
                }
            }

            Global.setUseExclusiveMode(value ?? false);
            Global.Save();
        }

        internal void ChangeLedColor(ControllerContract param)
        {
            string device = param.Id;
            Tuple<int,int,int> rgb = param.Value as Tuple<int,int,int>;

            Global.saveColor(int.Parse(device), rgb.Item1.ToByte(), rgb.Item2.ToByte(), rgb.Item3.ToByte());
        }

        internal void ChangeLedFlash(ControllerContract param)
        {
            string device = param.Id;
            bool? value = param.Value as bool?;

            Global.setFlashWhenLowBattery(int.Parse(device), value ?? false);
        }

        internal void ChangeLedBattery(ControllerContract param)
        {
            string device = param.Id;
            bool? value = param.Value as bool?;

            Global.setLedAsBatteryIndicator(int.Parse(device), value ?? false);
        }

        internal void ChangeTouchValue(ControllerContract param)
        {
            string device = param.Id;
            int? value = param.Value as int?;

            Global.setTouchSensitivity(int.Parse(device), value.ToByte());
        }

        internal void ChangeTouchTap(ControllerContract param)
        {
            string device = param.Id;
            int? value = param.Value as int?;

            Global.setTapSensitivity(int.Parse(device), value.ToByte());
        }

        internal void ChangeTouchScroll(ControllerContract param)
        {
            string device = param.Id;
            int? value = param.Value as int?;

            Global.setScrollSensitivity(int.Parse(device), value.ToByte());
        }

        internal void ChangeTouchStartup(ControllerContract param)
        {
            string device = param.Id;

            throw new NotImplementedException();
        }

        internal void ChangeTouchJitter(ControllerContract param)
        {
            string device = param.Id;
            bool? value = param.Value as bool?;

            Global.setTouchpadJitterCompensation(int.Parse(device), value ?? false);
        }

        internal void ChangeTouchRight(ControllerContract param)
        {
            string device = param.Id;
            bool? value = param.Value as bool?;

            Global.setLowerRCOff(int.Parse(device), !value ?? true);
        }

        internal void ChangeRumbleBoost(ControllerContract param)
        {
            string device = param.Id;
            int? value = param.Value as int?;

            Global.saveRumbleBoost(int.Parse(device), value.ToByte());
            //TODO: move setRumble to global
            //scpDevice.setRumble((byte)leftMotorBar.Value, (byte)rightMotorBar.Value, device);
        }

        internal void ChangeRumbleHeavy(ControllerContract param)
        {
            string device = param.Id;
            int? value = param.Value as int?;

            //TODO: move setRumble to global
            //scpDevice.setRumble((byte)leftMotorBar.Value, (byte)rightMotorBar.Value, device);
        }

        internal void ChangeRumbleLight(ControllerContract param)
        {
            string device = param.Id;
            int? value = param.Value as int?;

            //TODO: move setRumble to global
            //scpDevice.setRumble((byte)leftMotorBar.Value, (byte)rightMotorBar.Value, device);
        }

        internal void ChangeRumbleSwap(ControllerContract param)
        {
            string device = param.Id;
            bool? value = param.Value as bool?;

            //TODO: setRumbleSwap not found? Repo sync problem.
            //Global.setRumbleSwap(device, rumbleSwap.Checked);
        }

        internal void ChangeTuningLeft(ControllerContract param)
        {
            string device = param.Id;
            double? value = param.Value as double?;

            Global.setLeftTriggerMiddle(int.Parse(device), value ?? 0);    
        }

        internal void ChangeTuningRight(ControllerContract param)
        {
            string device = param.Id;
            double? value = param.Value as double?;

            Global.setRightTriggerMiddle(int.Parse(device), value ?? 0);    
        }

        internal void ChangeTuningIdle(ControllerContract param)
        {
            string device = param.Id;
            double? value = param.Value as double?;

            //TODO: repo sync, no method found.
            //Global.setIdleDisconnectTimeout(device, disconnectTimeout);
        }
    }
}
