namespace ConfigurationLibrary
{
    /// <summary>
    /// Library used to read and write configuration data
    /// </summary>
    public class ConfigurationManager : IConfigurationManager
    {
        #region fields

        // Path of configuration file
        private string Path;

        #endregion

        #region properties

        /// <summary>
        /// Provider used to retrieve and send data
        /// </summary>
        /// TODO: this should be an interface if we decide to use more provider or
        /// we can simplify this a lot if we stick with XML only configuration
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

        /// <summary>
        /// Constructor. Set the path of the file and create the default provider
        /// </summary>
        public ConfigurationManager(string path)
        {
            Path = path;
            ConfigProvider = new XmlConfigProvider(Path);
        }

        #endregion

        #region methods

        /// <summary>
        /// Retrieve the value of the key
        /// </summary>
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
        
        /// <summary>
        /// Retrieve the default value of the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Set the value of the key
        /// </summary>
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
