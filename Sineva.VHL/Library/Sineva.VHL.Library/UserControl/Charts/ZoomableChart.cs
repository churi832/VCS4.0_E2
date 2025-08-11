using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;

namespace Sineva.VHL.Library
{
	public partial class ZoomableChart : UserControl
	{
        public delegate void ZoomChartMouseClick(double x, double y);
        public delegate void ZoomChartCursorSelect();
        public delegate void ZoomChartZoomRefresh();

		#region Fields
		private string m_XLabelFormat = "F3";
		private string m_YLabelFormat = "F3";
		private bool m_MouseDown = false;
		private bool m_AddCursor = false;
        private int m_CursorCount = 0;
		private ChartPosition m_1stCursorPos = new ChartPosition();
		private ChartPosition m_2ndCursorPos = new ChartPosition();
		private bool m_InitChartDone = false;
		private bool m_ShowAxisLabel = true;
		private ToolTip m_ToolTip = new ToolTip();
		private Point m_prevPosition;
        private bool m_ShowStatus = true;
        private bool m_MouseMoving = false;
        private bool m_AxisLabelVisible = true;

        private Color m_DefaultChartBackColor = Color.FloralWhite;
		#endregion

		#region Propreties
        [Category("!UI Setting")]
        public string XLabelFormat
		{
			get { return m_XLabelFormat; }
			set 
			{ 
				m_XLabelFormat = value;
				this.chart1.ChartAreas[0].AxisX.LabelStyle.Format = m_XLabelFormat;
			}
		}

        [Category("!UI Setting")]
        public string YLabelFormat
		{
			get { return m_YLabelFormat; }
			set 
			{ 
				m_YLabelFormat = value;
				this.chart1.ChartAreas[0].AxisY.LabelStyle.Format = m_YLabelFormat;
			}
		}
        [Category("!UI Setting")]
        public bool ShowAxisLabel
		{
			get { return m_ShowAxisLabel; }
			set 
			{ 
				m_ShowAxisLabel = value;
				if (value == false)
				{
					this.chart1.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
					this.chart1.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
                    this.chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    this.chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                    this.chart1.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;
                    this.chart1.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;
					this.chart1.ChartAreas[0].AxisX.MinorTickMark.Enabled = false;
					this.chart1.ChartAreas[0].AxisY.MinorTickMark.Enabled = false;
				}
			}
		}

        public SeriesCollection Series
        {
            get
            {
                try
                {
                    return this.chart1.Series;
                }
                catch(Exception ex)
                {
                    //ExceptionLog.WriteLog(ex.Message);
                    return null;
                }
            }
        }

		public ChartAreaCollection Areas
		{
			get { return this.chart1.ChartAreas; }
		}
        public TitleCollection Titles
        {
            get { return this.chart1.Titles; }
        }
        [Browsable(false)]
		public event ZoomChartMouseClick ZoomChartClickedPos = null;
		[Browsable(false)]
		public event ZoomChartCursorSelect SelectedCursors = null;
        [Browsable(false)]
        public event ZoomChartZoomRefresh RefreshZoom = null;
        [Browsable(false), ReadOnly(true)]
		public ChartPosition M_CursorPos1
        {
            get { return m_1stCursorPos; }
            set { m_1stCursorPos = value; }
        }
		[Browsable(false), ReadOnly(true)]
		public ChartPosition M_CursorPos2
        {
            get { return m_2ndCursorPos; }
            set { m_2ndCursorPos = value; }
        }

        public int CursorCount
        {
            get { return m_CursorCount; }
            set { m_CursorCount = value; }
        }
        public bool ShowStatus
        {
            get { return m_ShowStatus; }
            set { m_ShowStatus = value; }
        }
        public Color DefaultChartBackColor
        {
            get { return m_DefaultChartBackColor; }
            set { m_DefaultChartBackColor = value; }
        }

        [Category("!UI Setting : Chart Area"), DisplayName("Chart BackGradientStyle")]
        public GradientStyle ChartAreaBackGradientStyle
        {
            get { return this.chart1.ChartAreas[0].BackGradientStyle; }
            set { this.chart1.ChartAreas[0].BackGradientStyle = value; }
        }
        [Category("!UI Setting : Chart Area"), DisplayName("Chart BackColor")]
        public Color ChartAreaBackgroundColor
        {
            get { return this.chart1.ChartAreas[0].BackColor; }
            set { this.chart1.ChartAreas[0].BackColor = value; }
        }
        [Category("!UI Setting : Chart Area"), DisplayName("Chart ShadowColor")]
        public Color ChartAreaShadowColor
        {
            get { return this.chart1.ChartAreas[0].ShadowColor; }
            set { this.chart1.ChartAreas[0].ShadowColor = value; }
        }
        #endregion

