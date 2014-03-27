using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using CommunicationLibrary;
using ConfigurationLibrary;
using ControllerConfigurationLibrary;
using EventLogLibrary;
using MahApps.Metro;
using MessengerLibrary;
using NotifyIconLibrary;
using TranslationLibrary;
using UserLibrary;

namespace DS4Tool
{
    // TODO: keep theme change here??? (along with image color?)
    public class ApplicationManager
    {
        private readonly IMessengerManager Messenger;
        private readonly IConfigurationManager Configuration;
        private readonly ITranslationManager Translation;
        private readonly IUserManager User;
        private readonly ISubscribingService Service;
        private readonly INotifyIconManager NotifyIcon;
        private readonly IEventLogManager Logger;
        private readonly IControllerConfigurationManager Controller;

        public ApplicationManager(IMessengerManager messenger, ITranslationManager translation, IConfigurationManager configuration, 
                                  IUserManager user, INotifyIconManager notifyIcon, IEventLogManager logger, IControllerConfigurationManager controller)
        {
            Messenger = messenger;
            Translation = translation;
            Configuration = configuration;
            User = user;
            NotifyIcon = notifyIcon;
            Logger = logger;
            Controller = controller;

            Logger.Initialize(Constants.SERVICE_NAME);
            Logger.Subscribe(param => Messenger.NotifyColleagues(AppMessages.NEW_LOG_MESSAGE, param.Entry));

            Accent a = ThemeManager.DefaultAccents.Single(x => x.Name == Configuration.GetData(ConfOptions.OPTION_ACCENT));
            Theme t = (Theme)Enum.Parse(typeof(Theme), Configuration.GetData(ConfOptions.OPTION_THEME));
            ThemeManager.ChangeTheme(App.Current, a, t);

            Translation.ChangeLanguage(Configuration.GetData(ConfOptions.OPTION_LANGUAGE));

            DuplexChannelFactory<ISubscribingService> pipeFactory = new DuplexChannelFactory<ISubscribingService>(new ServiceCommand(), 
                new NetNamedPipeBinding(), new EndpointAddress(Constants.PIPE_ADDRESS + Constants.SERVICE_NAME));
            Service = pipeFactory.CreateChannel();
            Service.Subscribe();
        }

        public void Start()
        {
            MetroMainWindow w = new MetroMainWindow();
            w.DataContext = new MainWindowViewModel(User, Configuration, Translation);
            w.Show();
        }
        public void Close()
        {
            Logger.Unsubscribe();
            Service.Unsubscribe();
        }

        public bool GetControllerIcon(bool defaultValue, string name)
        {
            bool result = false;
            if (defaultValue == false)
                bool.TryParse(Controller.GetData(ControllerOptions.SHOW_ICON, name), out result);
            else
                bool.TryParse(Controller.GetDefault(ControllerOptions.SHOW_ICON, name), out result);

            return result;
        }
        public void SetControllerIcon(ControllerContract status)
        {
            Controller.SetData(ControllerOptions.SHOW_ICON, status.Name, status.IsIconVisible.ToString());
            NotifyIcon.SetIcon(status.Id, status.IsUsbConnected, status.BatteryValue, status.IsIconVisible);
        }


        public void NotifyControllerChange(ControllerContract status)
        {
            Messenger.NotifyColleagues(AppMessages.CONTROLLER_CHANGE_STATUS, status);
        }
        public void RegisterControllerChange(Action<ControllerContract> action)
        {
            Messenger.Register<ControllerContract>(AppMessages.CONTROLLER_CHANGE_STATUS, action);
        }
        public void RegisterNewLogMessage(Action<EventLogEntry> action)
        {
            Messenger.Register<EventLogEntry>(AppMessages.NEW_LOG_MESSAGE, action);
        }
    }
}