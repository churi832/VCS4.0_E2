/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.10 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sineva.VHL.Library
{
    public class XmlHelper<T>
    {
        public Type m_Type;

        public XmlHelper()
        {
            m_Type = typeof(T);
        }

        public void Save(string path, object obj)
        {
            using (TextWriter textWriter = new StreamWriter(path))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(m_Type);
                    serializer.Serialize(textWriter, obj);
                }
                catch (Exception ex) { }
                textWriter.Close();
            }
        }

        public T Read(string path)
        {
            T result = default(T);
            using (TextReader textReader= new StreamReader(path))
            {
                try
                {
                    XmlSerializer deserializer = new XmlSerializer(m_Type);
                    result = (T)deserializer.Deserialize(textReader);
                }
                catch(Exception ex) { }
                textReader.Close();
            }
            return result;
        }
    }
}
