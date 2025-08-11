/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.17 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Xml.Serialization;

namespace Sineva.VHL.Library
{
    [Serializable]
    [Editor(typeof(UIEditorXYPositionInput), typeof(UITypeEditor))]
    [TypeConverter(typeof(XyPositionConvert))]
    public class XyPosition : IComparable
    {
        private double m_X;
        private double m_Y;
        private short m_Order;

        private int m_BelowDecimalPoint = 0;

        #region Property
        public double X
        {
            get { return m_X; }
            set { m_X = value; }
        }
        public double Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }

        public short Order
        {
            get { return m_Order; }
            set { m_Order = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public XyPosition Clone
        {
            get
            {
                XyPosition clone = new XyPosition();
                clone.X = m_X;
                clone.Y = m_Y;
                clone.Order = m_Order;
                clone.m_BelowDecimalPoint = m_BelowDecimalPoint;
                return clone;
            }
        }
        #endregion

        #region Constructor
        public XyPosition()
        {
            m_BelowDecimalPoint = 0;
        }
        public XyPosition(int bdp)
        {
            if (bdp > 6) bdp = 6;
            else if (bdp < 0) bdp = 0;

            m_BelowDecimalPoint = bdp;
        }
        public XyPosition(double x, double y, int bdp = 0)
            : this(bdp)
        {
            m_X = x;
            m_Y = y;
        }
        #endregion

        #region Interface Member
        public int CompareTo(object obj)
        {
            if (obj == null || obj.GetType() != GetType()) return 1;

            XyPosition target = obj as XyPosition;
            if (target != null)
            {
                if (target.X == m_X && target.Y == m_Y) return 0;
                return -1;
            }
            else
                throw new ArgumentException("Object arguemnt is not a type of XyPosition");
        }
        #endregion

        #region Methods
        public static implicit operator XyPosition(string str)
        {
            try
            {
                string[] split = str.Split(new char[] { ',' });
                double x, y;
                bool valid = true;
                valid &= double.TryParse(split[0], out x);
                valid &= double.TryParse(split[1], out y);

                if (valid)
                {
                    return new XyPosition(x, y);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        public static bool TryParse(string text, out XyPosition val)
        {
            val = text;
            return val != null;
        }

        public static XyPosition operator +(XyPosition a, XyPosition b)
        {
            XyPosition c = new XyPosition();
            c.X = a.X + b.X;
            c.Y = a.Y + b.Y;

            return c;
        }

        public static XyPosition operator -(XyPosition a, XyPosition b)
        {
            XyPosition c = new XyPosition();
            c.X = a.X - b.X;
            c.Y = a.Y - b.Y;

            return c;
        }

        public static XyPosition operator *(XyPosition a, XyPosition b)
        {
            XyPosition c = new XyPosition();
            c.X = a.X * b.X;
            c.Y = a.Y * b.Y;

            return c;
        }

        public static XyPosition operator /(XyPosition a, XyPosition b)
        {
            XyPosition c = new XyPosition();
            c.X = a.X / b.X;
            c.Y = a.Y / b.Y;

            return c;
        }

        public void Reset()
        {
            m_X = 0.0f;
            m_Y = 0.0f;
        }
        public void Set(XyPosition a)
        {
            m_X = a.X;
            m_Y = a.Y;
        }
        public void Set(double a, double b)
        {
            m_X = a;
            m_Y = b;
        }

        public XyPosition Scale(double rt)
        {
            XyPosition result = new XyPosition();
            result.X = X * rt;
            result.Y = Y * rt;

            return result;
        }

        public static double Distance(XyPosition a, XyPosition b)
        {
            double dist;

            double X = (a - b).X;
            double Y = (a - b).Y;

            dist = Math.Sqrt(X * X + Y * Y);

            return dist;
        }

        public static double Area(XyPosition strartPoint, XyPosition endPoint)
        {
            double dRet;

            dRet = Math.Abs(strartPoint.X - endPoint.X) * Math.Abs(strartPoint.Y - endPoint.Y);

            return dRet;
        }
        #endregion

        public override string ToString()
        {
            string value = string.Format("{0}, {1}", X, Y);

            if (m_BelowDecimalPoint == 1) value = string.Format("{0:F1}, {1:F1}", X, Y);
            else if (m_BelowDecimalPoint == 2) value = string.Format("{0:F2}, {1:F2}", X, Y);
            else if (m_BelowDecimalPoint == 3) value = string.Format("{0:F3}, {1:F3}", X, Y);
            else if (m_BelowDecimalPoint == 4) value = string.Format("{0:F4}, {1:F4}", X, Y);
            else if (m_BelowDecimalPoint == 5) value = string.Format("{0:F5}, {1:F5}", X, Y);
            else if (m_BelowDecimalPoint == 6) value = string.Format("{0:F6}, {1:F6}", X, Y);

            return value;
        }
    }

    public class XyPositionConvert : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(XyPosition))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) && value is XyPosition)
            {
                XyPosition m = (XyPosition)value;
                if (m == null) this.ToString();
                else return m.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                try
                {
                    char[] banChars = new char[] { '(', ')', '_', ',', ' ', ':', '=' };
                    string[] splits = (value as string).Split(banChars);
                    List<string> values = new List<string>();
                    for (int i = 0; i < splits.Length; i++)
                    {
                        double temp_double = 0.0f;
                        if (double.TryParse(splits[i], out temp_double)) values.Add(splits[i]);
                    }

                    if (values.Count >= 2)
                    {
                        double x0, y0;
                        bool valid = true;
                        valid &= double.TryParse(values[0], out x0);
                        valid &= double.TryParse(values[1], out y0);
                        if (valid)
                        {
                            return new XyPosition(x0, y0);
                        }
                        else
                        {
                            return this;
                        }
                    }
                    else
                    {
                        return this;
                    }
                }
                catch
                {
                    return this;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
