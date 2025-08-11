using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.RegistryKey
{
    public class RegistryConfiguration
    {
        #region Singleton
        public static readonly RegistryConfiguration Instance = new RegistryConfiguration();
        private bool m_Initialized = false;
        #endregion

        #region Serializer
        ConfigurationSerializer configurationSerializer;
        #endregion

        #region Event
        public event ConfigurationChangedEventHandler ConfigurationChanged;
        #endregion

        #region Constructor
        public RegistryConfiguration()
        {
            Initialize();
        }
        #endregion

        public bool Initialize()
        {
            if (!m_Initialized)
            {
                configurationSerializer = new ConfigurationSerializer();
                m_Initialized = true;
            }
            return m_Initialized;
        }

        public void WriteEntry<T>(string key, T value)
        {
            configurationSerializer.WriteEntry(key, value);
        }

        public T ReadEntry<T>(string key, T value)
        {
            return configurationSerializer.ReadEntry(key, value);
        }

        public string[] GetEntryNames()
        {
            return configurationSerializer.ReadEntryNames();
        }
    }
}
