using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using CommunicationLibrary;
using ConfigurationLibrary;
using ControllerConfigurationLibrary;
using CoreLibrary;
using MessengerLibrary;
using NotifyIconLibrary;
using ThemeLibrary;
using TranslationLibrary;

namespace DS4Tool
{
    /// <summary>
    /// This ViewModel is associated with the MainWindows (Metro or Classic, no difference)
    /// and it's only purpose is handling User data
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Dependency
        
        private readonly IConfigurationManager ConfigurationManager;
        private readonly ITranslationManager TranslationManager;
        private readonly INotifyIconManager IconManager;
        private readonly IControllerConfigurationManager ControllerConfigurationManager;
        private readonly IThemeManager ThemeManager;
        private readonly IMessengerManager MessengerManager;
        private readonly ISubscribingService SubscribingService;

        #endregion 

        #region Controllers Tab

        private ObservableCollection<ControllerViewModel> controllers;
        public ObservableCollection<ControllerViewModel> Controllers
        {
            get
            {
                if (controllers == null)
                {
                    controllers = new ObservableCollection<ControllerViewModel>();
                }
                return controllers;
            }
        }

        private void ControllerChangeStatus(ControllerContract controller)
        {
            ControllerViewModel c = Controllers.FirstOrDefault(param => param.Id == controller.Id);

            if (c != null && controller.IsUsbConnected == false && controller.IsBluetoothConnected == false)
            {
                Controllers.Remove(c);
            }
            else if (c != null)
            {
                c.ChangeStatus(controller);
            }
            else
            {
                Controllers.Add(new ControllerViewModel(IconManager, ControllerConfigurationManager, controller));
            }
        }

        #endregion // Controllers Tab

        #region Log Tab

        private CustomCollection<EventLogEntry> messageList;
        public CustomCollection<EventLogEntry> MessageList
        {
            get
            {
                if (messageList == null)
                {
                    messageList = new CustomCollection<EventLogEntry>();
                }
                return messageList;
            }
        }

        private void AddLogMessage(EventLogEntry message)
        {
            MessageList.Add(message);
        }

        #endregion // Log Tab

        #region Option Tab

        private bool? metroStyle;
        public bool? MetroStyle
        {
            get
            {
                if (metroStyle == null)
                    return bool.Parse(ConfigurationManager.GetData(ConfigOptions.OPTION_STYLE));
                return metroStyle;
            }
            set
            {
                if (metroStyle != value)
                {
                    metroStyle = value;
                    NotifyPropertyChanged(() => MetroStyle);

                    ConfigurationManager.SetData(ConfigOptions.OPTION_STYLE, (value ?? false).ToString());
                }
            }
        }

        private bool? exclusiveMode;
        public bool? ExclusiveMode
        {
            get
            {
                if (exclusiveMode == null)
                    return bool.Parse(ConfigurationManager.GetData(ConfigOptions.OPTION_EXCLUSIVE));
                return exclusiveMode;
            }
            set
            {
                if (exclusiveMode != value)
                {
                    exclusiveMode = value;
                    NotifyPropertyChanged(() => ExclusiveMode);

                    ConfigurationManager.SetData(ConfigOptions.OPTION_EXCLUSIVE, (value ?? false).ToString());
                    if (value == true)
                        SubscribingService.SendCommand(ControllerContract.Create(ControllerMessage.CONTROLLERS_EXCLUSIVE_ENABLED));
                    else
                        SubscribingService.SendCommand(ControllerContract.Create(ControllerMessage.CONTROLLERS_EXCLUSIVE_DISABLED));
                }
            }
        }

        private IEnumerable<string> accentList;
        public IEnumerable<string> AccentList
        {
            get
            {
                if (accentList == null)
                {
                    accentList = ThemeManager.GetAccentList();
                }
                return accentList;
            }
        }

        private string selectedAccent;
        public string SelectedAccent
        {
            get
            {
                if (selectedAccent == null)
                    return ConfigurationManager.GetData(ConfigOptions.OPTION_ACCENT);
                return selectedAccent;
            }
            set
            {
                if (selectedAccent != value)
                {
                    selectedAccent = value;
                    NotifyPropertyChanged(() => SelectedAccent);
                    
                    ConfigurationManager.SetData(ConfigOptions.OPTION_ACCENT, value);
                    ThemeManager.SetTheme(value, SelectedTheme);
                }
            }
        }

        private IEnumerable<string> themeList;
        public IEnumerable<string> ThemeList
        {
            get
            {
                if (themeList == null)
                {
                    themeList = ThemeManager.GetThemeList();
                }
                return themeList;
            }
        }

