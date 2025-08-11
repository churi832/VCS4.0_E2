using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Sineva.VHL.Library
{
	public partial class DoubleBufferedGridView : System.Windows.Forms.DataGridView
	{
		public DoubleBufferedGridView()
			: base()
		{
			InitializeComponent();
			this.DoubleBuffered = true;
		}

		// 화면 깜빡임 최소화
		// .net 과 Nvidia graphic 호환성 이상 : GridView 사용시 점유율 증가 및 화면 painting 속도 저하됨
		// DoubleBuffer 설정으로 최소화
		//protected void SetDoubleBuffer()
		//{
		//    //this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		//    //this.SetStyle(ControlStyles.UserPaint, true);
		//    //this.SetStyle(ControlStyles.CacheText, true);
		//    //this.SetStyle(ControlStyles.DoubleBuffer, true);
		//    //this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

		//    this.DoubleBuffered = true;
		//}
	}
}
