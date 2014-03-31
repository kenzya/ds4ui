﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using HidLibrary;

namespace DS4Library
{
    public struct DS4Color
    {
        public byte red;
        public byte green;
        public byte blue;
    }

    public enum ConnectionType : byte { BT, USB }; // Prioritize Bluetooth when both are connected.

    /**
     * The haptics engine uses a stack of these states representing the light bar and rumble motor settings.
     * It (will) handle composing them and the details of output report management.
     */
    public struct DS4HapticState
    {
        public DS4Color LightBarColor;
        public bool LightBarExplicitlyOff;
        public byte LightBarFlashDurationOn, LightBarFlashDurationOff;
        public byte RumbleMotorStrengthLeftHeavySlow, RumbleMotorStrengthRightLightFast;
        public bool RumbleMotorsExplicitlyOff;
        public bool IsLightBarSet()
        {
            return LightBarExplicitlyOff || LightBarColor.red != 0 || LightBarColor.green != 0 || LightBarColor.blue != 0;
        }
        public bool IsRumbleSet()
        {
            return RumbleMotorsExplicitlyOff || RumbleMotorStrengthLeftHeavySlow != 0 || RumbleMotorStrengthRightLightFast != 0;
        }
    }

    public class DS4Device
    {
        private const int BT_OUTPUT_REPORT_LENGTH = 78;
        private const int BT_INPUT_REPORT_LENGTH = 547;
        private HidDevice hDevice;
        private string Mac;
        private DS4State cState = new DS4State();
        private DS4State pState = new DS4State();
        private ConnectionType conType;
        private byte[] accel = new byte[6];
        private byte[] gyro = new byte[6];
        private byte[] inputReport;
        private byte[] btInputReport = null;
        private byte[] outputReportBuffer, outputReport;
        private readonly DS4Touchpad touchpad = null;
        private byte rightLightFastRumble;
        private byte leftHeavySlowRumble;
        private DS4Color ligtBarColor;
        private byte ledFlashOn, ledFlashOff;
        private Thread ds4Input, ds4Output;
        private int battery;
        private bool charging;
        public event EventHandler<EventArgs> Report = null;
        public event EventHandler<EventArgs> Removal = null;

        public HidDevice HidDevice { get { return hDevice; } }
        public bool IsExclusive { get { return HidDevice.IsExclusive; } }
        public bool IsDisconnecting { get; private set; }

        public string MacAddress { get { return Mac; } }

        public ConnectionType ConnectionType { get { return conType; } }

        public int Battery { get { return battery; } }
        public bool Charging { get { return charging; } }

        public byte RightLightFastRumble
        {
            get { return rightLightFastRumble; }
            set
            {
                if (value == rightLightFastRumble) return;
                rightLightFastRumble = value;
            }
        }

        public byte LeftHeavySlowRumble
        {
            get { return leftHeavySlowRumble; }
            set
            {
                if (value == leftHeavySlowRumble) return;
                leftHeavySlowRumble = value;
            }
        }

        public DS4Color LightBarColor
        {
            get { return ligtBarColor; }
            set
            {
                if (ligtBarColor.red != value.red || ligtBarColor.green != value.green || ligtBarColor.blue != value.blue)
                {
                    ligtBarColor = value;
                }
            }
        }

        public byte LightBarOnDuration
        {
            get { return ledFlashOn; }
            set
            {
                if (ledFlashOn != value)
                {
                    ledFlashOn = value;
                }
            }
        }

        public byte LightBarOffDuration
        {
            get { return ledFlashOff; }
            set
            {
                if (ledFlashOff != value)
                {
                    ledFlashOff = value;
                }
            }
        }

        public DS4Touchpad Touchpad { get { return touchpad; } }

        public static ConnectionType HidConnectionType(HidDevice hidDevice)
        {
            return hidDevice.Capabilities.InputReportByteLength == 64 ? ConnectionType.USB : ConnectionType.BT;
        }

        public DS4Device(HidDevice hidDevice)
        {
            hDevice = hidDevice;
            hDevice.MonitorDeviceEvents = true;
            conType = HidConnectionType(hDevice);
            Mac = hDevice.readSerial();
            if (conType == ConnectionType.USB)
            {
                inputReport = new byte[64];
                outputReport = new byte[hDevice.Capabilities.OutputReportByteLength];
                outputReportBuffer = new byte[hDevice.Capabilities.OutputReportByteLength];
            }
            else
            {
                btInputReport = new byte[BT_INPUT_REPORT_LENGTH];
                inputReport = new byte[btInputReport.Length - 2];
                outputReport = new byte[BT_OUTPUT_REPORT_LENGTH];
                outputReportBuffer = new byte[BT_OUTPUT_REPORT_LENGTH];
            }
            touchpad = new DS4Touchpad();
        }

