using System.Collections.Generic;

namespace TranslationLibrary
{
    /// <summary>
    /// Interface of TranslationManager
    /// </summary>
    public interface ITranslationManager
    {
        void Initialize(string path);
        void ChangeLanguage(string language);
        IEnumerable<string> GetLanguagesList();
        object Translate(string key);
    }
}
