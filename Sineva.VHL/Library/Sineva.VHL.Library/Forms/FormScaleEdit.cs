/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V1.0
 * Programmer	: Software Group
 * Issue Date	: 23.02.20
 * Description	: 
 * 
 ****************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
	public partial class FormScaleEdit : Form
	{
		public Scale m_Scale = null;
		private XyPosition m_SelectedPos = null;

		public FormScaleEdit()
		{
			InitializeComponent();
			this.cbScaleUsage.DataSource = Enum.GetValues(typeof(Use));
			this.cbScaleUsage.SelectedIndex = 0;
		}

		public FormScaleEdit(Scale scale)
		{
			m_Scale = scale;
			InitializeComponent();

			Use useTemp = m_Scale.Usage;
			this.cbScaleUsage.DataSource = Enum.GetValues(typeof(Use));
			this.cbScaleUsage.SelectedItem = useTemp;

			this.lbScale.DataSource = m_Scale.Samples;
			this.textBox1.Text = m_Scale.Name;

			this.chart1.ChartAreas[0].AxisX.MajorTickMark.TickMarkStyle = System.Windows.Forms.DataVisualization.Charting.TickMarkStyle.AcrossAxis;
			this.chart1.ChartAreas[0].AxisY.MajorTickMark.TickMarkStyle = System.Windows.Forms.DataVisualization.Charting.TickMarkStyle.AcrossAxis;
			this.chart1.ChartAreas[0].AxisX.MinorTickMark.Enabled = true;
			this.chart1.ChartAreas[0].AxisY.MinorTickMark.Enabled = true;

			this.chart1.ChartAreas[0].AxisX.ScaleView.SizeType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;

			this.chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = false;
			this.chart1.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = false;

			this.chart1.ChartAreas[0].CursorX.LineColor = Color.Red;
			this.chart1.ChartAreas[0].CursorX.LineWidth = 1;
			this.chart1.ChartAreas[0].CursorX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
			this.chart1.ChartAreas[0].CursorX.Interval = 0;
			this.chart1.ChartAreas[0].CursorY.LineColor = Color.Red;
			this.chart1.ChartAreas[0].CursorY.LineWidth = 1;
			this.chart1.ChartAreas[0].CursorY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
			this.chart1.ChartAreas[0].CursorY.Interval = 0;


			this.chart1.ChartAreas[0].AxisX.LabelStyle.Format = "F3";
			this.chart1.ChartAreas[0].AxisY.LabelStyle.Format = "F3";

			this.chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
			this.chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

			PlotChart();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			using (FormXyPositionInput form = new FormXyPositionInput())
			{
				if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					m_Scale.Samples.Add(new XyPosition(form.m_Result.X, form.m_Result.Y));
					ReBindingListBox();
					PlotChart();
                    //ButtonLog.WriteLog(this.Name.ToString(), string.Format("Add Click : [X = {0}][Y = {1}]", form.m_Result.X, form.m_Result.Y));
                }
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (m_SelectedPos != null)
			{
                //ButtonLog.WriteLog(this.Name.ToString(), string.Format("Remove Click : [X = {0}][Y = {1}]", m_SelectedPos.X, m_SelectedPos.Y));
                m_Scale.Samples.Remove(m_SelectedPos);
			}
			ReBindingListBox();
			PlotChart();
		}

		private void lbScale_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.lbScale.SelectedItem == null) return;
			m_SelectedPos = this.lbScale.SelectedItem as XyPosition;
		}

		private void PlotChart()
		{
			ClearPlot();
			string name = "scale";
			this.chart1.Series.Add(name);
			chart1.Series[name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			chart1.Series[name].Color = Color.Black;
			foreach (XyPosition pos in m_Scale.Samples)
			{
				this.chart1.Series[name].Points.AddXY(pos.X, pos.Y);
			}
		}

		private void ReBindingListBox()
		{
			this.lbScale.DataSource = null;
			this.lbScale.Refresh();
			this.lbScale.DataSource = m_Scale.Samples;
		}

		private void ClearPlot()
		{
			this.SuspendLayout();
			this.chart1.Series.Clear();
			this.ResumeLayout();
		}

		private void lbScale_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (this.lbScale.SelectedItem == null) return;

			ListBox lb = sender as ListBox;
			using (FormXyPositionInput form = new FormXyPositionInput(lb.SelectedItem as XyPosition))
			{
				if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					ReBindingListBox();
					PlotChart();
				}
			}
		}

		private void btnNameApply_Click(object sender, EventArgs e)
		{
			m_Scale.Name = this.textBox1.Text;
            //ButtonLog.WriteLog(this.Name.ToString(), string.Format("Name Apply : {0}", m_Scale.Name));
        }

		private void btnClose_Click(object sender, EventArgs e)
		{
            //ButtonLog.WriteLog(this.Name.ToString(), string.Format("Close Click"));
            this.Dispose();
		}

		private void cbScaleUsage_SelectedIndexChanged(object sender, EventArgs e)
		{
			Use usage = m_Scale.Usage;
			m_Scale.Usage = Enum.TryParse(cbScaleUsage.SelectedValue.ToString(), out usage) ? usage : Use.Use;
		}

        private void lbScale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                try
                {
                    ListBox view = sender as ListBox;

                    string s = Clipboard.GetText();
                    char[] splitter = { '\r', '\n' };
                    string[] lines = s.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    string[] line0 = lines[0].Split('\t');

                    int ClipboardRowCount = lines.Length;
                    int ClipboardColCount = line0.Length;

                    if (ClipboardColCount > 2) // Ref 위치만....
                    {
                        MessageBox.Show("cann't paste, Selected-Columms of Copy. need 2ea Cols");
                        return;
                    }


                    m_Scale.Samples.Clear();

                    List<XyPosition> pTempLists = new List<XyPosition>();

                    bool check = true;
                    string line;
                    for (int i = 0; i < ClipboardRowCount; i++)
                    {
                        line = lines[i];
                        string[] cells = line.Split('\t');

                        XyPosition pTemp = new XyPosition(0.0, 0.0);

                        double x0 = 0.0;
                        double y0 = 0.0;
                        short order = 0;

                        if (Double.TryParse(cells[0], out x0)) pTemp.X = x0;
                        else check = false;
                        if (Double.TryParse(cells[1], out y0)) pTemp.Y = y0;
                        else check = false;

                        if (check) pTempLists.Add(pTemp);
                        else break;
                    }
                    if (!check)
                    {
                        return;
                    }
                    m_Scale.Samples = ObjectCopier.Clone(pTempLists);
                    view.DataSource = m_Scale.Samples;

                    //ButtonLog.WriteLog(this.Name.ToString(), string.Format("Paste (Keys.V)\r\n{0}", s));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Clip Board Copy Exception {0}", ex.ToString()));
                }
            }
        }
	}
}
