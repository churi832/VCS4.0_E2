/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.11 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;

namespace Sineva.VHL.Library
{
    public class UnitConversion
    {
        public static double Newton2Kg(double N)
        {
            //return 0.102f * N;
            return 0.10197f * N;
        }

        public static double Kg2Newton(double kg)
        {
            //return 9.807f * kg;
            return (1.0f / 0.10197f) * kg;
        }
        public static double Newton2g(double N)
        {
            //return 102.0408f * N;
            return 101.97f * N;
        }
        public static double g2Newton(double g)
        {
            //return 0.0098f * g;
            return (1.0f / 101.97f) * g;
        }

        public static double mm2um(double mm)
        {
            return 1000.0f * mm;
        }

        public static double um2mm(double um)
        {
            return 0.001f * um;
        }

        public static double Degree2Radian(double angle)
        {
            return angle * (Math.PI / 180.0); ;
        }

        public static double Radian2Degree(double radian)
        {
            return radian * (180.0 / Math.PI); ;
        }

        public static double TAN(double val)
        {
            return Math.Tan(Degree2Radian(val));
        }

        public static double ATAN(double val)
        {
            return Radian2Degree(Math.Atan(val));
        }

        public static string ConverterDuodenary(int a, bool first = true) // Decimal을 12진수로 표현하자
        {
            string rv = string.Empty;
            int a0 = (int)(a / 12);
            int b0 = (int)(a % 12);
            if (a0 > 0) rv = ConverterDuodenary(a0, false);
            rv += string.Format("{0:X1}", b0);
            if (first) while (true) { if (rv.Length >= 4) break; else rv = "0" + rv; } //4 자리로 맞추자.
            return rv;
        }

    }
}
