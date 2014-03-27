using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using CommunicationLibrary;
using ConfigurationLibrary;
using CoreLibrary;
using MahApps.Metro;
using TranslationLibrary;
using UserLibrary;

namespace DS4Tool
{
    /// <summary>
    /// This ViewModel is associated with the MainWindows (Metro or Classic, no difference)
    /// and it's only purpose is handling User data
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Dependency

        private readonly IUserManager UserManager;
        private readonly IConfigurationManager ConfigurationManager;
        private readonly ITranslationManager TranslationManager;

        #endregion 

        #region ToolBar Info

        private string userName;
        public string UserName
        {
            get
            {
                if (string.IsNullOrEmpty(userName))
                    userName = UserManager.GetUserName();
                return userName;
            }
        }

        private string userImage;
        public string UserImage
        {
            get 
            {
                if (string.IsNullOrEmpty(userImage))
                    userImage = UserManager.GetUserImage();
                return userImage;
            }
        }

        #endregion // ToolBar Info

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
                Controllers.Add(new ControllerViewModel(controller));
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
            MessageList.Insert(0, message);
        }

        #endregion // Log Tab

        #region Option Tab

        private bool? metroStyle;
        public bool? MetroStyle
        {
            get
            {
                if (metroStyle == null)
                    return bool.Parse(ConfigurationManager.GetData(ConfOptions.OPTION_STYLE));
                return metroStyle;
            }
            set
            {
                if (metroStyle != value)
                {
                    metroStyle = value;
                    NotifyPropertyChanged(() => MetroStyle);

                    ConfigurationManager.SetData(ConfOptions.OPTION_STYLE, (value ?? false).ToString());
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
                    accentList = ThemeManager.DefaultAccents.Select(x => x.Name).ToList();
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
                    return ThemeManager.DefaultAccents.Single(x => x.Name == ConfigurationManager.GetData(ConfOptions.OPTION_ACCENT)).Name;
                return selectedAccent;
            }
            set
            {
                if (selectedAccent != value)
                {
                    selectedAccent = value;
                    NotifyPropertyChanged(() => SelectedAccent);
                    
                    ConfigurationManager.SetData(ConfOptions.OPTION_ACCENT, value);
                    Accent a = ThemeManager.DefaultAccents.Single(x => x.Name == value);
                    Theme theme = (Theme)Enum.Parse(typeof(Theme), ConfigurationManager.GetData(ConfOptions.OPTION_THEME));
                    ThemeManager.ChangeTheme(App.Current, a, theme);
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
                    themeList = Enum.GetNames(typeof(Theme)).ToList();
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
                    return ((Theme)Enum.Parse(typeof(Theme), ConfigurationManager.GetData(ConfOptions.OPTION_THEME))).ToString();
                return selectedTheme;
            }
            set
            {
                if (selectedTheme != value)
                {
                    selectedTheme = value;
                    NotifyPropertyChanged(() => SelectedTheme);

                    ConfigurationManager.SetData(ConfOptions.OPTION_THEME, value);
                    Theme t = (Theme)Enum.Parse(typeof(Theme), value);
                    Accent accent = ThemeManager.DefaultAccents.Single(x => x.Name == ConfigurationManager.GetData(ConfOptions.OPTION_ACCENT));
                    ThemeManager.ChangeTheme(App.Current, accent, t);
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
                    return ConfigurationManager.GetData(ConfOptions.OPTION_LANGUAGE);
                return selectedLanguage;
            }
            set
            {
                if (selectedLanguage != value)
                {
                    selectedLanguage = value;
                    NotifyPropertyChanged(() => SelectedLanguage);
                    
                    ConfigurationManager.SetData(ConfOptions.OPTION_LANGUAGE, value);
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
            SelectedAccent = ThemeManager.DefaultAccents.Single(x => x.Name == ConfigurationManager.GetDefault(ConfOptions.OPTION_ACCENT)).Name;
            SelectedTheme = ((Theme)Enum.Parse(typeof(Theme), ConfigurationManager.GetDefault(ConfOptions.OPTION_THEME))).ToString();
            SelectedLanguage = ConfigurationManager.GetDefault(ConfOptions.OPTION_LANGUAGE);
            MetroStyle = bool.Parse(ConfigurationManager.GetDefault(ConfOptions.OPTION_STYLE));
        }

        #endregion

        #region Ctor

        public MainWindowViewModel(IUserManager userManager, IConfigurationManager configurationManager, ITranslationManager translationManager)
        {
            UserManager = userManager;
            ConfigurationManager = configurationManager;
            TranslationManager = translationManager;

            App.AppManager.RegisterControllerChange(param => ControllerChangeStatus(param));
            App.AppManager.RegisterNewLogMessage(param => AddLogMessage(param));
        }

        #endregion
    }
}
