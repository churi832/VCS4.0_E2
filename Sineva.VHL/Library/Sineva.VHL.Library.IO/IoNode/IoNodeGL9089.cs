using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.IO
{
    [Serializable()]
    public class IoNodeGL9089 : IoNodeCrevisModbus
    {
        public IoNodeGL9089()
        {
            m_Name = "GL-9089";
            m_ProductNo = 0x9089;
            m_MaxSlotNo = 16;
            m_Description = "Crevis Network Adapter G-Series, Modbus TCP/UDP (16 Slots)";
        }
    }
}
