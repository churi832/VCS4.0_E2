using System.Runtime.InteropServices;
using System.Text;

namespace Sineva.OHT.Common
{
    public class IniProfile
    {
        #region InterlopServices
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);
        #endregion

        public static string GetPrivateProfileString(string section, string key, string defaultValue, string fileName)
        {
            StringBuilder sb = new StringBuilder();
            int rv = GetPrivateProfileString(section, key, "ERROR", sb, 255, fileName);
            string returnValue = sb.ToString().Trim();

            return returnValue;
        }

        public static void SetPrivateProfileString(string section, string key, string value, string fileName)
        {
            WritePrivateProfileString(section, key, value, fileName);
        }
    }
}
