using Sineva.VHL.Data.Alarm;
using Sineva.VHL.Data.Process;
using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Sineva.VHL.GUI
{
    public partial class FormDataDisplay : Form
    {
        Dictionary<enTPD, bool> m_DisplayOn = new Dictionary<enTPD, bool>();
        public FormDataDisplay()
        {
            InitializeComponent();
            EventHandlerManager.Instance.UpdatePathData += Instance_UpdatePathData;

            InitChart();
        }
        private void InitChart()
        {
            checkedListBox1.Items.Clear();
            zoomableChart1.ClearPlot();
            Random rand = new Random();
            string[] tpd_names = Enum.GetNames(typeof(enTPD));
            foreach (string series_name in tpd_names)
            {
                zoomableChart1.Series.Add(series_name);
                zoomableChart1.Series[series_name].XValueType = ChartValueType.DateTime;
                zoomableChart1.Series[series_name].YValueType = ChartValueType.Int32;
                zoomableChart1.Series[series_name].ChartType = SeriesChartType.FastLine;
                int r = rand.Next(0, 255);
                int g = rand.Next(0, 255);
                int b = rand.Next(0, 255);
                zoomableChart1.Series[series_name].Color = System.Drawing.Color.FromArgb(((int)(((byte)(r)))), ((int)(((byte)(g)))), ((int)(((byte)(b)))));
            }
            foreach (enTPD tpd in Enum.GetValues(typeof(enTPD)))
            {
                m_DisplayOn.Add(tpd, false);
                checkedListBox1.Items.Add(tpd);
            }

            zoomableChart1.Areas[0].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Seconds;
            zoomableChart1.Areas[0].AxisX.LabelStyle.Format = "mm:ss";
            zoomableChart1.Areas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            zoomableChart1.Areas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            zoomableChart1.Areas[0].AxisX.MajorGrid.LineColor = Color.Gray;
            zoomableChart1.Areas[0].AxisY.MajorGrid.LineColor = Color.Gray;

            m_DisplayOn[enTPD.VirtualBCR] = true;
            m_DisplayOn[enTPD.TargetBcrDistance] = true;
            m_DisplayOn[enTPD.BCRRemainDistance] = true;
            checkedListBox1.SetItemChecked((int)enTPD.VirtualBCR, true);
            checkedListBox1.SetItemChecked((int)enTPD.TargetBcrDistance, true);
            checkedListBox1.SetItemChecked((int)enTPD.BCRRemainDistance, true);
        }
        private void Instance_UpdatePathData(object pathData, bool b)
        {
            if (this.IsDisposed) return;
            try
            {
                if (zoomableChart1.InvokeRequired)
                {
                    DelVoid_UpdatePathData d = new DelVoid_UpdatePathData(Instance_UpdatePathData);
                    this.Invoke(d, pathData, b);
                }
                else
                {
                    if (pathData == null || b == false)
                    {
                        for (int i = 0; i < zoomableChart1.Series.Count; i++)
                        {
                            zoomableChart1.Series[i].Points.Clear();
                        }
                        this.zoomableChart1.ResetAutoValues();
                    }
                    else
                    {
                        if (zoomableChart1.Series.Count > 1)
                        {
                            DateTime x = DateTime.Now;
                            Dictionary<enTPD, string> tpds = pathData as Dictionary<enTPD, string>;
                            if (tpds != null)
                            {
                                int max_count = 0;
                                double val = 0.0f;
                                foreach (KeyValuePair<enTPD, string> item in tpds)
                                {
                                    enTPD key = item.Key;
                                    if (m_DisplayOn[key])
                                    {
                                        if (double.TryParse(item.Value, out val))
                                        {
                                            string name = Enum.GetName(typeof(enTPD), key);
                                            zoomableChart1.Series[name].Points.AddXY(x, val);
                                            if (zoomableChart1.Series[name].Points.Count > 200)
                                            {
                                                this.zoomableChart1.Series[name].Points.RemoveAt(0);
                                            }
                                            max_count = Math.Max(max_count, zoomableChart1.Series[name].Points.Count);
                                        }
                                    }
                                }
                                if (max_count >= 200)
                                {
                                    this.zoomableChart1.ResetAutoValues();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void MenuHide_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckState cur = e.CurrentValue;
            CheckState next = e.NewValue;
            if (checkedListBox1.SelectedItem != null)
            {
                enTPD select = (enTPD)checkedListBox1.SelectedItem;
                if (next == CheckState.Checked) m_DisplayOn[select] = true;
                else m_DisplayOn[select] = false;
            }
        }
    }
}
