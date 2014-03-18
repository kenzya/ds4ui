
namespace ControllerConfigurationLibrary
{
    public interface IControllerConfigurationManager
    {
        void Initialize(string path);
        
        void SetData(string key, string name, string value);
        
        string GetData(string key, string name);
       
        string GetDefault(string key, string name);
    }
}
