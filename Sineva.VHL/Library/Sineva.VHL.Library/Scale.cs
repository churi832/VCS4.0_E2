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

namespace Sineva.VHL.Library
{
    public enum enScaleType
    {
        SECTION,
        EQUATION
    }
    [Serializable]
    [Editor(typeof(UIEditorScale), typeof(UITypeEditor))]
    public class Scale
    {
        #region Fields
        protected Use m_Usage;
        private List<XyPosition> m_Samples = new List<XyPosition>();
        private enScaleType m_Type = enScaleType.SECTION;
        private double m_A1 = 0.0;
        private double m_A2 = 0.0;
        private double m_A3 = 0.0;
        private double m_A4 = 0.0;
        private double m_A5 = 0.0;
        private double m_B = 0.0;
        #endregion

        #region Properties

        public enScaleType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        public double A1
        {
            get { return m_A1; }
            set { m_A1 = value; }
        }

        public double A2
        {
            get { return m_A2; }
            set { m_A2 = value; }
        }

        public double A3
        {
            get { return m_A3; }
            set { m_A3 = value; }
        }

        public double A4
        {
            get { return m_A4; }
            set { m_A4 = value; }
        }

        public double A5
        {
            get { return m_A5; }
            set { m_A5 = value; }
        }

        public double B
        {
            get { return m_B; }
            set { m_B = value; }
        }

        public string Name { get; set; }
        public List<XyPosition> Samples
        {
            get { return m_Samples; }
            set { m_Samples = value; }
        }
        public Use Usage
        {
            get { return m_Usage; }
            set { m_Usage = value; }
        }
        #endregion

        #region Constructor
        public Scale()
        {
        }
        #endregion

        #region Methods
        public double GetX(double y)
        {
            if (m_Type == enScaleType.SECTION)
            {
                for (int i = 0; i < m_Samples.Count - 1; i++)
                {
                    if (m_Samples[i].Y < y && y <= m_Samples[i + 1].Y)
                    {
                        double diffX = (m_Samples[i + 1].X - m_Samples[i].X);
                        double diffY = (m_Samples[i + 1].Y - m_Samples[i].Y);

                        if (diffX != 0.0 && diffY != 0)
                        {
                            double a = (diffX / diffY);
                            double b = m_Samples[i].Y;
                            double x0 = m_Samples[i].X;


                            double x = Math.Round((a * (y - b) + x0), 5);
                            return x;
                            //return (diffX / diffY) * (y - m_Samples[i].Y) + m_Samples[i].X;
                        }
                        else
                            return y;
                    }
                }
            }
            else if (m_Type == enScaleType.EQUATION)
            {
                return y;
            }
            return y;
        }

        public double GetY(double x)
        {
            if (m_Type == enScaleType.SECTION)
            {
                for (int i = 0; i < m_Samples.Count - 1; i++)
                {
                    if (m_Samples[i].X < x && x <= m_Samples[i + 1].X)
                    {
                        double diffX = (m_Samples[i + 1].X - m_Samples[i].X);
                        double diffY = (m_Samples[i + 1].Y - m_Samples[i].Y);

                        if (diffX != 0.0 && diffY != 0)
                        {
                            double a = (diffY / diffX);
                            double b = m_Samples[i].Y;
                            double x0 = m_Samples[i].X;

                            double y = a * (x - x0) + b;
                            return (y + 0.00005f);
                            //return (diffY / diffX) * (x - m_Samples[i].X) + m_Samples[i].Y;
                        }
                        else
                            return x;
                    }
                }
            }
            else if (m_Type == enScaleType.EQUATION)
            {
                double f = (double)x;
                var sol = A5 * Math.Pow(f, 5) +
                    A4 * Math.Pow(f, 4) +
                    A3 * Math.Pow(f, 3) +
                    A2 * Math.Pow(f, 2) +
                    A1 * f + B;
                return sol;
            }
            return x;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
                return Samples.Count.ToString();
            else
                return string.Format("{0}-{1}-{2}", Name, Samples.Count, Usage);
        }
        #endregion
    }
}
