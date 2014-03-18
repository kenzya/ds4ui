using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace UserLibrary
{
    public class UserManager : IUserManager
    {
        public UserManager()
        { }

        public string GetUserName()
        {
            return WindowsIdentity.GetCurrent().Name.Split('\\').Last();
        }
        public string GetUserImage()
        {
            string username = WindowsIdentity.GetCurrent().Name;
            return GetUserTilePath(username);
        }

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