		#region Constructor
		public ZoomableChart()
		{
			InitializeComponent();

            if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                this.DoubleBuffered = true;

                InitChart();

                this.toolStripMenuItemAddCursor.Enabled = true;
                this.toolStripMenuItemClearCursor.Enabled = false;
                this.lblCursor.Text = "";

                if(!m_ShowStatus)
                {
                    tableLayoutPanel1.RowCount = 1;
                }
            }
		}
		#endregion

		#region Methods
		private void InitChart()
		{
			if (m_InitChartDone) return;
			foreach (System.Reflection.PropertyInfo prop in typeof(Color).GetProperties())
			{
				if (prop.PropertyType.FullName == "System.Drawing.Color")
					this.cbColorPicker.Items.Add(prop.Name);
			}

			#region Chart setting
            this.chart1.ChartAreas[0].BackColor = m_DefaultChartBackColor;

            this.chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
			this.chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
			this.chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
			this.chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;

			this.chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
			this.chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;

			this.chart1.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;

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

			this.chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
			this.chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

			this.chart1.ChartAreas[0].AxisX.MajorTickMark.TickMarkStyle = TickMarkStyle.AcrossAxis;
			this.chart1.ChartAreas[0].AxisY.MajorTickMark.TickMarkStyle = TickMarkStyle.AcrossAxis;
			this.chart1.ChartAreas[0].AxisX.MinorTickMark.Enabled = true;
			this.chart1.ChartAreas[0].AxisY.MinorTickMark.Enabled = true;

			this.chart1.ChartAreas[0].AxisX.LabelStyle.Format = m_XLabelFormat;
			this.chart1.ChartAreas[0].AxisY.LabelStyle.Format = m_YLabelFormat;
			#endregion

			m_InitChartDone = true;
		}

		public void ClearPlot()
		{
			this.SuspendLayout();
			this.chart1.Series.Clear();
			this.ResumeLayout();
		}

