/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.11 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sineva.VHL.Library
{
    public static class ObjectCopier
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source)
        {
            if (typeof(T).IsSerializable)
            {
                if (Object.ReferenceEquals(source, null)) return default(T);

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (T)formatter.Deserialize(stream);
                }
            }
            else
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }
        }

        public static class EnumUtil<T>
        {
            public static T Parse(string s)
            {
                return (T)Enum.Parse(typeof(T), s);
            }
        }
    }
}
