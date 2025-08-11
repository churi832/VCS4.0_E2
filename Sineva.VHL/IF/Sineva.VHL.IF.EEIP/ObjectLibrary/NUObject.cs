using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.OHT.IF.EEIP.NET
{
    public class NUObject
    {
        public EEIPClient eeipClient;

        /// <summary>
        /// Constructor. </summary>
        /// <param name="eeipClient"> EEIPClient Object</param>
        public NUObject(EEIPClient eeipClient)
        {
            this.eeipClient = eeipClient;
        }

        /// <summary>
        /// Reads the Instance of the Assembly Object (Instance 101 returns the bytes of the class ID 101)
        /// </summary>
        /// <param name="Attribute ID"> Instance number to be returned</param>
        /// <returns>bytes of the Instance</returns>
        public byte[] getInstance(int attributeNo)
        {

            byte[] byteArray = eeipClient.GetAttributeSingle(0x66, 0x01, attributeNo);
            return byteArray;
        }

        /// <summary>
        /// Sets an Instance of the Assembly Object
        /// </summary>
        /// <param name="instanceNo"> Instance number to be returned</param>
        /// <returns>bytes of the Instance</returns>
        public void setInstance(int attributeNo, byte[] value)
        {

            eeipClient.SetAttributeSingle(0x66, 0x01, attributeNo, value);
        }

    }
}
