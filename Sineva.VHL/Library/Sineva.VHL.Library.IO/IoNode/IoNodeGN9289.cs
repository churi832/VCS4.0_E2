using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.IO
{
    [Serializable()]
    public class IoNodeGN9289 : IoNodeCrevisModbus
    {
        public IoNodeGN9289()
        {
            m_Name = "GN-9289";
            m_ProductNo = 0x9289;
            m_MaxSlotNo = 63;
            m_Description = "Crevis Network Adapter G-Series, Modbus TCP/UDP (63 Slots)";
        }
    }
}
