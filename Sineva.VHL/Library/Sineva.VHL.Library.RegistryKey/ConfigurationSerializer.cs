using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.RegistryKey
{
    public class ConfigurationSerializer : IConfigurationSerializer
    {
        #region Fields
        private Microsoft.Win32.RegistryKey m_CurrentRegistryKey;
        #endregion

        #region Constructor
        public ConfigurationSerializer()
        {
            if (m_CurrentRegistryKey == null) m_CurrentRegistryKey = OpenRegKey();
        }
        #endregion

        #region Methods
        public Microsoft.Win32.RegistryKey OpenRegKey()
        {
            string path = System.Environment.Is64BitOperatingSystem ? @"SOFTWARE\Wow6432Node\Sineva.VHL.Library.RegistryKey" : @"SOFTWARE\Sineva.VHL.Library.RegistryKey";
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path, true);
            if (key == null)
            {
                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(path);
            }

            if (key == null)
            {
                return null;
            }

            Microsoft.Win32.RegistryKey settingsKey = key.OpenSubKey("RawDataSettings", true);
            if (settingsKey == null)
            {
                settingsKey = key.CreateSubKey("RawDataSettings");
            }

            return settingsKey;
        }

        public string[] ReadEntryNames()
        {
            //return m_CurrentRegistryKey.GetSubKeyNames();
            return m_CurrentRegistryKey.GetValueNames();
        }
        #endregion

        #region Model.IConfigurationSerialiser
        public void WriteEntry<T>(string key, T value)
        {
            if (value != null && m_CurrentRegistryKey != null)
            {
                m_CurrentRegistryKey.SetValue(key, value.ToString());
            }
        }

        public T ReadEntry<T>(string key, T value)
        {
            try
            {
                if (typeof(T).IsEnum)
                {
                    string text = ReadEntry(key, value.ToString());
                    if (text != null)
                    {
                        T temp = value;
                        value = (T)Enum.Parse(typeof(T), text, true);
                    }
                }
                else
                {
                    if (m_CurrentRegistryKey != null)
                        value = LoadRegistryEntry<T>(m_CurrentRegistryKey, value, key);
                }
            }
            catch
            {

            }
            return value;
        }
        public static T LoadRegistryEntry<T>(Microsoft.Win32.RegistryKey key, T value, string valueName)
        {
            if (key != null)
            {
                object obj = key.GetValue(valueName);
                if (obj != null)
                {
                    value = (T)Convert.ChangeType(obj, typeof(T));
                }
            }

            return value;
        }
        #endregion Model.IConfigurationSerialiser
    }
}