		public void ResetAutoValues()
		{
			this.chart1.ResetAutoValues();
		}
		double selX, selY;
		double dx, dy;
		double newX, newY;
		private void chart1_MouseMove(object sender, MouseEventArgs e)
		{
			//Point mousePoint = new Point(e.X, e.Y);
			//// Cursors display
			//{
			//    this.chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(mousePoint, true);
			//    this.chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(mousePoint, true);
			//}

			//this.toolStripStatusLabelCursorPos.Text = string.Format("{0:F3},{1:F3}", chart1.ChartAreas[0].CursorX.Position, chart1.ChartAreas[0].CursorY.Position);
			//if (e.Location.X > 100 || e.Location.Y > 100) return;
            if (m_MouseMoving) return;
            m_MouseMoving = true;

			var pos = e.Location;

			// Pan Control
			Chart ptrChart = sender as Chart;
			selX = selY = 0;
			try
			{
                if (pos.X < 100 && pos.Y < 100)
                {
                    selX = ptrChart.ChartAreas[0].AxisX.PixelPositionToValue(pos.X);
                    selY = ptrChart.ChartAreas[0].AxisY.PixelPositionToValue(pos.Y);
                }
			}
			catch (Exception err) 
			{ 
				/*ToDo: Set coordinate to 0,0 */
                //Trace.WriteLine("ZoomableChart Exception ========>" + pos.ToString());
                m_MouseMoving = false;
                //ExceptionLog.WriteLog(err.ToString() + pos.ToString());
				return; 
			} //Handle exception when scrolled out of range.

            try
            {
                if (m_MouseDown)
                {
                    //Pan Move - Valid only if view is zoomed
                    if (ptrChart.ChartAreas[0].AxisX.ScaleView.IsZoomed ||
                        ptrChart.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                    {
					    if (ptrChart.ChartAreas[0].CursorX.SelectionStart == double.NaN)
						    dx = -selX + ptrChart.ChartAreas[0].CursorX.Position;
					    else
						    dx = -selX + ptrChart.ChartAreas[0].CursorX.SelectionStart;

					    if (ptrChart.ChartAreas[0].CursorY.SelectionStart == double.NaN)
						    dy = -selY + ptrChart.ChartAreas[0].CursorY.Position;
					    else
						    dy = -selY + ptrChart.ChartAreas[0].CursorY.SelectionStart;

                        newX = ptrChart.ChartAreas[0].AxisX.ScaleView.Position + dx;
                        newY = ptrChart.ChartAreas[0].AxisY.ScaleView.Position + dy;
                        //double newY2 = ptrChart.ChartAreas[0].AxisY2.ScaleView.Position + dy;

                        ptrChart.ChartAreas[0].AxisX.ScaleView.Scroll(newX);
                        ptrChart.ChartAreas[0].AxisY.ScaleView.Scroll(newY);
                        //ptrChart.ChartAreas[0].AxisY2.ScaleView.Scroll(newY2);
                        //System.Diagnostics.Trace.WriteLine(string.Format("{0}, {1}",dx, dy));
                    }
                }
            }
            catch(Exception ex)
            {
                m_MouseMoving = false;
                //ExceptionLog.WriteLog(ex.ToString());
                return;
            }

			/// Series point value  tooltip display
			/// 
            try
            {
                if (m_prevPosition != pos)
                {
                    m_ToolTip.RemoveAll();
                    m_prevPosition = pos;
                    var results = chart1.HitTest(pos.X, pos.Y, false, ChartElementType.DataPoint);
                    foreach (var result in results)
                    {
                        if (result.ChartElementType == ChartElementType.DataPoint)
                        {
                            var prop = result.Object as DataPoint;
                            if (prop != null)
                            {
                                var pointXPixel = result.ChartArea.AxisX.ValueToPixelPosition(prop.XValue);
                                var pointYPixel = result.ChartArea.AxisY.ValueToPixelPosition(prop.YValues[0]);
                                if (Math.Abs(pos.X - pointXPixel) < 2 && Math.Abs(pos.Y - pointYPixel) < 2)
                                {
                                    m_ToolTip.Show("X=" + prop.XValue + ", Y=" + prop.YValues[0], this.chart1, pos.X, pos.Y - 15);
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                m_MouseMoving = false;
                //ExceptionLog.WriteLog(ex.ToString());
                return;
            }

            m_MouseMoving = false;
		}

		private void chart1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != System.Windows.Forms.MouseButtons.Middle) return;
            if (m_MouseMoving) return;

			m_MouseDown = true;

			Chart ptrChart = (Chart)sender;
			ChartArea ptrChartArea = ptrChart.ChartAreas[0];

			//if (e.Location.X > 100 || e.Location.Y > 100) return;

            try
            {
                ptrChartArea.CursorX.SelectionStart = ptrChartArea.AxisX.PixelPositionToValue(e.Location.X);
                ptrChartArea.CursorY.SelectionStart = ptrChartArea.AxisY.PixelPositionToValue(e.Location.Y);
                ptrChartArea.CursorX.SelectionEnd = ptrChartArea.CursorX.SelectionStart;
                ptrChartArea.CursorY.SelectionEnd = ptrChartArea.CursorY.SelectionStart;

                Point mousePoint = new Point(e.X, e.Y);
            }
            catch(Exception ex)
            {
                //ExceptionLog.WriteLog(ex.ToString());
                m_MouseDown = false;
            }
		}

		private void chart1_MouseUp(object sender, MouseEventArgs e)
		{
            Chart ptrChart = (Chart)sender;
            if (ptrChart.ChartAreas[0].AxisX.ScaleView.IsZoomed || ptrChart.ChartAreas[0].AxisY.ScaleView.IsZoomed) 
            {
                if (RefreshZoom != null) RefreshZoom();
            }
            if (e.Button != System.Windows.Forms.MouseButtons.Middle) return;
			m_MouseDown = false;
		}

		private void chart1_MouseClick(object sender, MouseEventArgs e)
		{
            if (m_MouseMoving) return;

			if (ZoomChartClickedPos != null)
			{
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    ZoomChartClickedPos(chart1.ChartAreas[0].CursorX.Position, chart1.ChartAreas[0].CursorY.Position);
            }

			this.lblXyPos.Text = string.Format("({0:F4}, {1:F4})", chart1.ChartAreas[0].CursorX.Position, chart1.ChartAreas[0].CursorY.Position);
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				this.contextMenuStrip1.Show((Chart)sender, new Point(e.X, e.Y));
				if (this.chart1.ChartAreas[0].AxisX.ScaleView.IsZoomed || this.chart1.ChartAreas[0].AxisY.ScaleView.IsZoomed)
				{
					this.toolStripMenuItemUnZoom.Enabled = true;
					this.toolStripMenuItemUnZoomAll.Enabled = true;
				}
				else
				{
					this.toolStripMenuItemUnZoom.Enabled = false;
					this.toolStripMenuItemUnZoomAll.Enabled = false;
				}
			}
			else if (m_AddCursor && m_CursorCount < 2)
			{
				m_AddCursor = false;
				HorizontalLineAnnotation h = new HorizontalLineAnnotation();
				h.AxisX = this.chart1.ChartAreas[0].AxisX;
				h.AxisY = this.chart1.ChartAreas[0].AxisY;
				h.IsSizeAlwaysRelative = false;
				h.AnchorY = chart1.ChartAreas[0].CursorY.Position;
				h.IsInfinitive = true;
				h.ClipToChartArea = this.chart1.ChartAreas[0].Name;
				h.LineColor = Color.Lime;
				h.LineDashStyle = ChartDashStyle.Solid;
				h.LineWidth = 1;
				this.chart1.Annotations.Add(h);

				VerticalLineAnnotation v = new VerticalLineAnnotation();
				v.AxisX = this.chart1.ChartAreas[0].AxisX;
				v.AxisY = this.chart1.ChartAreas[0].AxisY;
				v.IsSizeAlwaysRelative = false;
				v.AnchorX = chart1.ChartAreas[0].CursorX.Position;
				v.IsInfinitive = true;
				v.ClipToChartArea = this.chart1.ChartAreas[0].Name;
				v.LineColor = Color.Lime;
				v.LineDashStyle = ChartDashStyle.Solid;
				v.LineWidth = 1;
				this.chart1.Annotations.Add(v);
				m_CursorCount++;
				if (m_CursorCount == 1)
				{
					m_1stCursorPos.X = chart1.ChartAreas[0].CursorX.Position;
					m_1stCursorPos.Y = chart1.ChartAreas[0].CursorY.Position;
					this.toolStripMenuItemClearCursor.Enabled = true;
				}
				if (m_CursorCount == 2)
				{
					m_2ndCursorPos.X = chart1.ChartAreas[0].CursorX.Position;
					m_2ndCursorPos.Y = chart1.ChartAreas[0].CursorY.Position;

					double distanceX = Math.Abs(m_1stCursorPos.X - m_2ndCursorPos.X);
					double distanceY = Math.Abs(m_1stCursorPos.Y - m_2ndCursorPos.Y);

					this.lblCursor.Text = string.Format("Distance({0:F4},{1:F4})", distanceX, distanceY);

					this.toolStripMenuItemAddCursor.Enabled = false;
                    if (SelectedCursors != null)
                        SelectedCursors();
				}
			}
		}

		#region Methods / ToolStripMenu
		private void toolStripMenuItemUnZoom_Click(object sender, EventArgs e)
		{
			this.chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
			this.chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            if (RefreshZoom != null) RefreshZoom();
        }

        private void toolStripMenuItemUnZoomAll_Click(object sender, EventArgs e)
		{
			this.SuspendLayout();
			while (this.chart1.ChartAreas[0].AxisX.ScaleView.IsZoomed || this.chart1.ChartAreas[0].AxisY.ScaleView.IsZoomed)
			{
				this.chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
				this.chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
			}
            if (RefreshZoom != null) RefreshZoom();
            this.ResumeLayout();
		}

		private void toolStripMenuItemAddCursor_Click(object sender, EventArgs e)
		{
			m_AddCursor = true;
		}

		private void toolStripMenuItemClearCursor_Click(object sender, EventArgs e)
		{
			this.chart1.Annotations.Clear();
			this.chart1.UpdateAnnotations();
			this.chart1.Update();
			m_CursorCount = 0;
			this.lblCursor.Text = "";
			this.toolStripMenuItemAddCursor.Enabled = true;
			this.toolStripMenuItemClearCursor.Enabled = false;

            if (SelectedCursors != null)
                SelectedCursors();
		}
		#endregion

		private void cbColorPicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.chart1.ChartAreas[0].BackColor = Color.FromName(this.cbColorPicker.SelectedItem as string);
			}
			catch (Exception err)
			{
                MessageBox.Show(err.ToString() + this.ToString());
				//ExceptionLog.WriteLog(err.ToString());
			}
		}
		#endregion
	}

	public class ChartPosition
	{
		private double m_X;
		private double m_Y;

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

		public ChartPosition()
		{

		}
	}
}
