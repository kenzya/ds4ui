
namespace ControllerConfigurationLibrary
{
    /// <summary>
    /// Interface implemented by the configuration
    /// </summary>
    public interface IControllerConfigurationManager
    {
        void SetData(string key, string name, string value);      
        string GetData(string key, string name);
        string GetDefault(string key, string name);
    }
}
