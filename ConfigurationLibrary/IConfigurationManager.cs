
namespace ConfigurationLibrary
{
    /// <summary>
    /// Interface implemented by the configuration
    /// </summary>
    public interface IConfigurationManager
    {
        string GetData(string key);
        string GetDefault(string key);
        void SetData(string key, string value);
    }
}
