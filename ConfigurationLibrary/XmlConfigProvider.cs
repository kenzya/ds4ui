using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ConfigurationLibrary
{
    /// <summary>
    /// Provider of XML configuration files
    /// </summary>
    internal class XmlConfigProvider
    {
        #region fields

        // Path of the file XML
        private readonly string path;

        // XML Document
        private readonly XDocument xmlDocument;

        // Dictionary of configuration's values
        private readonly Dictionary<string, string> dictionary;

        // Dictionary of configuration's default values
        private readonly Dictionary<string, string> defaults;

        #endregion

        #region ctor & dtor

        /// <summary>
        /// Constructor. Initialize fields and fill dictionaries with values
        /// </summary>
        /// <param name="file"></param>
        internal XmlConfigProvider(string file)
        {
            path = file;
            dictionary = new Dictionary<string, string>();
            defaults = new Dictionary<string, string>();
            xmlDocument = XDocument.Load(path);

            foreach (XElement node in xmlDocument.Root.Elements())
            {
                dictionary.Add(node.Name.LocalName, node.Value);
                defaults.Add(node.Name.LocalName, node.Attribute("Default").Value);
            }
        }

        /// <summary>
        /// Destructor. Write the XML Document when we destroy it (it happens when we quit the client.
        /// We are fine with it because during runtime we use the value from dictionary that are always
        /// updated)
        /// </summary>
        ~XmlConfigProvider()
        {
            xmlDocument.Save(path);
        }

        #endregion

        #region methods

        /// <summary>
        /// Read the value from dictionary
        /// </summary>
        internal string GetValue(string key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            else
                return null;
        }

        /// <summary>
        /// Read the value from the dictionary of default values
        /// </summary>
        internal string GetDefault(string key)
        {
            if (defaults.ContainsKey(key))
                return defaults[key];
            else
                return null;
        }

        /// <summary>
        /// Update the value on the dictionary and on the file
        /// (the value on the file will appear only when we save it
        /// and it happens when we close the application)
        /// </summary>
        internal void SetValue(string key, string value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;

                xmlDocument.Root.Elements().Single(x => x.Name.LocalName == key).SetValue(value);
            }
        }

        #endregion
    }
}
