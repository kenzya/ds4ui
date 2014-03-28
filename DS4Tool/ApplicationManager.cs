using System.ServiceModel;
using CommunicationLibrary;
using ConfigurationLibrary;
using ControllerConfigurationLibrary;
using EventLogLibrary;
using MessengerLibrary;
using NotifyIconLibrary;
using ThemeLibrary;
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
        private readonly IThemeManager Theme;

        public ApplicationManager(IMessengerManager messenger, ITranslationManager translation, IConfigurationManager configuration, IUserManager user, 
                                  INotifyIconManager notifyIcon, IEventLogManager logger, IControllerConfigurationManager controller, IThemeManager theme)
        {
            Messenger = messenger;
            Translation = translation;
            Configuration = configuration;
            User = user;
            NotifyIcon = notifyIcon;
            Logger = logger;
            Controller = controller;
            Theme = theme;

            Logger.Initialize(Constants.SERVICE_NAME);
            Logger.Subscribe(param => Messenger.NotifyColleagues(AppMessages.NEW_LOG_MESSAGE, param.Entry));

            string a = Configuration.GetData(ConfOptions.OPTION_ACCENT);
            string t = Configuration.GetData(ConfOptions.OPTION_THEME);
            Theme.SetTheme(a, t);

            Translation.ChangeLanguage(Configuration.GetData(ConfOptions.OPTION_LANGUAGE));

            DuplexChannelFactory<ISubscribingService> pipeFactory = new DuplexChannelFactory<ISubscribingService>(new ServiceCommand(Messenger), 
                new NetNamedPipeBinding(), new EndpointAddress(Constants.PIPE_ADDRESS + Constants.SERVICE_NAME));
            Service = pipeFactory.CreateChannel();
            Service.Subscribe();
        }

        public void Start()
        {
            MetroMainWindow w = new MetroMainWindow();
            w.DataContext = new MainWindowViewModel(User, Configuration, Translation, NotifyIcon, Controller, Theme, Messenger);
            w.Show();
        }
        public void Close()
        {
            Logger.Unsubscribe();
            Service.Unsubscribe();
        }
    }
}