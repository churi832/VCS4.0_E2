using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.RegistryKey
{
    public interface IConfigurationSerializer
    {
        void WriteEntry<T>(string key, T value);
        T ReadEntry<T>(string key, T value);
    }
}
