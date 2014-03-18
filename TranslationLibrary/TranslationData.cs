using System;
using System.ComponentModel;
using System.Windows;

namespace TranslationLibrary
{
    /// <summary>
    /// Resource used by the Translate Markup extension to obtain the translated value
    /// </summary>
    internal class TranslationData : IWeakEventListener, INotifyPropertyChanged
    {
        #region private members

        private string key;

        #endregion // private members

        #region properties

        public object Value
        {
            get
            {
                return TranslationManager.Instance.Translate(key);
            }
        }

        #endregion // properties

        #region ctor & dtor

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationData"/> class.
        /// </summary>
        /// <param name="key">The key referencing a value of the provider</param>
        internal TranslationData(string key)
        {
            this.key = key;
            LanguageChangedEventManager.AddListener(TranslationManager.Instance, this);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="TranslationData"/> is reclaimed by garbage collection.
        /// </summary>
        ~TranslationData()
        {
            LanguageChangedEventManager.RemoveListener(TranslationManager.Instance, this);
        }

        #endregion // ctor & dtor

        #region IWeakEventListener members

        /// <summary>
        /// WeakEvent implementation
        /// </summary>
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(LanguageChangedEventManager))
            {
                OnLanguageChanged(sender, e);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Update the Value when a new provider is selected
        /// </summary>
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        #endregion // IWeakEventListener members

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged members
    }
}
