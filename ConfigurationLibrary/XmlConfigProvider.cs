using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ConfigurationLibrary
{
    internal class XmlConfigProvider
    {
        #region fields

        private readonly string path;
        private readonly XDocument xmlDocument;
        private readonly Dictionary<string, string> dictionary;
        private readonly Dictionary<string, string> defaults;

        #endregion

        #region ctor & dtor

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

        ~XmlConfigProvider()
        {
            xmlDocument.Save(path);
        }

        #endregion

        #region methods

        internal string GetValue(string key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            else
                return null;
        }

        internal string GetDefault(string key)
        {
            if (defaults.ContainsKey(key))
                return defaults[key];
            else
                return null;
        }

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
