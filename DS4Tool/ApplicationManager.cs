using CommunicationLibrary;
using ConfigurationLibrary;
using ControllerConfigurationLibrary;
using EventLogLibrary;
using MahApps.Metro;
using MessengerLibrary;
using NotifyIconLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using TranslationLibrary;
using UserLibrary;

namespace DS4Tool
{
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
            
            SetAccent(Configuration.GetData(ConfOptions.OPTION_ACCENT));
            SetTheme(Configuration.GetData(ConfOptions.OPTION_THEME));
            SetLanguage(Configuration.GetData(ConfOptions.OPTION_LANGUAGE));

            DuplexChannelFactory<ISubscribingService> pipeFactory = new DuplexChannelFactory<ISubscribingService>(new ServiceCommand(), 
                new NetNamedPipeBinding(), new EndpointAddress(Constants.PIPE_ADDRESS + Constants.SERVICE_NAME));
            Service = pipeFactory.CreateChannel();
            Service.Subscribe();
        }

        public void Start()
        {
            MetroMainWindow w = new MetroMainWindow();
            w.DataContext = new MainWindowViewModel();
            w.Show();
        }
        public void Close()
        {
            Logger.Unsubscribe();
            Service.Unsubscribe();
        }

        public string GetUserName()
        {
            string userName = User.GetUserName();

            if (string.IsNullOrEmpty(userName))
                return string.Empty;
            else
                return userName;
        }
        public string GetUserImage()
        {
            string userImage = User.GetUserImage();

            if (string.IsNullOrEmpty(userImage))
                return string.Empty;
            else
                return userImage;
        }
        public Accent GetAccent(bool defaultValue)
        {
            if (defaultValue == false)
                return ThemeManager.DefaultAccents.Single(x => x.Name == Configuration.GetData(ConfOptions.OPTION_ACCENT));
            else
                return ThemeManager.DefaultAccents.Single(x => x.Name == Configuration.GetDefault(ConfOptions.OPTION_ACCENT));
        }
        public Theme GetTheme(bool defaultValue)
        {
            if (defaultValue == false)
                return (Theme)Enum.Parse(typeof(Theme), Configuration.GetData(ConfOptions.OPTION_THEME));
            else
                return (Theme)Enum.Parse(typeof(Theme), Configuration.GetDefault(ConfOptions.OPTION_THEME));
        }
        public string GetLanguage(bool defaultValue)
        {
            if (defaultValue == false)
                return Configuration.GetData(ConfOptions.OPTION_LANGUAGE);
            else
                return Configuration.GetDefault(ConfOptions.OPTION_LANGUAGE);
        }
        public bool GetStyle(bool defaultValue)
        {
            if (defaultValue == false)
                return bool.Parse(Configuration.GetData(ConfOptions.OPTION_STYLE));
            else
                return bool.Parse(Configuration.GetDefault(ConfOptions.OPTION_STYLE));
        }
        public void SetStyle(bool style)
        {
            Configuration.SetData(ConfOptions.OPTION_STYLE, style.ToString());
        }
        public void SetAccent(string accent)
        {
            Configuration.SetData(ConfOptions.OPTION_ACCENT, accent);
            Accent a = ThemeManager.DefaultAccents.Single(x => x.Name == accent);
            Theme theme = (Theme)Enum.Parse(typeof(Theme), Configuration.GetData(ConfOptions.OPTION_THEME));

            ThemeManager.ChangeTheme(App.Current, a, theme);
        }
        public void SetTheme(string theme)
        {
            Configuration.SetData(ConfOptions.OPTION_THEME, theme);
            Theme t = (Theme)Enum.Parse(typeof(Theme), theme);
            Accent accent = ThemeManager.DefaultAccents.Single(x => x.Name == Configuration.GetData(ConfOptions.OPTION_ACCENT));

            ThemeManager.ChangeTheme(App.Current, accent, t);

            if (t == Theme.Dark)
            {
                Application.Current.Resources["DS4Image"] = Application.Current.Resources["DS4ImageWhite"];
                Application.Current.Resources["USBImage"] = Application.Current.Resources["USBImageWhite"];
                Application.Current.Resources["BTImage"] = Application.Current.Resources["BTImageWhite"];
            }
            else
            {
                Application.Current.Resources["DS4Image"] = Application.Current.Resources["DS4ImageBlack"];
                Application.Current.Resources["USBImage"] = Application.Current.Resources["USBImageBlack"];
                Application.Current.Resources["BTImage"] = Application.Current.Resources["BTImageBlack"];
            }
        }
        public void SetLanguage(string language)
        {
            Configuration.SetData(ConfOptions.OPTION_LANGUAGE, language);
            Translation.ChangeLanguage(language);
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

        public IEnumerable<string> GetAccentList()
        {
            return ThemeManager.DefaultAccents.Select(x => x.Name).ToList();
        }
        public IEnumerable<string> GetThemeList()
        {
            return Enum.GetNames(typeof(Theme)).ToList();
        }
        public IEnumerable<string> GetLanguagesList()
        {
            List<string> languages = Translation.GetLanguagesList().ToList();

            if (languages != null && languages.Count > 0)
                return languages;
            else
                return null;
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