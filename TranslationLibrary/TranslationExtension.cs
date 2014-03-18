using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace TranslationLibrary
{
    /// <summary>
    /// The Translate Markup extension returns a binding to a TranslationData
    /// that provides a translated resource of the specified key
    /// </summary>
    public class TranslateExtension : MarkupExtension
    {
        #region properties

        private string key;

        [ConstructorArgument("key")]
        public string Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }

        #endregion // properties

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateExtension"/> class.
        /// </summary>
        /// <param name="key">The key referencing a value of the provider</param>
        public TranslateExtension(string key)
        {
            this.key = key;
        }

        #endregion // ctor

        #region methods

        /// <summary>
        /// See <see cref="MarkupExtension.ProvideValue" />
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Binding binding = new Binding("Value");
            binding.Source = new TranslationData(key);
            return binding.ProvideValue(serviceProvider);
        }

        #endregion // methods
    }
}
