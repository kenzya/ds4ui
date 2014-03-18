
namespace NotifyIconLibrary
{
    public interface INotifyIconManager
    {
        void ShowIcon(int id);
        
        void HideIcon(int id);

        void SetIcon(int id, bool isUsb, int value, bool isVisible);
    }
}