        private string selectedTheme;
        public string SelectedTheme
        {
            get
            {
                if (selectedTheme == null)
                    return ConfigurationManager.GetData(ConfigOptions.OPTION_THEME);
                return selectedTheme;
            }
            set
            {
                if (selectedTheme != value)
                {
                    selectedTheme = value;
                    NotifyPropertyChanged(() => SelectedTheme);

                    ConfigurationManager.SetData(ConfigOptions.OPTION_THEME, value);
                    ThemeManager.SetTheme(SelectedAccent, value);
                }
            }
        }

        private IEnumerable<string> languageList;
        public IEnumerable<string> LanguageList
        {
            get
            {
                if (languageList == null)
                {
                    languageList = TranslationManager.GetLanguagesList().ToList();
                }
                return languageList;
            }
        }

        private string selectedLanguage;
        public string SelectedLanguage
        {
            get
            {
                if (selectedLanguage == null)
                    return ConfigurationManager.GetData(ConfigOptions.OPTION_LANGUAGE);
                return selectedLanguage;
            }
            set
            {
                if (selectedLanguage != value)
                {
                    selectedLanguage = value;
                    NotifyPropertyChanged(() => SelectedLanguage);
                    
                    ConfigurationManager.SetData(ConfigOptions.OPTION_LANGUAGE, value);
                    TranslationManager.ChangeLanguage(value);
                }
            }
        }

        private DelegateCommand restoreDefaultCommand;
        public ICommand RestoreDefaultCommand
        {
            get
            {
                if (restoreDefaultCommand == null)
                {
                    restoreDefaultCommand = new DelegateCommand(param => ExecuteRestore());
                }
                return restoreDefaultCommand;
            }
        }
        private void ExecuteRestore()
        {
            SelectedAccent = ConfigurationManager.GetDefault(ConfigOptions.OPTION_ACCENT);
            SelectedTheme = ConfigurationManager.GetDefault(ConfigOptions.OPTION_THEME);
            SelectedLanguage = ConfigurationManager.GetDefault(ConfigOptions.OPTION_LANGUAGE);
            MetroStyle = bool.Parse(ConfigurationManager.GetDefault(ConfigOptions.OPTION_STYLE));
            ExclusiveMode = bool.Parse(ConfigurationManager.GetDefault(ConfigOptions.OPTION_EXCLUSIVE));
        }

        #endregion

        #region Ctor

        public MainWindowViewModel(IConfigurationManager configurationManager, ITranslationManager translationManager, INotifyIconManager iconManager, 
            IControllerConfigurationManager controllerConfigurationManager, IThemeManager themeManager, IMessengerManager messengerManager, 
            ISubscribingService subscribingService)
        {            
            ConfigurationManager = configurationManager;
            TranslationManager = translationManager;
            IconManager = iconManager;
            ControllerConfigurationManager = controllerConfigurationManager;
            ThemeManager = themeManager;
            MessengerManager = messengerManager;
            SubscribingService = subscribingService;

            MessengerManager.Register<ControllerContract>(AppMessages.CONTROLLER_CHANGE_STATUS, param => ControllerChangeStatus(param));
            MessengerManager.Register<EventLogEntry>(AppMessages.NEW_LOG_MESSAGE, param => AddLogMessage(param));
        }

        #endregion

        int i = 0;
        private DelegateCommand testCmd;
        public ICommand TestCmd
        {
            get
            {
                if (testCmd == null)
                {
                    testCmd = new DelegateCommand(param => Test());
                }
                return testCmd;
            }
        }
        private void Test()
        {
            i++;
            ControllerContract c = ControllerContract.Create(i.ToString(), "N° " + i, false, true, 50, ControllerMessage.CONTROLLER_CONNECT);
            ControllerChangeStatus(c);
            AddLogMessage(new TestLog("Prova", TranslationManager));
            AddLogMessage(new TestLog("Ciao", TranslationManager));
        }

        private CustomCollection<TestLog> messageList2;
        public CustomCollection<TestLog> MessageList2
        {
            get
            {
                if (messageList2 == null)
                {
                    messageList2 = new CustomCollection<TestLog>();
                }
                return messageList2;
            }
        }
        private void AddLogMessage(TestLog message)
        {
            MessageList2.Insert(0, message);
        }

        public class TestLog
        {
            public string Message { get; set; }
            public string TimeGenerated { get; set; }

            public TestLog(string msg, ITranslationManager trans)
            {
                Message = trans.Translate(msg).ToString();
                TimeGenerated = DateTime.Now.ToLongTimeString();
            }
        }
    }
}
