﻿using System.Collections.Generic;
using System.Windows;

namespace ThemeLibrary
{
    /// <summary>
    /// Interface of the ThemeManager
    /// </summary>
    public interface IThemeManager
    {
        void SetTheme(string accent, string theme);

        IEnumerable<string> GetAccentList();

        IEnumerable<string> GetThemeList();
    }
}
