using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace Sineva.OHT.Common
{
    [Serializable()]
    public abstract class Config
    {
        #region Fields
        protected string _Category = string.Empty;
        #endregion

        #region Properties
        [Browsable(false)]
        public string Category
        {
            get { return _Category; }
        }
        #endregion

        #region Constructors
        public Config()
        {
        }
        #endregion

        #region Methods
        public string GetValue(string fileName, params string[] args)
        {
            try
            {
                string result = string.Empty;

                XDocument document = XDocument.Load(fileName);
                result = GetNodeValue(document.FirstNode as XElement, 0, args);

                return result;
            }
            catch
            {
                return string.Empty;
            }
        }
        private string GetNodeValue(XElement node, int index, params string[] args)
        {
            try
            {
                string result = string.Empty;

                if (args.Length > index + 1)
                {
                    result = GetNodeValue(node.Element(args[index]), ++index, args);
                }
                else
                {
                    result = node.Element(args[index]).Value.ToString();
                }

                return result;
            }
            catch
            {
                return string.Empty;
            }
        }
        public virtual bool Save(string configPath)
        {
            try
            {
                PropertyInfo[] properties = this.GetType().GetProperties();
                string fileName = string.Format("{0}\\{1}.XML", configPath, _Category);

                XmlWriterSettings setting = new XmlWriterSettings();
                setting.Indent = true;
                setting.NewLineOnAttributes = true;

                XmlWriter xmlWriter = XmlWriter.Create(fileName);
                xmlWriter.WriteStartDocument();

                xmlWriter.WriteStartElement("root");
                bool needsEnd = false;
                foreach (PropertyInfo propertyInfo in properties)
                {
                    if (!propertyInfo.CanWrite) continue;
                    string saveName = propertyInfo.Name;
                    string typeName = propertyInfo.PropertyType.Name;
                    bool attributeFlag = false;
                    Attribute currentAttribute = propertyInfo.GetCustomAttribute(typeof(ConfigAttribute));
                    if (currentAttribute != null)
                    {
                        if (!string.IsNullOrWhiteSpace((currentAttribute as ConfigAttribute).SaveName))
                        {
                            saveName = (currentAttribute as ConfigAttribute).SaveName;
                        }
                        if ((currentAttribute as ConfigAttribute).IsElementAttribute)
                        {
                            attributeFlag = true;
                        }
                    }
                    switch (typeName)
                    {
                        case "Boolean":
                        case "String":
                        case "Int32":
                        case "Double":
                            if (attributeFlag)
                            {
                                xmlWriter.WriteStartElement(saveName);
                                needsEnd = true;
                                xmlWriter.WriteAttributeString(propertyInfo.Name, propertyInfo.GetValue(this).ToString());
                            }
                            else
                            {
                                xmlWriter.WriteElementString(saveName, propertyInfo.GetValue(this).ToString());
                            }
                            break;
                        case "FolderSelect":
                            xmlWriter.WriteElementString(saveName, propertyInfo.GetValue(this).ToString());
                            break;
                        case "Color":
                            System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter((Color)propertyInfo.GetValue(this));
                            xmlWriter.WriteElementString(saveName, converter.ConvertToString((Color)propertyInfo.GetValue(this)));
                            break;
                        case "LogFileSplitType":
                        case "Language":
                        case "MelsecUnitType":
                        case "MelsecProtocolType":
                        case "ProtocolType":
                        case "ConnectionType":
                        case "PathAlgorithm":
                            xmlWriter.WriteElementString(saveName, ((int)propertyInfo.GetValue(this)).ToString());
                            break;
                        case "List`1":
                            xmlWriter.WriteStartElement(saveName);
                            IList temp = (IList)propertyInfo.GetValue(this);
                            SaveIteration(xmlWriter, temp);
                            xmlWriter.WriteEndElement();
                            break;
                        default:
                            throw new Exception(saveName + " " + typeName);
                    }
                }
                if (needsEnd)
                {
                    xmlWriter.WriteEndElement();
                    needsEnd = false;
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();
                xmlWriter.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SaveIteration(XmlWriter xmlWriter, IList list)
        {
            try
            {
                foreach (var item in list)
                {
                    PropertyInfo[] subProperties = item.GetType().GetProperties();
                    bool needsEnd = false;
                    foreach (PropertyInfo propertyInfo in subProperties)
                    {
                        if (!propertyInfo.CanWrite) continue;
                        string saveName = propertyInfo.Name;
                        string typeName = propertyInfo.PropertyType.Name;
                        bool attributeFlag = false;
                        Attribute currentAttribute = propertyInfo.GetCustomAttribute(typeof(ConfigAttribute));
                        if (currentAttribute != null)
                        {
                            if (!string.IsNullOrWhiteSpace((currentAttribute as ConfigAttribute).SaveName))
                            {
                                saveName = (currentAttribute as ConfigAttribute).SaveName;
                            }
                            if ((currentAttribute as ConfigAttribute).IsElementAttribute)
                            {
                                attributeFlag = true;
                            }
                        }
                        switch (typeName)
                        {
                            case "Boolean":
                            case "String":
                            case "Int32":
                            case "Double":
                                if (attributeFlag)
                                {
                                    xmlWriter.WriteStartElement(saveName);
                                    needsEnd = true;
                                    xmlWriter.WriteAttributeString(propertyInfo.Name, propertyInfo.GetValue(item).ToString());
                                }
                                else
                                {
                                    xmlWriter.WriteElementString(saveName, propertyInfo.GetValue(item).ToString());
                                }
                                break;
                            case "Color":
                                System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter((Color)propertyInfo.GetValue(this));
                                xmlWriter.WriteElementString(saveName, converter.ConvertToString((Color)propertyInfo.GetValue(this)));
                                break;
                            case "LogFileSplitType":
                            case "Language":
                            case "MelsecUnitType":
                            case "MelsecProtocolType":
                            case "ProtocolType":
                            case "ConnectionType":
                            case "PathAlgorithm":
                                xmlWriter.WriteElementString(saveName, ((int)propertyInfo.GetValue(item)).ToString());
                                break;
                            case "List`1":
                                xmlWriter.WriteStartElement(saveName);
                                IList temp = (IList)propertyInfo.GetValue(item);
                                SaveIteration(xmlWriter, temp);
                                xmlWriter.WriteEndElement();
                                break;
                            default:
                                throw new Exception(saveName + " " + typeName);
                        }
                    }
                    if (needsEnd)
                    {
                        xmlWriter.WriteEndElement();
                        needsEnd = false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public virtual bool Load(string configPath)
        {
            try
            {
                PropertyInfo[] properties = this.GetType().GetProperties();
                Dictionary<string, string> saveNameMapToProperties = new Dictionary<string, string>();
                List<string> getAttributeAsValue = new List<string>();
                foreach (PropertyInfo propertyInfo in properties)
                {
                    Attribute currentAttribute = propertyInfo.GetCustomAttribute(typeof(ConfigAttribute));
                    if (currentAttribute != null)
                    {
                        if (!string.IsNullOrWhiteSpace((currentAttribute as ConfigAttribute).SaveName))
                        {
                            saveNameMapToProperties.Add((currentAttribute as ConfigAttribute).SaveName, propertyInfo.Name);
                        }
                        if ((currentAttribute as ConfigAttribute).IsElementAttribute)
                        {
                            getAttributeAsValue.Add(propertyInfo.Name);
                        }
                    }
                }
                string fileName = string.Format("{0}\\{1}.XML", configPath, _Category);
                if (File.Exists(fileName) == true)
                {
                    XmlTextReader xmlReader = new XmlTextReader(fileName);

                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            string saveName = xmlReader.Name.Trim();
                            string keyName = saveName;
                            if (saveNameMapToProperties.ContainsKey(keyName))
                            {
                                keyName = saveNameMapToProperties[keyName];
                            }
                            var currentProperty = properties.FirstOrDefault(o => o.Name == keyName);
                            if (currentProperty != null && currentProperty.CanWrite)
                            {
                                string propertyValue = string.Empty;
                                string typeName = currentProperty.PropertyType.Name;
                                if (typeName != "List`1")
                                {
                                    if (getAttributeAsValue.Contains(keyName))
                                    {
                                        propertyValue = xmlReader.GetAttribute(saveName);
                                    }
                                    else
                                    {
                                        propertyValue = xmlReader.ReadString();
                                    }
                                }
                                switch (typeName)
                                {
                                    case "String":
                                        currentProperty.SetValue(this, propertyValue, null);
                                        break;
                                    case "Double":
                                        currentProperty.SetValue(this, Convert.ToDouble(propertyValue), null);
                                        break;
                                    case "Boolean":
                                        currentProperty.SetValue(this, Convert.ToBoolean(propertyValue), null);
                                        break;
                                    case "Int32":
                                        currentProperty.SetValue(this, Convert.ToInt32(propertyValue), null);
                                        break;
                                    case "Color":
                                        Color color = (Color)System.Windows.Media.ColorConverter.ConvertFromString(propertyValue);
                                        currentProperty.SetValue(this, color, null);
                                        break;
                                    case "FolderSelect":
                                        currentProperty.SetValue(this, new FolderSelect() { SelectedFolder = propertyValue }, null);
                                        break;
                                    case "LogFileSplitType":
                                        currentProperty.SetValue(this, Enum.ToObject(typeof(LogFileSplitType), Convert.ToInt32(propertyValue)), null);
                                        break;
                                    case "Language":
                                        currentProperty.SetValue(this, Enum.ToObject(typeof(Language), Convert.ToInt32(propertyValue)), null);
                                        break;
                                    case "MelsecUnitType":
                                        currentProperty.SetValue(this, Enum.ToObject(typeof(MelsecUnitType), Convert.ToInt32(propertyValue)), null);
                                        break;
                                    case "MelsecProtocolType":
                                        currentProperty.SetValue(this, Enum.ToObject(typeof(MelsecProtocolType), Convert.ToInt32(propertyValue)), null);
                                        break;
                                    case "ProtocolType":
                                        currentProperty.SetValue(this, Enum.ToObject(typeof(System.Net.Sockets.ProtocolType), Convert.ToInt32(propertyValue)), null);
                                        break;
                                    case "ConnectionType":
                                        currentProperty.SetValue(this, Enum.ToObject(typeof(ConnectionType), Convert.ToInt32(propertyValue)), null);
                                        break;
                                    case "PathAlgorithm":
                                        currentProperty.SetValue(this, Enum.ToObject(typeof(PathAlgorithm), Convert.ToInt32(propertyValue)), null);
                                        break;
                                    case "List`1":
                                        string subClassName = currentProperty.PropertyType.FullName.Split(new char[2] { '[', ',' }, StringSplitOptions.RemoveEmptyEntries)[1];
                                        object subInstances = LoadIteration(xmlReader, subClassName, saveName, this.GetType().GetProperties().FirstOrDefault(o => o.Name == keyName).PropertyType);
                                        if (subInstances != null)
                                        {
                                            currentProperty.SetValue(this, subInstances, null);
                                        }
                                        break;
                                    default:
                                        throw new Exception(saveName + " " + typeName);
                                }
                            }
                        }
                    }
                    xmlReader.Close();
                }
                else
                {
                    Save(configPath);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public object LoadIteration(XmlTextReader xmlReader, string subClassName, string listEndName, Type listType)
        {
            try
            {
                object instance = Activator.CreateInstance(listType);
                IList returnObject = (IList)instance;
                bool stopFlag = false;
                while (!xmlReader.EOF)
                {
                    object subClassUnit = Assembly.GetAssembly(this.GetType()).CreateInstance(subClassName);

                    PropertyInfo[] properties = subClassUnit.GetType().GetProperties();
                    Dictionary<string, string> saveNameMapToProperties = new Dictionary<string, string>();
                    List<string> getAttributeAsValue = new List<string>();
                    foreach (PropertyInfo propertyInfo in properties)
                    {
                        Attribute currentAttribute = propertyInfo.GetCustomAttribute(typeof(ConfigAttribute));
                        if (currentAttribute != null)
                        {
                            if (!string.IsNullOrWhiteSpace((currentAttribute as ConfigAttribute).SaveName))
                            {
                                saveNameMapToProperties.Add((currentAttribute as ConfigAttribute).SaveName, propertyInfo.Name);
                            }
                            if ((currentAttribute as ConfigAttribute).IsElementAttribute)
                            {
                                getAttributeAsValue.Add(propertyInfo.Name);
                            }
                        }
                    }
                    string listStartNode = string.Empty;
                    bool getFirstNode = false;
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            string saveName = xmlReader.Name.Trim();
                            if (!getFirstNode)
                            {
                                listStartNode = saveName;
                                getFirstNode = true;
                            }
                            string keyName = saveName;
                            if (saveNameMapToProperties.ContainsKey(keyName))
                            {
                                keyName = saveNameMapToProperties[keyName];
                            }
                            var currentProperty = properties.FirstOrDefault(o => o.Name == keyName);
                            if (currentProperty != null && currentProperty.CanWrite)
                            {
                                string propertyValue = string.Empty;
                                string typeName = currentProperty.PropertyType.Name;
                                if (typeName != "List`1")
                                {
                                    if (getAttributeAsValue.Contains(keyName))
                                    {
                                        propertyValue = xmlReader.GetAttribute(keyName);
                                    }
                                    else
                                    {
                                        propertyValue = xmlReader.ReadString();
                                    }
                                }
                                switch (typeName)
                                {
                                    case "String":
                                        if (string.IsNullOrWhiteSpace(propertyValue))
                                        {
                                            if (saveName == "IPAddress")
                                            {
                                                propertyValue = "127.0.0.1";
                                            }
                                        }
                                        currentProperty.SetValue(subClassUnit, propertyValue, null);
                                        break;
                                    case "Boolean":
                                        if (string.IsNullOrWhiteSpace(propertyValue))
                                        {
                                            propertyValue = "false";
                                        }
                                        break;
                                    case "Int32":
                                        if (string.IsNullOrWhiteSpace(propertyValue))
                                        {
                                            if (saveName == "PortNumber")
                                            {
                                                propertyValue = "9000";
                                            }
                                        }
                                        currentProperty.SetValue(subClassUnit, Convert.ToInt32(propertyValue), null);
                                        break;
                                    case "Color":
                                        if (string.IsNullOrWhiteSpace(propertyValue))
                                        {
                                            propertyValue = "#00000000";
                                        }
                                        Color color = (Color)System.Windows.Media.ColorConverter.ConvertFromString(propertyValue);
                                        currentProperty.SetValue(this, color, null);
                                        break;
                                    case "MelsecUnitType":
                                        int melsecUnitType = 1;
                                        if (string.IsNullOrWhiteSpace(propertyValue)) melsecUnitType = 1;
                                        else melsecUnitType = Convert.ToInt32(propertyValue);
                                        currentProperty.SetValue(subClassUnit, Enum.ToObject(typeof(MelsecUnitType), melsecUnitType), null);
                                        break;
                                    case "MelsecProtocolType":
                                        int melsecProtocolType = 6;
                                        if (string.IsNullOrWhiteSpace(propertyValue)) melsecProtocolType = 6;
                                        else melsecProtocolType = Convert.ToInt32(propertyValue);
                                        currentProperty.SetValue(subClassUnit, Enum.ToObject(typeof(MelsecProtocolType), melsecProtocolType), null);
                                        break;
                                    case "ProtocolType":
                                        int protocolType = 6;
                                        if (string.IsNullOrWhiteSpace(propertyValue)) protocolType = 6;
                                        else protocolType = Convert.ToInt32(propertyValue);
                                        currentProperty.SetValue(subClassUnit, Enum.ToObject(typeof(System.Net.Sockets.ProtocolType), protocolType), null);
                                        break;
                                    case "ConnectionType":
                                        int connectionType = 6;
                                        if (string.IsNullOrWhiteSpace(propertyValue)) connectionType = 6;
                                        else connectionType = Convert.ToInt32(propertyValue);
                                        currentProperty.SetValue(subClassUnit, Enum.ToObject(typeof(ConnectionType), connectionType), null);
                                        break;
                                    case "List`1":
                                        string subClassIteration = currentProperty.PropertyType.FullName.Split(new char[2] { '[', ',' }, StringSplitOptions.RemoveEmptyEntries)[1];
                                        object subInstances = LoadIteration(xmlReader, subClassIteration, saveName, this.GetType().GetProperties().FirstOrDefault(o => o.Name == keyName).PropertyType);
                                        if (subInstances != null)
                                        {
                                            currentProperty.SetValue(subClassUnit, subInstances, null);
                                        }
                                        break;
                                    case "Double":
                                        currentProperty.SetValue(subClassUnit, Convert.ToDouble(propertyValue), null);
                                        break;
                                    default:
                                        throw new Exception(saveName + " " + typeName);
                                }
                            }
                        }
                        else if (xmlReader.NodeType == XmlNodeType.EndElement)
                        {
                            string saveName = xmlReader.Name.Trim();
                            if (saveName == listEndName)
                            {
                                stopFlag = true;
                                break;
                            }
                            if (saveName == listStartNode)
                            {
                                break;
                            }
                        }
                    }
                    if (stopFlag)
                    {
                        break;
                    }
                    else if (getFirstNode)
                    {
                        returnObject.Add(subClassUnit);
                    }
                }
                return returnObject;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public virtual Config GetCopyOrNull()
        //{
        //    try
        //    {
        //        Config clone = Assembly.GetAssembly(this.GetType()).CreateInstance(this.GetType().FullName) as Config;
        //        PropertyInfo[] properties = this.GetType().GetProperties();
        //        foreach (var property in properties)
        //        {
        //            var temp = property.GetValue(this);
        //            property.SetValue(clone, temp, null);
        //        }
        //        return clone;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        public virtual void SetCopy(Config source)
        {
            try
            {
                PropertyInfo[] properties = this.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var temp = property.GetValue(source);
                    property.SetValue(this, temp, null);
                }
            }
            catch(Exception e)
            { 
            }
        }
        public virtual bool CompareWith(Config config)
        {
            try
            {
                PropertyInfo[] properties = this.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var temp = property.GetValue(config);
                    var current = property.GetValue(this);
                    if (!temp.Equals(current))
                    {
                        if (property.PropertyType.Name == "FolderSelect")
                        {
                            if (temp.ToString().Equals(current.ToString()))
                            {
                                continue;
                            }
                        }
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
