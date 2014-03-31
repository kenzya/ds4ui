using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using CommunicationLibrary;

namespace DS4Service
{
    [RunInstaller(true)]
    public class DS4ServiceInstaller : Installer
    {
        private ServiceProcessInstaller processInstaller;
        private ServiceInstaller serviceInstaller;

        public DS4ServiceInstaller()
        {
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = Constants.SERVICE_NAME;

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }
    }
}