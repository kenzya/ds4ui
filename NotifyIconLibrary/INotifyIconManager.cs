namespace NotifyIconLibrary
{
    /// <summary>
    /// Interface of the NotifyIconManager
    /// </summary>
    public interface INotifyIconManager
    {
        void ShowIcon(string id);
        
        void HideIcon(string id);

        void SetIcon(string id, bool isUsb, int value, bool isVisible);
    }
}
