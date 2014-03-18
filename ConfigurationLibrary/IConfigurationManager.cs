
namespace ConfigurationLibrary
{
    public interface IConfigurationManager
    {
        void Initialize(string path);
        string GetData(string key);
        string GetDefault(string key);
        void SetData(string key, string value);
    }
}
