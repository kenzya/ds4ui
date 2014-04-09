using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Windows;
using CommunicationLibrary;
using ConfigurationLibrary;
using ControllerConfigurationLibrary;
using EventLogLibrary;
using MessengerLibrary;
using NotifyIconLibrary;
using ThemeLibrary;
using TranslationLibrary;

namespace DS4Tool
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ApplicationManager appManager;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string applicationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DS4Tool");
            string languageDirectory = Path.Combine(applicationDirectory, "Languages");
            string controllerDirectory = Path.Combine(applicationDirectory, "Controllers");
            string configurationFile = Path.Combine(applicationDirectory, "configuration.xml");

            CheckDirectories(applicationDirectory, languageDirectory, controllerDirectory, configurationFile);
            if (CheckService())
            {
                IConfigurationManager configuration = new ConfigurationManager(configurationFile);
                ITranslationManager translation = new TranslationManager(languageDirectory);
                IControllerConfigurationManager controller = new ControllerConfigurationManager(controllerDirectory);
                IMessengerManager messenger = new MessengerManager();
                INotifyIconManager notifyIcon = new NotifyIconManager();
                IEventLogManager logger = new EventLogManager();
                IThemeManager theme = new ThemeManager();

                appManager = new ApplicationManager(messenger, translation, configuration, notifyIcon, logger, controller, theme);
                appManager.Start();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (appManager != null)
                appManager.Close();
        }

        private void CheckDirectories(string applicationDirectory, string languageDirectory, string controllerDirectory, string configurationFile)
        {            
            if (!Directory.Exists(applicationDirectory))
                Directory.CreateDirectory(applicationDirectory);
            
            if (!File.Exists(configurationFile))
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                using (Stream inputStream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".Resources.configuration.xml"))
                using (FileStream outStream = File.Create(configurationFile))
                    inputStream.CopyTo(outStream);
            }

            if (!Directory.Exists(languageDirectory))
                Directory.CreateDirectory(languageDirectory);

            if (Directory.EnumerateFiles(languageDirectory).Count() <= 0)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                using (Stream inputStream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".Resources.default_language.xml"))
                using (FileStream outStream = File.Create(Path.Combine(languageDirectory, "default_language.xml")))
                    inputStream.CopyTo(outStream);
            }

            if (!Directory.Exists(controllerDirectory))
                Directory.CreateDirectory(controllerDirectory);

            if (Directory.EnumerateFiles(controllerDirectory).Count() <= 0 || !Directory.EnumerateFiles(controllerDirectory).Contains(Path.Combine(controllerDirectory, "default_controller.xml")))
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                using (Stream inputStream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".Resources.default_controller.xml"))
                using (FileStream outStream = File.Create(Path.Combine(controllerDirectory, "default_controller.xml")))
                    inputStream.CopyTo(outStream);
            }
        }

        private bool CheckService()
        {
            ServiceController sc = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == Constants.SERVICE_NAME);
            if (sc == null)
            {
                MessageBox.Show("DS4Service is not installed!", "ERROR");                
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}