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
	public partial class MovingFlag : Label
	{
		public MovingFlag()
		{
			InitializeComponent();
			this.AutoSize = false;
			this.Visible = false;
		}

		public override string ToString()
		{
			return "*";
		}
	}
}
