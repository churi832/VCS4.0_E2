using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Sineva.VHL.Library.IO
{
    public partial class TrendGraph : UserControl
    {
        public enum LEGEND_DOCK_STYLE
        {
            NotSet,
            ChartArea1
        }

        #region Fields
        private SeriesChartType m_ChartType = SeriesChartType.Line;
        private List<IoChannel> m_Devices = new List<IoChannel>();
        private DateTime m_InitTime;
        private bool m_UpdateDuringVisible = false;
        private Random m_Random = new Random();
        private bool m_TestGraph = false;
        private LEGEND_DOCK_STYLE m_LegendDockStyle = LEGEND_DOCK_STYLE.ChartArea1;

        #endregion

        #region Properties
        public bool UpdateDuringVisible
        {
            get { return m_UpdateDuringVisible; }
            set { m_UpdateDuringVisible = value; }
        }

        public SeriesChartType ChartType
        {
            get { return m_ChartType; }
            set
            {
                m_ChartType = value;

                foreach(Series series in this.chart1.Series)
                {
                    series.ChartType = value;
                }
            }
        }

        public bool UseSamplingPeriodSetting
        {
            get { return dateTimePicker1.Visible; }
            set
            {
                dateTimePicker1.Visible = value;
                this.label4.Visible = value;
            }
        }


        public LEGEND_DOCK_STYLE LegendDockStyle
        {
            get
            {
                return (LEGEND_DOCK_STYLE)Enum.Parse(typeof(LEGEND_DOCK_STYLE), this.chart1.Legends[0].DockedToChartArea);
            }
            set
            {
                m_LegendDockStyle = value;
                this.chart1.Legends[0].DockedToChartArea = m_LegendDockStyle.ToString();
            }
        }



        public bool TestGraph
        {
            get { return m_TestGraph; }
            set { m_TestGraph = value; }
        }

        public int SamplingInterval
        {
            get { return this.timerUpdate.Interval; }
            set { this.timerUpdate.Interval = value; }
        }

        public decimal Capacity
        {
            get
            {
                return this.numericUpDownCapa.Value;
            }
            set
            {
                this.numericUpDownCapa.Value = value;
            }
        }

        public string YAxisTitle
        {
            get { return this.chart1.ChartAreas[0].AxisY.Title; }
            set { this.chart1.ChartAreas[0].AxisY.Title = value; }
        }

        public bool SamplingEnable
        {
            get { return this.timerUpdate.Enabled; }
            set { this.timerUpdate.Enabled = value; }
        }

        public string XAxisTitle
        {
            get { return this.chart1.ChartAreas[0].AxisX.Title; }
            set { this.chart1.ChartAreas[0].AxisX.Title = value; }
        }

        #endregion

        #region Constructor
        public TrendGraph()
        {
            InitializeComponent();
            this.chart1.ChartAreas[0].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Seconds;
            this.chart1.ChartAreas[0].AxisX.LabelStyle.Format = "mm:ss";
            this.chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            this.chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gray;
            this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gray;

            this.chart1.ChartAreas[0].AxisY2.MajorGrid.LineDashStyle = ChartDashStyle.NotSet;

            //this.chart1.ChartAreas[0].AxisX.IsMarginVisible = false;
            //this.chart1.ChartAreas[0].AxisY.IsMarginVisible = false;
            //this.chart1.ChartAreas[0].AxisY2.IsMarginVisible = false;



            m_InitTime = Convert.ToDateTime("2000/01/01");
            TimeSpan tSpan = new TimeSpan(0, 0, 1);
            this.dateTimePicker1.Value = m_InitTime + tSpan;
            //this.chart1.ChartAreas[0].AlignmentStyle = AreaAlignmentStyles.None;
            //this.chart1.ChartAreas[0].Position.Auto = false;
        }
        #endregion

        #region Methods
        public int GetSeriesCount()
        {
            return this.chart1.Series.Count;
        }

        public void AddDevice(IoChannel dev)
        {
            foreach(Series series in this.chart1.Series)
            {
                if(series.Name == dev.Name)
                    return;
            }
            m_Devices.Add(dev);
            AddSeries(dev.Name);
            this.timerUpdate.Enabled = true;
        }

        public bool RemoveDevice(IoChannel dev)
        {
            bool rv = false;
            for(int i = 0; i < m_Devices.Count; i++)
            {
                if(m_Devices[i].Equals(dev))
                {
                    m_Devices.RemoveAt(i);
                    rv = true;
                    break;
                }
            }
            if(rv == true) RemoveSeries(dev.Name);
            this.timerUpdate.Enabled = false;
            return rv;
        }

        public void RemoveAllDevice()
        {
            int count = m_Devices.Count;
            for(int i = count - 1; i >= 0; i--)
            {
                RemoveDevice(m_Devices[i]);
            }
        }

        private void AddSeries(string name)
        {
            this.chart1.Series.Add(name);
            this.chart1.Series[name].ChartType = m_ChartType;
            this.chart1.Series[name].XValueType = ChartValueType.DateTime;
            //this.chart1.Legends[name].BackColor = Color.Transparent;
            foreach(Legend legend in this.chart1.Legends)
            {
                legend.BackColor = Color.Transparent;
            }
        }

        private void RemoveSeries(string name)
        {
            int index = 0;
            for(int i = 0; i < this.chart1.Series.Count; i++)
            {
                if(this.chart1.Series[i].Name == name)
                    break;
                index++;
            }
            this.chart1.Series.RemoveAt(index);
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            if(m_UpdateDuringVisible && this.Visible == false) return;


            DateTime x = DateTime.Now;
            foreach(IoChannel dev in m_Devices)
            {
                if(m_TestGraph == true)
                    dev.State = m_Random.Next(100).ToString();
                // Test
                if(dev.IoType == IoType.DI || dev.IoType == IoType.DO)
                {
                    this.chart1.Series[dev.Name].YAxisType = AxisType.Secondary;
                    this.chart1.Series[dev.Name].Points.AddXY(x.ToOADate(), dev.State == "ON" ? 1 : 0);
                }
                else
                {
                    this.chart1.Series[dev.Name].YAxisType = AxisType.Primary;
                    this.chart1.Series[dev.Name].Points.AddXY(x.ToOADate(), double.Parse(dev.State));
                }

                if(this.chart1.Series[dev.Name].Points.Count >= Decimal.ToInt32(this.numericUpDownCapa.Value))
                {
                    this.chart1.Series[dev.Name].Points.RemoveAt(0);
                    this.chart1.ResetAutoValues();
                }
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            int interval =
                ((this.dateTimePicker1.Value.Minute - m_InitTime.Minute) * 60 +
                (this.dateTimePicker1.Value.Second - m_InitTime.Second)) * 1000;
            if(interval > 0)
                this.SamplingInterval = interval;
        }
        #endregion
    }
}
