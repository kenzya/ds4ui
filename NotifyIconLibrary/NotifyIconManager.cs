using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace NotifyIconLibrary
{
    /// <summary>
    /// This class is used to interact with the trayIcon of every controller connected
    /// </summary>
    public class NotifyIconManager : INotifyIconManager
    {
        #region fields

        private readonly Dictionary<string, NotifyIcon> dictionary;

        #endregion

        #region constructor

        public NotifyIconManager()
        {            
            dictionary = new Dictionary<string, NotifyIcon>();
        }
        
        #endregion

        #region methods

        /// <summary>
        /// Show the icon of a controller
        /// </summary>
        public void ShowIcon(string id)
        {
            if (!dictionary.ContainsKey(id))
                dictionary.Add(id, new NotifyIcon());
            dictionary[id].Visible = true;            
        }

        /// <summary>
        /// Hide the icon of a controller
        /// </summary>
        public void HideIcon(string id)
        {
            if (!dictionary.ContainsKey(id))
                dictionary.Add(id, new NotifyIcon());
            dictionary[id].Visible = false;            
        }

        /// <summary>
        /// Set the icon of a controller based on the battery value
        /// </summary>
        public void SetIcon(string id, bool isUsb, int value, bool isVisible)
        {
            if (!dictionary.ContainsKey(id))
                dictionary.Add(id, new NotifyIcon());

            dictionary[id].Visible = isVisible;
            
            if (isVisible == false)
            {
                return;
            }
            else if (isUsb)
            {
                // TO DO: do animation from value to 100                                
                dictionary[id].Icon = Icon.FromHandle(((BitmapImage)System.Windows.Application.Current.Resources["Charging10"]).ToBitmap().GetHicon());
            }
            else if (value >= 0 && value < 10)
            {
                dictionary[id].Icon = Icon.FromHandle(((BitmapImage)System.Windows.Application.Current.Resources["Batt0"]).ToBitmap().GetHicon());
            }
            else if (value >= 10 && value < 20)
            {
                dictionary[id].Icon = Icon.FromHandle(((BitmapImage)System.Windows.Application.Current.Resources["Batt1"]).ToBitmap().GetHicon());
            }
            else if (value >= 20 && value < 30)
            {
                dictionary[id].Icon = Icon.FromHandle(((BitmapImage)System.Windows.Application.Current.Resources["Batt2"]).ToBitmap().GetHicon());
            }
            else if (value >= 30 && value < 40)
            {
                dictionary[id].Icon = Icon.FromHandle(((BitmapImage)System.Windows.Application.Current.Resources["Batt3"]).ToBitmap().GetHicon());
            }
            else if (value >= 40 && value < 50)
            {
                dictionary[id].Icon = Icon.FromHandle(((BitmapImage)System.Windows.Application.Current.Resources["Batt4"]).ToBitmap().GetHicon());
            }
            else if (value >= 50 && value < 60)
            {
                dictionary[id].Icon = Icon.FromHandle(((BitmapImage)System.Windows.Application.Current.Resources["Batt5"]).ToBitmap().GetHicon());
            }
            else if (value >= 60 && value < 70)
            {
                dictionary[id].Icon = Icon.FromHandle(((BitmapImage)System.Windows.Application.Current.Resources["Batt6"]).ToBitmap().GetHicon());
            }
            else if (value >= 70 && value < 80)
            {
                dictionary[id].Icon = Icon.FromHandle(((BitmapImage)System.Windows.Application.Current.Resources["Batt7"]).ToBitmap().GetHicon());
            }
            else if (value >= 80 && value < 90)
            {
                dictionary[id].Icon = Icon.FromHandle(((BitmapImage)System.Windows.Application.Current.Resources["Batt8"]).ToBitmap().GetHicon());
            }
            else if (value >= 90 && value < 100)
            {
                dictionary[id].Icon = Icon.FromHandle(((BitmapImage)System.Windows.Application.Current.Resources["Batt9"]).ToBitmap().GetHicon());
            }
            else if (value >= 100)
            {
                dictionary[id].Icon = Icon.FromHandle(((BitmapImage)System.Windows.Application.Current.Resources["Batt10"]).ToBitmap().GetHicon());
            }
        }

        #endregion
    }
}
