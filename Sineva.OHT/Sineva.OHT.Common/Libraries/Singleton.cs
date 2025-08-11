using System;
using System.Reflection;

namespace Sineva.OHT.Common
{
    public class Singleton<T> where T : class
    {
        #region Fields
        private static volatile T _Instance = default(T);
        #endregion

        #region Properties
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (typeof(T))
                    {
                        if (_Instance == null)
                        {
                            CreateInstance();
                        }
                    }
                }

                return _Instance;
            }
        }
        #endregion

        #region Constructors
        protected Singleton()
        {
        }
        #endregion

        #region Methods
        private static void CreateInstance()
        {
            Type type = typeof(T);
            ConstructorInfo[] constructors = type.GetConstructors();

            if (constructors.Length > 0)
            {
                throw new InvalidOperationException(string.Format("{0} type has public constructor.", type.Name));
            }

            _Instance = (T)Activator.CreateInstance(type, true);
        }
        #endregion
    }
}
