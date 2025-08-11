using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library;

namespace Sineva.VHL.Library.Remoting
{
    [Serializable]
    public class RemoteObject : MarshalByRefObject
    {
        #region Fields
        private static int m_Ping = 0;
        private static List<GeneralObject> m_Items = new List<GeneralObject>();
        private TouchGUI m_TouchGUI = new TouchGUI();
        private PadRemoteGUI m_RemoteGUI = new PadRemoteGUI();
        #endregion

        #region Properties
        public int Ping
        {
            get { return m_Ping; }
            set { m_Ping = value; }
        }

        public TouchGUI TouchGUI
        {
            get { return m_TouchGUI; }
            set { m_TouchGUI = value; }
        }

        public PadRemoteGUI RemoteGUI
        {
            get => m_RemoteGUI;
            set => m_RemoteGUI = value;
        }
        #endregion

        #region Constructor
        public RemoteObject()
        {
        }
        #endregion
        #region Methods
        public int AddItem(GeneralObject item)
        {
            try
            {
                if (m_Items != null) m_Items.Add(item);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }

            return m_Items.Count;
        }
        public int RemoveItem(GeneralObject item)
        {
            try
            {
                if (m_Items.Contains(item)) m_Items.Remove(item);
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
            
            return m_Items.Count;
        }
        public void Clear()
        {
            try
            {
                m_Items.Clear();
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase method = System.Reflection.MethodBase.GetCurrentMethod();
                ExceptionLog.WriteLog(string.Format("[{0}.{1}]\n{2}", method.ReflectedType.FullName, method.Name, ex.ToString()));
            }
        }
        public bool Contains(GeneralObject item)
        {
            return m_Items.Contains(item);
        }
        public GeneralObject GetItem(string name)
        {
            GeneralObject item = m_Items.Where(x => x.Name == name).First();
            return item;
        }
        #endregion
    }
}
