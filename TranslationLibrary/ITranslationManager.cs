using System.Collections.Generic;

namespace TranslationLibrary
{
    public interface ITranslationManager
    {
        void Initialize(string path);
        void ChangeLanguage(string language);
        IEnumerable<string> GetLanguagesList();
        object Translate(string key);
    }
}
