using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;

namespace Sineva.VHL.Library.IO
{
    [Editor(typeof(UIEditorDeviceIoSelect), typeof(UITypeEditor))]
    [Serializable]
    public class DevIoCollection<T> : IDevIoCollection
    {
        #region Enumerator
        public IEnumerator GetEnumerator()
        {
            return new InnerEnumerator(this);
        }
        private class InnerEnumerator : IEnumerator
        {
            private int m_Index = -1;
            private DevIoCollection<T> m_Collection;

            public InnerEnumerator(DevIoCollection<T> collection)
            {
                m_Collection = collection;
            }

            public bool MoveNext()
            {
                if(m_Index < m_Collection.Count - 1)
                {
                    m_Index++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                m_Index = -1;
            }
            public object Current
            {
                get
                {
                    return m_Collection.Items[m_Index];
                }
            }
        }
        #endregion

        #region Field
        private List<T> m_Items = new List<T>();
        private Type m_ContainerType;
        private IoType m_DevIoType = IoType.DI;
        #endregion

        #region Property
        public string Name
        {
            get { return "Collection<" + m_ContainerType.Name + ">"; }
        }
        public List<T> Items
        {
            get { return m_Items; }
            set { m_Items = value; }
        }
        public Type ContainerType
        {
            get { return m_ContainerType; }
        }
        public IoType DevIoType
        {
            get { return m_DevIoType; }
        }
        public int Count
        {
            get { return m_Items.Count; }
        }
        #endregion

        #region Constructor
        public DevIoCollection()
        {
            m_ContainerType = typeof(T);
            if(m_ContainerType == typeof(DevIoDigitalInput)) m_DevIoType = IoType.DI;
            else if(m_ContainerType == typeof(DevIoDigitalOutput)) m_DevIoType = IoType.DO;
            else if(m_ContainerType == typeof(DevIoAnalogInput)) m_DevIoType = IoType.AI;
            else if(m_ContainerType == typeof(DevIoAnalogOutput)) m_DevIoType = IoType.AO;
            else
            {
                throw new Exception("DevIoCollection contains only '_DeviceIo' class");
            }
        }
        #endregion

        #region Interface
        public virtual void Clear()
        {
            m_Items.Clear();
        }
        public virtual void Add(object item)
        {
            m_Items.Add((T)item);
        }
        public virtual void Insert(int index, object item)
        {
            m_Items.Insert(index, (T)item);
        }
        public virtual void Remove(object item)
        {
            m_Items.Remove((T)item);
        }
        public virtual void RemoveAt(int index)
        {
            m_Items.RemoveAt(index);
        }
        public ICollection CreateCollection()
        {
            return new DevIoCollection<T>();
        }
        public _DeviceIo CreateNewItem()
        {
            return (_DeviceIo)Activator.CreateInstance(typeof(T));
        }
        public virtual object GetItem(int index)
        {
            return m_Items[index];
        }
        #endregion

        public object this[int index]
        {
            get
            {
                if(index >= 0 && index < m_Items.Count) return m_Items[index];
                else return default(T);
            }
            set
            {
                if(index >= 0 && index < m_Items.Count) m_Items[index] = (T)value;
            }
        }
        public object this[string name]
        {
            get
            {
                try { for(int i = 0; i < m_Items.Count; i++) if((m_Items[i] as _Device).MyName == name) return m_Items[i]; }
                catch { }
                return default(T);
            }
            set
            {
                if(this[name].Equals(default(T)) == false)
                    this[name] = value;
            }
        }

        public override string ToString()
        {
            return this.Name + string.Format(" ({0} Items)", this.Count);
        }

        #region Method T
        public bool IsOn()
        {
            bool rv = true;
            foreach(T item in m_Items)
                rv &= (item as _DeviceIo).IsOn();

            return rv;
        }
        public bool IsOn(int index)
        {
            return (m_Items[index] as _DeviceIo).IsOn();
        }
        public bool IsOff()
        {
            bool rv = true;
            foreach(T item in m_Items)
            {

                rv &= !(item as _DeviceIo).IsOn();
            }

            return rv;
        }
        public bool IsOff(int index)
        {
            return !(m_Items[index] as _DeviceIo).IsOn();
        }
        public bool[] GetDiStates()
        {
            List<bool> state = new List<bool>();
            foreach(T item in m_Items)
                state.Add((item as _DeviceIo).IsOn());

            return state.ToArray();
        }
        public double[] GetValues()
        {
            List<double> values = new List<double>();
            foreach(T item in m_Items)
                values.Add((item as _DeviceIo).GetValue());

            return values.ToArray();
        }
        public double GetValue(int index)
        {
            return (m_Items[index] as _DeviceIo).GetValue();
        }
        #endregion
    }
}
