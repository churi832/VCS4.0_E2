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
    [Editor(typeof(UIEditorXYZTPositionInput), typeof(UITypeEditor))]
    [TypeConverter(typeof(XyztPositionConvert))]
    public class XyztPosition : INotifyPropertyChanged
    {
        private double m_X;
        private double m_Y;
        private double m_Z;
        private double m_T;
        private short m_Order;

        private int m_BelowDecimalPoint = 0;

        #region Property
        public double X
        {
            get { return m_X; }
            set
            {
                m_X = value;
                NotifyPropertyChanged("X");
                OnPropertyValueChanged(this, EventArgs.Empty);
            }
        }
        public double Y
        {
            get { return m_Y; }
            set
            {
                m_Y = value;
                NotifyPropertyChanged("Y");
                OnPropertyValueChanged(this, EventArgs.Empty);
            }
        }
        public double Z
        {
            get { return m_Z; }
            set
            {
                m_Z = value;
                NotifyPropertyChanged("Z");
                OnPropertyValueChanged(this, EventArgs.Empty);
            }
        }
        public double T
        {
            get { return m_T; }
            set
            {
                m_T = value;
                NotifyPropertyChanged("T");
                OnPropertyValueChanged(this, EventArgs.Empty);
            }
        }
        public short Order
        {
            get { return m_Order; }
            set { m_Order = value; }
        }
        #endregion

        #region Constructor
        public XyztPosition()
        {
            m_BelowDecimalPoint = 0;
        }
        public XyztPosition(int bdp)
        {
            if (bdp > 6) bdp = 6;
            else if (bdp < 0) bdp = 0;

            m_BelowDecimalPoint = bdp;
        }
        public XyztPosition(double x, double y, double z, double t, int bdp = 0)
            : this(bdp)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
            m_T = t;
        }
        #endregion

        #region Methods
        public static implicit operator XyztPosition(string str)
        {
            try
            {
                string[] split = str.Split(new char[] { ',' });
                double x = 0.0f, y = 0.0f, z = 0.0f, t = 0.0f;
                bool valid = split.Length > 4;
                if (valid)
                {
                    valid &= double.TryParse(split[0], out x);
                    valid &= double.TryParse(split[1], out y);
                    valid &= double.TryParse(split[2], out z);
                    valid &= double.TryParse(split[3], out t);
                }

                if (valid)
                {
                    return new XyztPosition(x, y, z, t);
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

        public static bool TryParse(string text, out XyztPosition val)
        {
            val = text;
            return val != null;
        }

        public static XyztPosition operator +(XyztPosition a, XyztPosition b)
        {
            XyztPosition c = new XyztPosition();
            c.X = a.X + b.X;
            c.Y = a.Y + b.Y;
            c.Z = a.Z + b.Z;
            c.T = a.T + b.T;

            return c;
        }

        public static XyztPosition operator -(XyztPosition a, XyztPosition b)
        {
            XyztPosition c = new XyztPosition();
            c.X = a.X - b.X;
            c.Y = a.Y - b.Y;
            c.Z = a.Z - b.Z;
            c.T = a.T - b.T;

            return c;
        }

        public static XyztPosition operator *(XyztPosition a, XyztPosition b)
        {
            XyztPosition c = new XyztPosition();
            c.X = a.X * b.X;
            c.Y = a.Y * b.Y;
            c.Z = a.Z * b.Z;
            c.T = a.T * b.T;

            return c;
        }

        public static XyztPosition operator /(XyztPosition a, XyztPosition b)
        {
            XyztPosition c = new XyztPosition();
            c.X = a.X / b.X;
            c.Y = a.Y / b.Y;
            c.Z = a.Z / b.Z;
            c.T = a.T / b.T;

            return c;
        }

        public void Reset()
        {
            m_X = 0.0f;
            m_Y = 0.0f;
            m_Z = 0.0f;
            m_T = 0.0f;
        }
        public void Set(XyztPosition a)
        {
            m_X = a.X;
            m_Y = a.Y;
            m_Z = a.Z;
            m_T = a.T;
        }
        public void Set(double a, double b, double c, double d)
        {
            m_X = a;
            m_Y = b;
            m_Z = c;
            m_T = d;
        }

        public XyztPosition GetRatioApplied(double rt)
        {
            XyztPosition result = new XyztPosition();
            result.X = X * rt;
            result.Y = Y * rt;
            result.Z = Z * rt;
            result.T = T * rt;

            return result;
        }
        #endregion
        public override string ToString()
        {
            string value = string.Format("{0}, {1}, {2}, {3}", X, Y, Z, T);

            if (m_BelowDecimalPoint == 1) value = string.Format("{0:F1}, {1:F1}, , {2:F1}", X, Y, T);
            else if (m_BelowDecimalPoint == 2) value = string.Format("{0:F2}, {1:F2}, {2:F2}", X, Y, T);
            else if (m_BelowDecimalPoint == 3) value = string.Format("{0:F3}, {1:F3}, {2:F3}", X, Y, T);
            else if (m_BelowDecimalPoint == 4) value = string.Format("{0:F4}, {1:F4}, {2:F4}", X, Y, T);
            else if (m_BelowDecimalPoint == 5) value = string.Format("{0:F5}, {1:F5}, {2:F5}", X, Y, T);
            else if (m_BelowDecimalPoint == 6) value = string.Format("{0:F6}, {1:F6}, {2:F6}", X, Y, T);

            return value;
        }

        #region INotifyPropertyChanged 멤버

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        public event EventHandler PropertyValueChanged;
        private void OnPropertyValueChanged(object sender, EventArgs e)
        {
            EventHandler eh = PropertyValueChanged;
            if (eh != null)
                eh(sender, e);
        }
    }

    public class XyztPositionConvert : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(XyztPosition))
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
            if (destinationType == typeof(System.String) && value is XyztPosition)
            {
                XyztPosition m = (XyztPosition)value;
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

                    if (values.Count >= 4)
                    {
                        double x0, y0, z0, t0;
                        bool valid = true;
                        valid &= double.TryParse(values[0], out x0);
                        valid &= double.TryParse(values[1], out y0);
                        valid &= double.TryParse(values[2], out z0);
                        valid &= double.TryParse(values[3], out t0);
                        if (valid)
                        {
                            return new XyztPosition(x0, y0, z0, t0);
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
