using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Sineva.VHL.Library
{
	public partial class PointLineChart : UserControl
	{
		#region Fields
		private string m_Title;
		#endregion

		#region Properties
		public string Title
		{
			get { return m_Title; }
			set 
			{ 
				m_Title = value;
				this.chart1.Titles[0].Text = value;
			}
		}
		#endregion

		#region Constructor
        public PointLineChart()
		{
			InitializeComponent();
			chart1.Titles[0].Text = Title;
			chart1.Invalidate();
		}
		#endregion

		#region Method
		public void AddPoints(int SeriesNo, PointF p)
		{
			if (SeriesNo > 2) return;
			string series = string.Format("Series{0}", SeriesNo + 1);
			chart1.Series[series].Points.AddXY(p.X, p.Y);
		}

		public void RemovePoints(int SeriesNo)
		{
			if (SeriesNo > 2) return;
			string series = string.Format("Series{0}", SeriesNo + 1);
			chart1.Series[series].Points.Clear();
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(Name))
				return base.ToString();
			else
				return this.m_Title;
		}
		#endregion
	}
}
