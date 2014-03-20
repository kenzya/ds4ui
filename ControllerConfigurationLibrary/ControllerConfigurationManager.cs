using System.Collections.Generic;
using System.IO;

namespace ControllerConfigurationLibrary
{
    /// <summary>
    /// Library used to read and write controller's configuration data
    /// </summary>
    /// TODO: considering the similarities a merge with ConfigurationLibrary
    /// it's a good idea
    public class ControllerConfigurationManager : IControllerConfigurationManager
    {
        #region fields

        // Path of the controller's directory
        private readonly string mainPath;

        // Dictionary of the providers (one for each controller)
        // TODO: replace string with int when we switch to an ID-based system for the controllers
        private readonly Dictionary<string, XmlConfigProvider> dictionary;

        #endregion

        #region constructor

        /// <summary>
        /// Constructor. Set the path of the directory and create a provider for each 
        /// configuration file.
        /// </summary>
        /// <param name="path"></param>
        public ControllerConfigurationManager(string path)
        {
            mainPath = path;
            dictionary = new Dictionary<string, XmlConfigProvider>();

            foreach (string file in Directory.GetFiles(mainPath, "*.xml"))
            {
                XmlConfigProvider provider = new XmlConfigProvider(file);
                dictionary.Add(Path.GetFileNameWithoutExtension(file), provider);
            }
        }
        
        #endregion

        #region methods

        /// <summary>
        /// Set the value of a controller's configuration property
        /// </summary>
        /// TODO: switch to int instead of string for key dictionary
        public void SetData(string key, string name, string value)
        { 
            if (!dictionary.ContainsKey(name))
            {
                CreateNewConfiguration(mainPath, name);
            }

            dictionary[name].SetValue(key, value);
        }

        /// <summary>
        /// Get the value of a controller's configuration property
        /// </summary>
        /// TODO: switch to int instead of string for key dictionary
        public string GetData(string key, string name)
        {
            if (!dictionary.ContainsKey(name))
            {
                CreateNewConfiguration(mainPath, name);
            }

            return dictionary[name].GetValue(key);
        }

        /// <summary>
        /// Get the default value of a controller's configuration property
        /// </summary>
        /// TODO: switch to int instead of string for key dictionary
        public string GetDefault(string key, string name)
        {
            if (!dictionary.ContainsKey(name))
            {
                CreateNewConfiguration(mainPath, name);
            }

            return dictionary[name].GetDefault(key);
        }

        /// <summary>
        /// Create a new configuration file using the default configuration as a model.
        /// When we get or set a value and the corrisponding provider is not found, this
        /// means that it's a new controller and thus a new configuration should be created.
        /// </summary>
        private void CreateNewConfiguration(string directory, string name)
        {
            string defaultFile = Path.Combine(directory, "default_controller.xml");
            string newFile = Path.Combine(directory, name + ".xml");

            File.Copy(defaultFile, newFile);
            XmlConfigProvider provider = new XmlConfigProvider(newFile);
            dictionary.Add(Path.GetFileNameWithoutExtension(newFile), provider);
        }

        #endregion
    }
}
