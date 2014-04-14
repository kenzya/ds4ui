using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TaskbarNotification;

namespace NotifyIconLibrary
{
    /// <summary>
    /// This class is used to interact with the trayIcon of every controller connected
    /// </summary>
    public class NotifyIconManager : INotifyIconManager
    {
        #region fields

        private readonly Dictionary<string, TaskbarIcon> dictionary;

        #endregion

        #region constructor

        public NotifyIconManager()
        {
            dictionary = new Dictionary<string, TaskbarIcon>();
        }
        
        #endregion

        #region methods

        /// <summary>
        /// Show the icon of a controller
        /// </summary>
        public void ShowIcon(string id)
        {
            if (!dictionary.ContainsKey(id))
                dictionary.Add(id, new TaskbarIcon());
            dictionary[id].Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hide the icon of a controller
        /// </summary>
        public void HideIcon(string id)
        {
            if (!dictionary.ContainsKey(id))
                dictionary.Add(id, new TaskbarIcon());
            dictionary[id].Visibility = Visibility.Hidden;  
        }

        /// <summary>
        /// Set the icon of a controller based on the battery value
        /// </summary>
        public void SetIcon(string id, bool isUsb, int value, bool isVisible)
        {
            if (!dictionary.ContainsKey(id))
                dictionary.Add(id, new TaskbarIcon());

            if (isVisible == false)
            {
                dictionary[id].Visibility = Visibility.Hidden;
                return;
            }
            else if (isUsb)
            {
                // TO DO: do animation from value to 100                                
                dictionary[id].Visibility = Visibility.Visible;
                dictionary[id].Icon = new Icon(Application.GetResourceStream(new Uri("/DS4Tool;component/Images/10C.ico", UriKind.Relative)).Stream);
            }
            else if (value >= 0 && value < 10)
            {
                dictionary[id].Visibility = Visibility.Visible;
                dictionary[id].Icon = new Icon(Application.GetResourceStream(new Uri("/DS4Tool;component/Images/0.ico", UriKind.Relative)).Stream);
            }
            else if (value >= 10 && value < 20)
            {
                dictionary[id].Visibility = Visibility.Visible;
                dictionary[id].Icon = new Icon(Application.GetResourceStream(new Uri("/DS4Tool;component/Images/10.ico", UriKind.Relative)).Stream);
            }
            else if (value >= 20 && value < 30)
            {
                dictionary[id].Visibility = Visibility.Visible;
                dictionary[id].Icon = new Icon(Application.GetResourceStream(new Uri("/DS4Tool;component/Images/20.ico", UriKind.Relative)).Stream);
            }
            else if (value >= 30 && value < 40)
            {
                dictionary[id].Visibility = Visibility.Visible;
                dictionary[id].Icon = new Icon(Application.GetResourceStream(new Uri("/DS4Tool;component/Images/30.ico", UriKind.Relative)).Stream);
            }
            else if (value >= 40 && value < 50)
            {
                dictionary[id].Visibility = Visibility.Visible;
                dictionary[id].Icon = new Icon(Application.GetResourceStream(new Uri("/DS4Tool;component/Images/40.ico", UriKind.Relative)).Stream);
            }
            else if (value >= 50 && value < 60)
            {
                dictionary[id].Visibility = Visibility.Visible;                
                dictionary[id].Icon = new Icon(Application.GetResourceStream(new Uri("/DS4Tool;component/Images/50.ico", UriKind.Relative)).Stream);
            }
            else if (value >= 60 && value < 70)
            {
                dictionary[id].Visibility = Visibility.Visible;
                dictionary[id].Icon = new Icon(Application.GetResourceStream(new Uri("/DS4Tool;component/Images/60.ico", UriKind.Relative)).Stream);
            }
            else if (value >= 70 && value < 80)
            {
                dictionary[id].Visibility = Visibility.Visible;
                dictionary[id].Icon = new Icon(Application.GetResourceStream(new Uri("/DS4Tool;component/Images/70.ico", UriKind.Relative)).Stream);
            }
            else if (value >= 80 && value < 90)
            {
                dictionary[id].Visibility = Visibility.Visible;
                dictionary[id].Icon = new Icon(Application.GetResourceStream(new Uri("/DS4Tool;component/Images/80.ico", UriKind.Relative)).Stream);
            }
            else if (value >= 90 && value < 100)
            {
                dictionary[id].Visibility = Visibility.Visible;
                dictionary[id].Icon = new Icon(Application.GetResourceStream(new Uri("/DS4Tool;component/Images/90.ico", UriKind.Relative)).Stream);
            }
            else if (value >= 100)
            {
                dictionary[id].Visibility = Visibility.Visible;
                dictionary[id].Icon = new Icon(Application.GetResourceStream(new Uri("/DS4Tool;component/Images/100.ico", UriKind.Relative)).Stream);
            }
        }

        #endregion
    }
}
