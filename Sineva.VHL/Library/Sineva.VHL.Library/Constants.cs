/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.13 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library
{
    public static class Constants
    {
        public const double Pi = 3.141592;

        public const byte STX = 0x02;
        public const byte ETX = 0x03;
        public const byte ACK = 0x05;
        public const byte CR = 0x0D;
        public const byte LF = 0x0A;
        public const byte ESC = 0x1B;

        /// <summary>
        /// PSR Command/////////////////////////////////////
        /// </summary>
        public const byte SET_SV = 0x90;
        public const byte GET_SV = 0xD0;
        public const byte SET_PERCENT = 0x98;
        public const byte GET_PERCENT = 0xD8;
        public const byte GET_PV = 0x70;
        //////////////////////////////////////
        ///
        public const int INFINITE = 100000000; // PathFinder에 사용
    }
}
