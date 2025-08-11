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
    public class Comparison
    {
        /// <summary>
        /// Forward order sort for double type variable
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int ForwardDouble(double a, double b)
        {
            int rv = a.CompareTo(b);
            return rv;
        }
        /// <summary>
        /// Inverse order sort for double type variable
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int BackwardDouble(double a, double b)
        {
            int rv = b.CompareTo(a);
            return rv;
        }
        /// <summary>
        /// Forward order sort for int type variable
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int ForwardInt(Int32 a, Int32 b)
        {
            int rv = a.CompareTo(b);
            return rv;
        }
        /// <summary>
        /// Inverse order sort for int type variable
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int BackwardInt(Int32 a, Int32 b)
        {
            int rv = b.CompareTo(a);
            return rv;
        }
        /// <summary>
        /// Forward order sort for uint type variable
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int ForwardUInt(UInt32 a, UInt32 b)
        {
            int rv = a.CompareTo(b);
            return rv;
        }
        /// <summary>
        /// Inverse order sort for uint type variable
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int BackwardUInt(UInt32 a, UInt32 b)
        {
            int rv = b.CompareTo(a);
            return rv;
        }
    }
}
