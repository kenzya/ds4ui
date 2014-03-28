
namespace ControllerConfigurationLibrary
{
    /// <summary>
    /// Interface implemented by the configuration
    /// </summary>
    public interface IControllerConfigurationManager
    {
        void SetData(string id, string key, string value);      
        string GetData(string id, string key);
        string GetDefault(string id, string key);
    }
}
