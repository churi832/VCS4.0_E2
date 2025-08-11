/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.13 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Runtime.InteropServices;
using System.Net;

namespace Sineva.VHL.Library
{
    // "C" struct IN_ADDR
    [StructLayout(LayoutKind.Sequential)]
    public struct _C_IN_ADDR
    {
        public Anonymous1 S_un;

        [StructLayoutAttribute(LayoutKind.Explicit)]
        public struct Anonymous1
        {
            [FieldOffsetAttribute(0)]
            public Anonymous2 S_un_b;

            [FieldOffsetAttribute(0)]
            public Anonymous3 S_un_w;

            [FieldOffsetAttribute(0)]
            public uint S_addr;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct Anonymous2
        {
            public byte s_b1;
            public byte s_b2;
            public byte s_b3;
            public byte s_b4;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct Anonymous3
        {
            public ushort s_w1;
            public ushort s_w2;
        }

        public _C_IN_ADDR(IPAddress address) : this(address.GetAddressBytes()) { }

        public _C_IN_ADDR(byte[] address)
        {
            // Set this first, otherwise it wipes out the other fields
            S_un.S_un_w = new Anonymous3();

            S_un.S_addr = (uint)BitConverter.ToInt32(address, 0);

            S_un.S_un_b.s_b1 = address[0];
            S_un.S_un_b.s_b2 = address[1];
            S_un.S_un_b.s_b3 = address[2];
            S_un.S_un_b.s_b4 = address[3];
        }

        /// <summary>
        /// Unpacks an in_addr struct to an IPAddress object
        /// </summary>
        /// <returns></returns>
        public IPAddress ToIPAddress()
        {
            byte[] bytes = new[] { S_un.S_un_b.s_b1, S_un.S_un_b.s_b2, S_un.S_un_b.s_b3, S_un.S_un_b.s_b4 };
            return new IPAddress(bytes);
        }
    }
}