        public void StartUpdate()
        {
            if (ds4Input == null)
            {
                Console.WriteLine(MacAddress.ToString() + " " + System.DateTime.UtcNow.ToString("o") + "> start");
                sendOutputReport(true); // request the particular kind of input report we want
                ds4Output = new Thread(performDs4Output);
                ds4Output.Name = "DS4 Output thread: " + Mac;
                ds4Output.Start();
                ds4Input = new Thread(performDs4Input);
                ds4Input.Name = "DS4 Input thread: " + Mac;
                ds4Input.Start();
            }
            else
                Console.WriteLine("Thread already running for DS4: " + Mac);
        }

        public void StopUpdate()
        {
            if (ds4Input.ThreadState != System.Threading.ThreadState.Stopped || ds4Input.ThreadState != System.Threading.ThreadState.Aborted)
            {
                try
                {
                    ds4Input.Abort();
                    ds4Input.Join();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            StopOutputUpdate();
        }

        private void StopOutputUpdate()
        {
            if (ds4Output.ThreadState != System.Threading.ThreadState.Stopped || ds4Output.ThreadState != System.Threading.ThreadState.Aborted)
            {
                try
                {
                    ds4Output.Abort();
                    ds4Output.Join();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private bool writeOutput()
        {
            if (conType == ConnectionType.BT)
            {
                return hDevice.WriteOutputReportViaControl(outputReport);
            }
            else
            {
                return hDevice.WriteOutputReportViaInterrupt(outputReport, 8);
            }
        }

        private void performDs4Output()
        {
            lock (outputReport)
            {
                while (writeOutput())
                {
                    if (testRumble.IsRumbleSet()) // repeat test rumbles periodically; rumble has auto-shut-off in the DS4 firmware
                        Monitor.Wait(outputReport, 10000); // DS4 firmware stops it after 5 seconds, so let the motors rest for that long, too.
                    else
                        Monitor.Wait(outputReport);
                }
            }
        }

        /** Is the device alive and receiving valid sensor input reports? */
        public bool IsAlive()
        {
            return priorInputReport30 != 0xff;
        }
        private byte priorInputReport30 = 0xff;
        private void performDs4Input()
        {
            while (true)
            {
                if (conType != ConnectionType.USB)
                    if (hDevice.ReadFile(btInputReport) == HidDevice.ReadStatus.Success)
                    {
                        Array.Copy(btInputReport, 2, inputReport, 0, inputReport.Length);
                    }
                    else
                    {
                        Console.WriteLine(MacAddress.ToString() + " " + System.DateTime.UtcNow.ToString("o") + "> disconnect");
                        sendOutputReport(true); // Kick Windows into noticing the disconnection.
                        StopOutputUpdate();
                        if (!IsDisconnecting && Removal != null)
                            Removal(this, EventArgs.Empty);
                        return;

                    }
                else if (hDevice.ReadFile(inputReport) != HidDevice.ReadStatus.Success)
                {
                    StopOutputUpdate();
                    if (!IsDisconnecting && Removal != null)
                        Removal(this, EventArgs.Empty);
                    return;
                }
                if (ConnectionType == ConnectionType.BT && btInputReport[0] != 0x11)
                {
                    //Received incorrect report, skip it
                    continue;
                }
                DateTime utcNow = System.DateTime.UtcNow; // timestamp with UTC in case system time zone changes
                resetHapticState();
                if (cState == null)
                    cState = new DS4State();
                cState.ReportTimeStamp = utcNow;
                cState.LX = inputReport[1];
                cState.LY = inputReport[2];
                cState.RX = inputReport[3];
                cState.RY = inputReport[4];
                cState.L2 = inputReport[8];
                cState.R2 = inputReport[9];

                cState.Triangle = ((byte)inputReport[5] & (1 << 7)) != 0;
                cState.Circle = ((byte)inputReport[5] & (1 << 6)) != 0;
                cState.Cross = ((byte)inputReport[5] & (1 << 5)) != 0;
                cState.Square = ((byte)inputReport[5] & (1 << 4)) != 0;
                cState.DpadUp = ((byte)inputReport[5] & (1 << 3)) != 0;
                cState.DpadDown = ((byte)inputReport[5] & (1 << 2)) != 0;
                cState.DpadLeft = ((byte)inputReport[5] & (1 << 1)) != 0;
                cState.DpadRight = ((byte)inputReport[5] & (1 << 0)) != 0;

                //Convert dpad into individual On/Off bits instead of a clock representation
                byte dpad_state = 0;

                dpad_state = (byte)(
                ((cState.DpadRight ? 1 : 0) << 0) |
                ((cState.DpadLeft ? 1 : 0) << 1) |
                ((cState.DpadDown ? 1 : 0) << 2) |
                ((cState.DpadUp ? 1 : 0) << 3));

                switch (dpad_state)
                {
                    case 0: cState.DpadUp = true; cState.DpadDown = false; cState.DpadLeft = false; cState.DpadRight = false; break;
                    case 1: cState.DpadUp = true; cState.DpadDown = false; cState.DpadLeft = false; cState.DpadRight = true; break;
                    case 2: cState.DpadUp = false; cState.DpadDown = false; cState.DpadLeft = false; cState.DpadRight = true; break;
                    case 3: cState.DpadUp = false; cState.DpadDown = true; cState.DpadLeft = false; cState.DpadRight = true; break;
                    case 4: cState.DpadUp = false; cState.DpadDown = true; cState.DpadLeft = false; cState.DpadRight = false; break;
                    case 5: cState.DpadUp = false; cState.DpadDown = true; cState.DpadLeft = true; cState.DpadRight = false; break;
                    case 6: cState.DpadUp = false; cState.DpadDown = false; cState.DpadLeft = true; cState.DpadRight = false; break;
                    case 7: cState.DpadUp = true; cState.DpadDown = false; cState.DpadLeft = true; cState.DpadRight = false; break;
                    case 8: cState.DpadUp = false; cState.DpadDown = false; cState.DpadLeft = false; cState.DpadRight = false; break;
                }

                cState.R3 = ((byte)inputReport[6] & (1 << 7)) != 0;
                cState.L3 = ((byte)inputReport[6] & (1 << 6)) != 0;
                cState.Options = ((byte)inputReport[6] & (1 << 5)) != 0;
                cState.Share = ((byte)inputReport[6] & (1 << 4)) != 0;
                cState.R1 = ((byte)inputReport[6] & (1 << 1)) != 0;
                cState.L1 = ((byte)inputReport[6] & (1 << 0)) != 0;

                cState.PS = ((byte)inputReport[7] & (1 << 0)) != 0;
                cState.TouchButton = (inputReport[7] & (1 << 2 - 1)) != 0;
                cState.FrameCounter = (byte)(inputReport[7] >> 2);

                // Store Gyro and Accel values
                Array.Copy(inputReport, 14, accel, 0, 6);
                Array.Copy(inputReport, 20, gyro, 0, 6);

                charging = (inputReport[30] & 0x10) != 0;
                battery = (inputReport[30] & 0x0f) * 10;
                cState.Battery = (byte)battery;
                if (inputReport[30] != priorInputReport30)
                {
                    priorInputReport30 = inputReport[30];
                    Console.WriteLine(MacAddress.ToString() + " " + System.DateTime.UtcNow.ToString("o") + "> power subsystem octet: 0x" + inputReport[30].ToString("x02"));
                }

                // XXX DS4State mapping needs fixup, turn touches into an array[4] of structs.  And include the touchpad details there instead.
                for (int touches = inputReport[-1 + DS4Touchpad.TOUCHPAD_DATA_OFFSET - 1], touchOffset = 0; touches > 0; touches--, touchOffset += 9)
                {
                    cState.TouchPacketCounter = inputReport[-1 + DS4Touchpad.TOUCHPAD_DATA_OFFSET + touchOffset];
                    cState.Touch1 = (inputReport[0 + DS4Touchpad.TOUCHPAD_DATA_OFFSET + touchOffset] >> 7) != 0 ? false : true; // >= 1 touch detected
                    cState.Touch1Identifier = (byte)(inputReport[0 + DS4Touchpad.TOUCHPAD_DATA_OFFSET + touchOffset] & 0x7f);
                    cState.Touch2 = (inputReport[4 + DS4Touchpad.TOUCHPAD_DATA_OFFSET + touchOffset] >> 7) != 0 ? false : true; // 2 touches detected
                    cState.Touch2Identifier = (byte)(inputReport[4 + DS4Touchpad.TOUCHPAD_DATA_OFFSET + touchOffset] & 0x7f);
                    // Even when idling there is still a touch packet indicating no touch 1 or 2
                    touchpad.handleTouchpad(inputReport, cState, touchOffset);
                }

                /* Debug output of incoming HID data:
                if (cState.L2 == 0xff && cState.R2 == 0xff)
                {
                    Console.Write(MacAddress.ToString() + " " + System.DateTime.UtcNow.ToString("o") + ">");
                    for (int i = 0; i < inputReport.Length; i++)
                        Console.Write(" " + inputReport[i].ToString("x2"));
                    Console.WriteLine();
                } */

                if (conType == ConnectionType.BT && (!pState.PS || !pState.Options) && cState.PS && cState.Options)
                {
                    if (DisconnectBT())
                        return; // all done
                }
                // XXX fix initialization ordering so the null checks all go away
                if (Report != null)
                    Report(this, EventArgs.Empty);
                sendOutputReport(false);

                if (pState == null)
                    pState = new DS4State();
                cState.Copy(pState);
            }
        }

        private void sendOutputReport(bool synchronous)
        {
            setTestRumble();
            setHapticState();
            if (conType == ConnectionType.BT)
            {
                outputReportBuffer[0] = 0x11;
                outputReportBuffer[1] = 0x80;
                outputReportBuffer[3] = 0xff;
                outputReportBuffer[6] = rightLightFastRumble; //fast motor
                outputReportBuffer[7] = leftHeavySlowRumble; //slow motor
                outputReportBuffer[8] = LightBarColor.red; //red
                outputReportBuffer[9] = LightBarColor.green; //green
                outputReportBuffer[10] = LightBarColor.blue; //blue
                outputReportBuffer[11] = ledFlashOn; //flash on duration
                outputReportBuffer[12] = ledFlashOff; //flash off duration
            }
            else
            {
                outputReportBuffer[0] = 0x05;
                outputReportBuffer[1] = 0xff;
                outputReportBuffer[4] = rightLightFastRumble; //fast motor
                outputReportBuffer[5] = leftHeavySlowRumble; //slow  motor
                outputReportBuffer[6] = LightBarColor.red; //red
                outputReportBuffer[7] = LightBarColor.green; //green
                outputReportBuffer[8] = LightBarColor.blue; //blue
                outputReportBuffer[9] = ledFlashOn; //flash on duration
                outputReportBuffer[10] = ledFlashOff; //flash off duration
            }
            lock (outputReport)
            {
                if (synchronous)
                {
                    outputReportBuffer.CopyTo(outputReport, 0);
                    try
                    {
                        writeOutput();
                    }
                    catch
                    {
                        // If it's dead already, don't worry about it.
                    }
                }
                else
                {
                    bool output = false;
                    for (int i = 0; !output && i < outputReport.Length; i++)
                        output = outputReport[i] != outputReportBuffer[i];
                    if (output)
                    {
                        outputReportBuffer.CopyTo(outputReport, 0);
                        Monitor.Pulse(outputReport);
                    }
                }
            }
        }

        public bool DisconnectBT()
        {
            if (Mac != null)
            {
                Console.WriteLine("Trying to disonnect BT device");
                IntPtr btHandle = IntPtr.Zero;
                int IOCTL_BTH_DISCONNECT_DEVICE = 0x41000c;

                byte[] btAddr = new byte[8];
                string[] sbytes = Mac.Split(':');
                for (int i = 0; i < 6; i++)
                {
                    //parse hex byte in reverse order
                    btAddr[5 - i] = Convert.ToByte(sbytes[i], 16);
                }
                long lbtAddr = BitConverter.ToInt64(btAddr, 0);

                NativeMethods.BLUETOOTH_FIND_RADIO_PARAMS p = new NativeMethods.BLUETOOTH_FIND_RADIO_PARAMS();
                p.dwSize = Marshal.SizeOf(typeof(NativeMethods.BLUETOOTH_FIND_RADIO_PARAMS));
                IntPtr searchHandle = NativeMethods.BluetoothFindFirstRadio(ref p, ref btHandle);
                int bytesReturned = 0;
                bool success = false;
                while (!success && btHandle != IntPtr.Zero)
                {
                    success = NativeMethods.DeviceIoControl(btHandle, IOCTL_BTH_DISCONNECT_DEVICE, ref lbtAddr, 8, IntPtr.Zero, 0, ref bytesReturned, IntPtr.Zero);
                    NativeMethods.CloseHandle(btHandle);
                    if (!success)
                        if (!NativeMethods.BluetoothFindNextRadio(searchHandle, ref btHandle))
                            btHandle = IntPtr.Zero;

                }
                NativeMethods.BluetoothFindRadioClose(searchHandle);
                Console.WriteLine("Disconnect successful: " + success);
                success = true; // XXX return value indicates failure, but it still works?
                if (success)
                {
                    IsDisconnecting = true;
                    StopOutputUpdate();
                    if (Removal != null)
                        Removal(this, EventArgs.Empty);
                }
                return success;
            }
            return false;
        }

        private DS4HapticState testRumble = new DS4HapticState();
        public void setRumble(byte rightLightFastMotor, byte leftHeavySlowMotor)
        {
            testRumble.RumbleMotorStrengthRightLightFast = rightLightFastMotor;
            testRumble.RumbleMotorStrengthLeftHeavySlow = leftHeavySlowMotor;
            testRumble.RumbleMotorsExplicitlyOff = rightLightFastMotor == 0 && leftHeavySlowMotor == 0;
        }

        private void setTestRumble()
        {
            if (testRumble.IsRumbleSet())
            {
                pushHapticState(testRumble);
                if (testRumble.RumbleMotorsExplicitlyOff)
                    testRumble.RumbleMotorsExplicitlyOff = false;
            }
        }

        public DS4State getCurrentState()
        {
            return cState.Clone();
        }

        public DS4State getPreviousState()
        {
            return pState.Clone();
        }

        public void getExposedState(DS4StateExposed expState, DS4State state)
        {
            cState.Copy(state);
            expState.Accel = accel;
            expState.Gyro = gyro;
        }

        public void getCurrentState(DS4State state)
        {
            cState.Copy(state);
        }

        public void getPreviousState(DS4State state)
        {
            pState.Copy(state);
        }

        private DS4HapticState[] hapticState = new DS4HapticState[1];
        private int hapticStackIndex = 0;
        private void resetHapticState()
        {
            hapticStackIndex = 0;
        }

        // Use the "most recently set" haptic state for each of light bar/motor.
        private void setHapticState()
        {
            int i = 0;
            DS4Color lightBarColor = LightBarColor;
            byte lightBarFlashDurationOn = LightBarOnDuration, lightBarFlashDurationOff = LightBarOffDuration;
            byte rumbleMotorStrengthLeftHeavySlow = LeftHeavySlowRumble, rumbleMotorStrengthRightLightFast = rightLightFastRumble;
            foreach (DS4HapticState haptic in hapticState)
            {
                if (i++ == hapticStackIndex)
                    break; // rest haven't been used this time
                if (haptic.IsLightBarSet())
                {
                    lightBarColor = haptic.LightBarColor;
                    lightBarFlashDurationOn = haptic.LightBarFlashDurationOn;
                    lightBarFlashDurationOff = haptic.LightBarFlashDurationOff;
                }
                if (haptic.IsRumbleSet())
                {
                    rumbleMotorStrengthLeftHeavySlow = haptic.RumbleMotorStrengthLeftHeavySlow;
                    rumbleMotorStrengthRightLightFast = haptic.RumbleMotorStrengthRightLightFast;
                }
            }
            LightBarColor = lightBarColor;
            LightBarOnDuration = lightBarFlashDurationOn;
            LightBarOffDuration = lightBarFlashDurationOff;
            LeftHeavySlowRumble = rumbleMotorStrengthLeftHeavySlow;
            RightLightFastRumble = rumbleMotorStrengthRightLightFast;
        }

        public void pushHapticState(DS4HapticState hs)
        {
            if (hapticStackIndex == hapticState.Length)
            {
                DS4HapticState[] newHaptics = new DS4HapticState[hapticState.Length + 1];
                Array.Copy(hapticState, newHaptics, hapticState.Length);
                hapticState = newHaptics;
            }
            hapticState[hapticStackIndex++] = hs;
        }

        override
        public String ToString()
        {
            return Mac;
        }
    }
}