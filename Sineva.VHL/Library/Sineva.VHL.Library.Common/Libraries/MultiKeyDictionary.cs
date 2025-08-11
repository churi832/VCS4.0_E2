using System;
using System.Collections.Generic;
using System.Linq;

namespace Sineva.VHL.Library.Common
{
    public class MultiKeyDictionary<K1, K2, V> : Dictionary<K1, Dictionary<K2, V>>
    {
        public V this[K1 key1, K2 key2]
        {
            get
            {
                if ((ContainsKey(key1) == false) || (this[key1].ContainsKey(key2) == false)) throw new ArgumentOutOfRangeException();
                return base[key1][key2];
            }
            set
            {
                if (ContainsKey(key1) == false) this[key1] = new Dictionary<K2, V>();
                this[key1][key2] = value;
            }
        }
        public void Add(K1 key1, K2 key2, V value)
        {
            if (ContainsKey(key1) == false) this[key1] = new Dictionary<K2, V>();
            this[key1][key2] = value;
        }
        public void Remove(K1 key1, K2 key2)
        {
            if (ContainsKey(key1, key2) == true)
            {
                this[key1].Remove(key2);
            }
        }
        public bool ContainsKey(K1 key1, K2 key2)
        {
            return base.ContainsKey(key1) && this[key1].ContainsKey(key2);
        }
        public new IEnumerable<V> Values
        {
            get
            {
                return from baseDictionary in base.Values
                       from baseKey in baseDictionary.Keys
                       select baseDictionary[baseKey];
            }
        }
        public MultiKeyDictionary<K1, K2, V> Clone()
        {
            try
            {
                MultiKeyDictionary<K1, K2, V> result = (MultiKeyDictionary<K1, K2, V>)base.MemberwiseClone();

                return result;
            }
            catch
            {
                return new MultiKeyDictionary<K1, K2, V>();
            }
        }
    }
}
