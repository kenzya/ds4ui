using System.Collections.Generic;
using System.IO;

namespace ControllerConfigurationLibrary
{
    public class ControllerConfigurationManager : IControllerConfigurationManager
    {
        #region fields

        private readonly string mainPath;
        private readonly Dictionary<string, XmlConfigProvider> dictionary;

        #endregion

        #region constructor

        public ControllerConfigurationManager(string path)
        {
            mainPath = path;
            dictionary = new Dictionary<string, XmlConfigProvider>();
            Initialize(mainPath);
        }
        
        #endregion

        #region methods

        public void Initialize(string path)
        {
            foreach (string file in Directory.GetFiles(path, "*.xml"))
            {
                XmlConfigProvider provider = new XmlConfigProvider(file);
                dictionary.Add(Path.GetFileNameWithoutExtension(file), provider);
            }
        }

        public void SetData(string key, string name, string value)
        { 
            if (!dictionary.ContainsKey(name))
            {
                CreateNewConfiguration(mainPath, name);
            }

            dictionary[name].SetValue(key, value);
        }

        public string GetData(string key, string name)
        {
            if (!dictionary.ContainsKey(name))
            {
                CreateNewConfiguration(mainPath, name);
            }

            return dictionary[name].GetValue(key);
        }

        public string GetDefault(string key, string name)
        {
            if (!dictionary.ContainsKey(name))
            {
                CreateNewConfiguration(mainPath, name);
            }

            return dictionary[name].GetDefault(key);
        }

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
