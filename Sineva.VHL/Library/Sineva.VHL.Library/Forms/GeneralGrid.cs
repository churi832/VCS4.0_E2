/****************************************************************
 * Copyright	: www.kpscorp.co.kr
 * Version		: V1.0
 * Programmer	: KPS-개발4팀
 * Issue Date	: 14.01.21
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
	public partial class GeneralGrid : UserControl
	{
		public string Title
		{
			get { return lblTitle.Text; }
			set { lblTitle.Text = value; }
		}

		public object DataSource
		{
			get { return doubleBufferedGridView1.DataSource; }
			set { doubleBufferedGridView1.DataSource = value; }
		}

		public GeneralGrid()
		{
			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.CacheText, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}
	}
}
