namespace ConfigurationLibrary
{
    public class ConfigurationManager : IConfigurationManager
    {
        #region fields

        private string Path;

        #endregion

        #region properties

        private XmlConfigProvider configProvider;
        private XmlConfigProvider ConfigProvider
        {
            get
            {
                return configProvider;
            }
            set
            {
                configProvider = value;
            }
        }

        #endregion

        #region constructor

        private ConfigurationManager()
        { }
        public ConfigurationManager(string path)
        {
            Initialize(path);
        }

        #endregion

        #region methods

        public void Initialize(string path)
        {
            Path = path;
            ConfigProvider = new XmlConfigProvider(Path);
        }
        public string GetData(string key)
        {
            if (ConfigProvider != null)
            {
                string value = ConfigProvider.GetValue(key);
                if (value != null)
                {
                    return value;
                }
            }
            return string.Empty;
        }
        public string GetDefault(string key)
        {
            if (ConfigProvider != null)
            {
                string value = ConfigProvider.GetDefault(key);
                if (value != null)
                {
                    return value;
                }
            }
            return string.Empty;
        }

        public void SetData(string key, string value)
        {
            if (ConfigProvider != null)
            {
                ConfigProvider.SetValue(key, value);
            }
        }

        #endregion
    }
}
