using System.Collections.Generic;
using System.Xml.Linq;

namespace TranslationLibrary
{
    /// <summary>
    /// The XmlTranslationProvider parse a XML file and store the resulting values
    /// that should be used for localization purpose
    /// </summary>
    internal class XmlTranslationProvider
    {
        #region readonly members

        internal readonly string language;
        internal readonly string author;
        internal readonly string version;

        private readonly Dictionary<string, string> dictionary;

        #endregion // readonly members

        #region ctor

        /// <summary>
        /// Initialize a new XmlTranslationProvider.
        /// </summary>
        /// <param name="file">Xml file containing the dictionary</param>
        internal XmlTranslationProvider(string file)
        {
            dictionary = new Dictionary<string, string>();
            XDocument xmlDocument = XDocument.Load(file);
            language = xmlDocument.Root.Attribute("language").Value;
            author = xmlDocument.Root.Attribute("author").Value;
            version = xmlDocument.Root.Attribute("version").Value;

            foreach (XElement node in xmlDocument.Root.Elements())
            {
                string key = node.Attribute("key").Value;
                string value = node.Value;

                dictionary.Add(key, value);
            }
        }

        #endregion // ctor

        #region methods

        /// <summary>
        /// Do the translation using the public dictionary
        /// </summary>
        /// <param name="key">Key to search in the dictionary</param>
        /// <returns>Translated object</returns>
        internal object Translate(string key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            else
                return key;
        }

        #endregion // methods
    }
}
