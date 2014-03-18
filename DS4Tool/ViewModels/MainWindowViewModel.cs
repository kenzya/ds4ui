using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using CommunicationLibrary;
using CoreLibrary;

namespace DS4Tool
{
    /// <summary>
    /// This ViewModel is associated with the MainWindows (Metro or Classic, no difference)
    /// and it's only purpose is handling User data
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region ToolBar Info

        private string userName;
        public string UserName
        {
            get
            {
                if (string.IsNullOrEmpty(userName))
                    userName = App.AppManager.GetUserName();
                return userName;
            }
        }

        private string userImage;
        public string UserImage
        {
            get 
            {
                if (string.IsNullOrEmpty(userImage))
                    userImage = App.AppManager.GetUserImage();
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
                    return App.AppManager.GetStyle(false);
                return metroStyle;
            }
            set
            {
                if (metroStyle != value)
                {
                    metroStyle = value;
                    NotifyPropertyChanged(() => MetroStyle);
                    App.AppManager.SetStyle(value ?? false);
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
                    accentList = App.AppManager.GetAccentList();
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
                    return App.AppManager.GetAccent(false).Name;
                return selectedAccent;
            }
            set
            {
                if (selectedAccent != value)
                {
                    selectedAccent = value;
                    NotifyPropertyChanged(() => SelectedAccent);
                    App.AppManager.SetAccent(value);
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
                    themeList = App.AppManager.GetThemeList();
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
                    return App.AppManager.GetTheme(false).ToString();
                return selectedTheme;
            }
            set
            {
                if (selectedTheme != value)
                {
                    selectedTheme = value;
                    NotifyPropertyChanged(() => SelectedTheme);
                    App.AppManager.SetTheme(value);
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
                    languageList = App.AppManager.GetLanguagesList();
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
                    return App.AppManager.GetLanguage(false);
                return selectedLanguage;
            }
            set
            {
                if (selectedLanguage != value)
                {
                    selectedLanguage = value;
                    NotifyPropertyChanged(() => SelectedLanguage);
                    App.AppManager.SetLanguage(value);
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
            SelectedAccent = App.AppManager.GetAccent(true).Name;
            SelectedTheme = App.AppManager.GetTheme(true).ToString();
            SelectedLanguage = App.AppManager.GetLanguage(true);
            MetroStyle = App.AppManager.GetStyle(true);
        }

        #endregion

        #region Ctor

        public MainWindowViewModel()
        {
            App.AppManager.RegisterControllerChange(param => ControllerChangeStatus(param));
            App.AppManager.RegisterNewLogMessage(param => AddLogMessage(param));
        }

        #endregion
    }
}
