using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MahApps.Metro;

namespace ThemeLibrary
{
    /// <summary>
    /// This class is used to handle separately everythings theme related like
    /// accents, theme and images color.
    /// </summary>
    public class ThemeManager : IThemeManager
    {
        #region ctor

        public ThemeManager()
        { 
        }

        #endregion // ctor

        /// <summary>
        /// Set the style of the application. It change the accent, the theme and the images
        /// </summary>
        public void SetTheme(Application app, string accent, string theme)
        {
            Accent a = MahApps.Metro.ThemeManager.DefaultAccents.Single(x => x.Name == accent);
            Theme t = (Theme)Enum.Parse(typeof(Theme), theme);
            MahApps.Metro.ThemeManager.ChangeTheme(app, a, t);
            
            if (t == Theme.Dark)
            {
                Application.Current.Resources["DS4Image"] = Application.Current.Resources["DS4ImageWhite"];
                Application.Current.Resources["USBImage"] = Application.Current.Resources["USBImageWhite"];
                Application.Current.Resources["BTImage"] = Application.Current.Resources["BTImageWhite"];
            }
            else
            {
                Application.Current.Resources["DS4Image"] = Application.Current.Resources["DS4ImageBlack"];
                Application.Current.Resources["USBImage"] = Application.Current.Resources["USBImageBlack"];
                Application.Current.Resources["BTImage"] = Application.Current.Resources["BTImageBlack"];
            }
        }

        /// <summary>
        /// Get the list of accents available
        /// </summary>
        public IEnumerable<string> GetAccentList()
        {
            return MahApps.Metro.ThemeManager.DefaultAccents.Select(x => x.Name).ToList();
        }

        /// <summary>
        /// Get the list of themes available
        /// </summary>
        public IEnumerable<string> GetThemeList()
        {
            return Enum.GetNames(typeof(Theme)).ToList();
        }
    }
}
