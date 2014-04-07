using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using CommunicationLibrary;

namespace ServiceDestroyer
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string svcPath = Path.Combine(Environment.CurrentDirectory, Constants.SERVICE_NAME + ".exe");

            ServiceController sc = new ServiceController(Constants.SERVICE_NAME, Environment.MachineName);
            ServiceControllerPermission scp = new ServiceControllerPermission(ServiceControllerPermissionAccess.Control, Environment.MachineName, Constants.SERVICE_NAME);
            scp.Assert();
            sc.Refresh();
            ServiceInstaller si = new ServiceInstaller();

            if (si.DoesServiceExist(Constants.SERVICE_NAME))
            {
                if (sc.Status == ServiceControllerStatus.Running)
                    sc.Stop();

                sc.WaitForStatus(ServiceControllerStatus.Stopped);
                si.UnInstallService(Constants.SERVICE_NAME);
                MessageBox.Show("Service removed");
            }

            EventLog eventLog = new EventLog();
            eventLog.Source = Constants.SERVICE_NAME;
            eventLog.Log = "Application";
            if (!EventLog.SourceExists(eventLog.Source))
            {
                EventLog.DeleteEventSource(eventLog.Source, Environment.MachineName);
                MessageBox.Show("EventLog removed");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DS4Tool");

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                MessageBox.Show("Configuration deleted");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(Environment.CurrentDirectory, Constants.SERVICE_NAME + ".exe");
            ServiceInstaller si = new ServiceInstaller();
            si.InstallService(path, Constants.SERVICE_NAME);

            ServiceController sc = new ServiceController(Constants.SERVICE_NAME, Environment.MachineName);
            new ServiceControllerPermission(ServiceControllerPermissionAccess.Browse, Environment.MachineName, Constants.SERVICE_NAME).Assert();
            sc.Refresh();
            sc.WaitForStatus(ServiceControllerStatus.Running);
        }
    }

    public class ServiceInstaller
    {
        #region DLLImport

        [DllImport("advapi32.dll")]
        public static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, int scParameter);

        [DllImport("Advapi32.dll")]
        public static extern IntPtr CreateService(IntPtr SC_HANDLE, string lpSvcName, string lpDisplayName,
        int dwDesiredAccess, int dwServiceType, int dwStartType, int dwErrorControl, string lpPathName,
        string lpLoadOrderGroup, int lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);

        [DllImport("advapi32.dll")]
        public static extern void CloseServiceHandle(IntPtr SCHANDLE);

        [DllImport("advapi32.dll")]
        public static extern int StartService(IntPtr SVHANDLE, int dwNumServiceArgs, string lpServiceArgVectors);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr OpenService(IntPtr SCHANDLE, string lpSvcName, int dwNumServiceArgs);

        [DllImport("advapi32.dll")]
        public static extern int DeleteService(IntPtr SVHANDLE);

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        #endregion DLLImport

        public bool InstallService(string svcPath, string svcName)
        {
            int SC_MANAGER_CREATE_SERVICE = 0x0002;
            int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
            int SERVICE_ERROR_NORMAL = 0x00000001;
            int STANDARD_RIGHTS_REQUIRED = 0xF0000;
            int SERVICE_QUERY_CONFIG = 0x0001;
            int SERVICE_CHANGE_CONFIG = 0x0002;
            int SERVICE_QUERY_STATUS = 0x0004;
            int SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
            int SERVICE_START = 0x0010;
            int SERVICE_STOP = 0x0020;
            int SERVICE_PAUSE_CONTINUE = 0x0040;
            int SERVICE_INTERROGATE = 0x0080;
            int SERVICE_USER_DEFINED_CONTROL = 0x0100;
            int SERVICE_AUTO_START = 0x00000002;
            int SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG | SERVICE_QUERY_STATUS | SERVICE_ENUMERATE_DEPENDENTS |
                SERVICE_START | SERVICE_STOP | SERVICE_PAUSE_CONTINUE | SERVICE_INTERROGATE | SERVICE_USER_DEFINED_CONTROL);

            try
            {
                IntPtr sc_handle = OpenSCManager(null, null, SC_MANAGER_CREATE_SERVICE);
                if (sc_handle.ToInt32() != 0)
                {
                    IntPtr sv_handle = CreateService(sc_handle, svcName, svcName, SERVICE_ALL_ACCESS, SERVICE_WIN32_OWN_PROCESS,
                        SERVICE_AUTO_START, SERVICE_ERROR_NORMAL, svcPath, null, 0, null, null, null);
                    if (sv_handle.ToInt32() == 0)
                    {
                        CloseServiceHandle(sc_handle);
                        return false;
                    }
                    else
                    {
                        //now trying to start the service
                        int i = StartService(sv_handle, 0, null);
                        // If the value i is zero, then there was an error starting the service.
                        // note: error may arise if the service is already running or some other problem.
                        if (i == 0)
                        {
                            return false;
                        }

                        CloseServiceHandle(sc_handle);
                        return true;
                    }
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool UnInstallService(string svcName)
        {
            int GENERIC_WRITE = 0x40000000;
            IntPtr sc_hndl = OpenSCManager(null, null, GENERIC_WRITE);
            if (sc_hndl.ToInt32() != 0)
            {
                int DELETE = 0x10000;
                IntPtr svc_hndl = OpenService(sc_hndl, svcName, DELETE);

                if (svc_hndl.ToInt32() != 0)
                {
                    int i = DeleteService(svc_hndl);
                    if (i != 0)
                    {
                        CloseServiceHandle(sc_hndl);
                        return true;
                    }
                    else
                    {
                        CloseServiceHandle(sc_hndl);
                        return false;
                    }
                }
                else
                    return false;
            }
            else
                return false;
        }
        public bool DoesServiceExist(string svcName)
        {
            ServiceController[] services = ServiceController.GetServices(Environment.MachineName);
            ServiceController service = services.FirstOrDefault(s => s.ServiceName == svcName);

            return service != null;
        }
    }
}
