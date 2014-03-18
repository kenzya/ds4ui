using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace TranslationLibrary
{
    /// <summary>
    /// The TranslationManager's job is to instantiate and retrieve the collection of TranslationProviders
    /// </summary>
    public class TranslationManager : ITranslationManager
    {
        #region singleton

        private static TranslationManager translationManager;
        internal static TranslationManager Instance
        {
            get
            {
                return translationManager;
            }
        }

        #endregion // singleton

        #region Constructor

        public TranslationManager()
        {
            translationManager = this;
        }
        public TranslationManager(string path)
        {
            translationManager = this;
            Initialize(path);
        }

        #endregion // Constructor

        #region properties

        private Collection<XmlTranslationProvider> translationCollection;
        private Collection<XmlTranslationProvider> TranslationCollection
        {
            get
            {
                if (translationCollection == null)
                {
                    translationCollection = new Collection<XmlTranslationProvider>();
                }
                return translationCollection;
            }
        }

        private XmlTranslationProvider selectedTranslationProvider;
        private XmlTranslationProvider SelectedTranslationProvider
        {
            get
            {
                return selectedTranslationProvider;
            }
            set
            {
                selectedTranslationProvider = value;
                OnLanguageChanged();
            }
        }

        #endregion // properties

        #region events

        /// <summary>
        /// Event raised on TranslationProvider change
        /// </summary>
        public event EventHandler LanguageChanged;
        private void OnLanguageChanged()
        {
            if (LanguageChanged != null)
            {
                LanguageChanged(this, EventArgs.Empty);
            }
        }

        #endregion // events

        #region methods

        /// <summary>
        /// Scan a folder for every XML files representing a language
        /// </summary>
        /// <param name="path">Folder's path</param>
        public void Initialize(string path)
        {
            foreach (string file in Directory.GetFiles(path, "*.xml"))
            {
                XmlTranslationProvider provider = new XmlTranslationProvider(file);
                TranslationCollection.Add(provider);
            }
        }

        /// <summary>
        /// Change the TranslationProvider
        /// </summary>
        /// <param name="language">New LanguageProvider</param>
        public void ChangeLanguage(string language)
        {
            SelectedTranslationProvider = TranslationCollection.Single(x => x.language == language);
        }

        /// <summary>
        /// Get the lists of languages available
        /// </summary>
        /// <returns>List of language</returns>
        public IEnumerable<string> GetLanguagesList()
        {
            Collection<string> collection = new Collection<string>();
            foreach (XmlTranslationProvider provider in TranslationCollection)
            {
                collection.Add(provider.language);
            }
            return collection;
        }

        /// <summary>
        /// Use the selected provider to do the translation
        /// </summary>
        /// <param name="key">Key to search in the dictionary of the provider</param>
        /// <returns>Translated object returned by the provider</returns>
        public object Translate(string key)
        {
            if (SelectedTranslationProvider != null)
            {
                object value = SelectedTranslationProvider.Translate(key);
                if (value != null)
                {
                    return value;
                }
            }
            return string.Format("!{0}!", key);
        }

        #endregion // methods
    }
}
