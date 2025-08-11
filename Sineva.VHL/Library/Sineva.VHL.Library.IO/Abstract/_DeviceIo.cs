using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library;

namespace Sineva.VHL.Library.IO
{
    [Serializable]
    public abstract class _DeviceIo : _Device
    {
        private IoChannel m_IoItem = null;
        private string m_ChannelName = string.Empty;
        private bool m_Initialized = false;

        //[XmlIgnore()]
        public IoChannel IoItem
        {
            get { return m_IoItem; }
            set { m_IoItem = value; }
        }

        public abstract void SetIoChannel(IoChannel ch);
        public abstract IoChannel GetIoChannel();
        public abstract IoType GetIoType();
        public abstract string GetStateString();

        public virtual void Initialize()
        {
            if(m_Initialized) return;

            if(m_IoItem != null)
            {
                IoChannel instance = IoManager.Instance.GetChannelByName(m_IoItem.IoType, m_IoItem.Name);
                m_IoItem = instance;
            }
        }
        public virtual void SetOn()
        {
            Initialize();
            if(m_Initialized)
            {
                if(m_IoItem.IoType == IoType.DO) m_IoItem.SetDo(true);
                else if(m_IoItem.IoType == IoType.DI) m_IoItem.SetDo(true);
            }
        }
        public virtual void SetOff()
        {
            Initialize();
            if(m_Initialized)
            {
                if(m_IoItem.IoType == IoType.DO) m_IoItem.SetDo(false);
                else if(m_IoItem.IoType == IoType.DI) m_IoItem.SetDo(false);
            }
        }
        public virtual void SetValue(double val)
        {
            Initialize();
            if(m_Initialized)
            {
                if(m_IoItem.IoType == IoType.AO) m_IoItem.SetAo(val);
                else if(m_IoItem.IoType == IoType.AI) m_IoItem.SetAo(val);
            }
        }
        public virtual double GetValue()
        {
            Initialize();
            if(m_Initialized)
            {
                if(m_IoItem.IoType == IoType.AI) return m_IoItem.GetAi();
                else if(m_IoItem.IoType == IoType.AO) return m_IoItem.GetAi();
            }
            return 0;
        }
        public virtual bool IsOn()
        {
            Initialize();
            if(m_Initialized)
            {
                if(m_IoItem.IoType == IoType.DI) return m_IoItem.GetDi();
                else if(m_IoItem.IoType == IoType.DO) return m_IoItem.GetDi();
            }
            return false;
        }

        public override string ToString()
        {
            if(string.IsNullOrEmpty(m_ChannelName) && GetIoChannel() != null)
            {
                m_ChannelName = GetIoChannel().ToString();
            }

            return string.IsNullOrEmpty(m_ChannelName) ? "(Null)" : m_ChannelName;
        }
    }
}
