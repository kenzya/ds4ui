﻿using System.ServiceModel;
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
    public class ApplicationManager
    {
        private readonly IMessengerManager Messenger;
        private readonly IConfigurationManager Configuration;
        private readonly ITranslationManager Translation;
        private readonly ISubscribingService Service;
        private readonly INotifyIconManager NotifyIcon;
        private readonly IEventLogManager Logger;
        private readonly IControllerConfigurationManager Controller;
        private readonly IThemeManager Theme;

        public ApplicationManager(IMessengerManager messenger, ITranslationManager translation, IConfigurationManager configuration, INotifyIconManager notifyIcon, 
            IEventLogManager logger, IControllerConfigurationManager controller, IThemeManager theme)
        {
            Messenger = messenger;
            Translation = translation;
            Configuration = configuration;            
            NotifyIcon = notifyIcon;
            Logger = logger;
            Controller = controller;
            Theme = theme;

            Logger.Initialize(Constants.SERVICE_NAME);
            Logger.Subscribe(param => Messenger.NotifyColleagues(AppMessages.NEW_LOG_MESSAGE, param.Entry));

            Translation.ChangeLanguage(Configuration.GetData(ConfigOptions.OPTION_LANGUAGE));
            
            string a = Configuration.GetData(ConfigOptions.OPTION_ACCENT);
            string t = Configuration.GetData(ConfigOptions.OPTION_THEME);
            Theme.SetTheme(a, t);

            DuplexChannelFactory<ISubscribingService> pipeFactory = new DuplexChannelFactory<ISubscribingService>(new ServiceCommand(Messenger), 
                new NetNamedPipeBinding(), new EndpointAddress(Constants.PIPE_ADDRESS + Constants.SERVICE_NAME));
            Service = pipeFactory.CreateChannel();
            Service.Subscribe();
        }

        public void Start()
        {
            MetroMainWindow w = new MetroMainWindow();            
            w.DataContext = new MainWindowViewModel(Configuration, Translation, NotifyIcon, Controller, Theme, Messenger, Service);
            w.Show();

            if (bool.Parse(Configuration.GetData(ConfigOptions.OPTION_MINIMIZED)))
                w.WindowState = WindowState.Minimized;
            else
                w.WindowState = WindowState.Normal;
        }
        public void Close()
        {
            Logger.Unsubscribe();
            Service.Unsubscribe();
        }
    }
}