using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace UserLibrary
{
    /// <summary>
    /// This class is used to retrieve information of the WindowsUser launching the application
    /// </summary>
    public class UserManager : IUserManager
    {
        #region ctor

        public UserManager()
        { }

        #endregion

        /// <summary>
        /// Get the username of the user launching the application
        /// </summary>
        public string GetUserName()
        {
            return WindowsIdentity.GetCurrent().Name.Split('\\').Last();
        }

        /// <summary>
        /// Get the image of the user launching the application
        /// </summary>
        public string GetUserImage()
        {
            string username = WindowsIdentity.GetCurrent().Name;
            return GetUserTilePath(username);
        }

        /// <summary>
        /// Sometimes it fails to retrieve the user image located in the user directory.
        /// So using this method is more secure.
        /// </summary>
        [DllImport("shell32.dll", EntryPoint = "#261", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern void GetUserTilePath(string username, UInt32 whatever, StringBuilder picpath, int maxLength);
        private static string GetUserTilePath(string username)
        {
            StringBuilder sb = new StringBuilder(1000);
            GetUserTilePath(username, 0x80000000, sb, sb.Capacity);
            return sb.ToString();
        }
    }
}
