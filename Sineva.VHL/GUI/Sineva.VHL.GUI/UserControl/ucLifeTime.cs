using Sineva.VHL.Data;
using Sineva.VHL.Device;
using Sineva.VHL.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.GUI
{
    public partial class ucLifeTime : UCon, IUpdateUCon
    {
        #region Fields
        private const int OnePageItems = 20;
        private SortedList<uint, GeneralObject[]> m_PageList = new SortedList<uint, GeneralObject[]>();
        private List<string> m_TrendLoggingItems = new List<string>();
        private uint m_CurPageNo;

        private List<Label> m_SvIdList = new List<Label>();
        private List<Label> m_DescList = new List<Label>();
        private List<Label> m_ValueList = new List<Label>();
        #endregion

        #region Constructor
        public ucLifeTime()
            : base(OperateMode.None)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.CacheText, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.ContainerControl, true);
            UpdateStyles();
        }
        #endregion

        #region Methods
        public bool Initialize()
        {
            bool rv = true;
            try
            {
                m_PageList.Clear();
                List<GeneralObject> familyItems = new List<GeneralObject>();
                int pageCount, pageTotalNo = 0;
                // Add Items
                if (GV.LifeTimeItems.Count > 0)
                {
                    familyItems.Clear();
                    familyItems.AddRange(GV.LifeTimeItems.ToArray());
                    pageCount = (familyItems.Count / (OnePageItems + 1)) + 1;
                    for (int i = 0; i < pageCount; i++)
                    {
                        int itemsCount = OnePageItems;
                        if (i == pageCount - 1) itemsCount = (familyItems.Count % ((i + 1) * OnePageItems + 1));
                        GeneralObject[] pageItemArray = familyItems.Skip(i * OnePageItems).Take(itemsCount).ToArray();
                        m_PageList.Add((uint)(pageTotalNo + i), pageItemArray);
                    }
                    pageTotalNo += pageCount;
                }

                m_SvIdList.Add(lblSv01); m_SvIdList.Add(lblSv02); m_SvIdList.Add(lblSv03); m_SvIdList.Add(lblSv04); m_SvIdList.Add(lblSv05);
                m_SvIdList.Add(lblSv06); m_SvIdList.Add(lblSv07); m_SvIdList.Add(lblSv08); m_SvIdList.Add(lblSv09); m_SvIdList.Add(lblSv10);
                m_SvIdList.Add(lblSv11); m_SvIdList.Add(lblSv12); m_SvIdList.Add(lblSv13); m_SvIdList.Add(lblSv14); m_SvIdList.Add(lblSv15);
                m_SvIdList.Add(lblSv16); m_SvIdList.Add(lblSv17); m_SvIdList.Add(lblSv18); m_SvIdList.Add(lblSv19); m_SvIdList.Add(lblSv20);
                m_DescList.Add(lblDesc01); m_DescList.Add(lblDesc02); m_DescList.Add(lblDesc03); m_DescList.Add(lblDesc04); m_DescList.Add(lblDesc05);
                m_DescList.Add(lblDesc06); m_DescList.Add(lblDesc07); m_DescList.Add(lblDesc08); m_DescList.Add(lblDesc09); m_DescList.Add(lblDesc10);
                m_DescList.Add(lblDesc11); m_DescList.Add(lblDesc12); m_DescList.Add(lblDesc13); m_DescList.Add(lblDesc14); m_DescList.Add(lblDesc15);
                m_DescList.Add(lblDesc16); m_DescList.Add(lblDesc17); m_DescList.Add(lblDesc18); m_DescList.Add(lblDesc19); m_DescList.Add(lblDesc20);
                m_ValueList.Add(lblValue01); m_ValueList.Add(lblValue02); m_ValueList.Add(lblValue03); m_ValueList.Add(lblValue04); m_ValueList.Add(lblValue05);
                m_ValueList.Add(lblValue06); m_ValueList.Add(lblValue07); m_ValueList.Add(lblValue08); m_ValueList.Add(lblValue09); m_ValueList.Add(lblValue10);
                m_ValueList.Add(lblValue11); m_ValueList.Add(lblValue12); m_ValueList.Add(lblValue13); m_ValueList.Add(lblValue14); m_ValueList.Add(lblValue15);
                m_ValueList.Add(lblValue16); m_ValueList.Add(lblValue17); m_ValueList.Add(lblValue18); m_ValueList.Add(lblValue19); m_ValueList.Add(lblValue20);
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }

            return rv;
        }

        public void UpdateState()
        {
            if (!this.Visible) return;
            try
            {
                if (m_PageList.ContainsKey(m_CurPageNo))
                {
                    for (int i = 0; i < OnePageItems; i++)
                    {
                        if (m_PageList[m_CurPageNo].Length > i)
                        {
                            if (m_SvIdList[i].Text != m_PageList[m_CurPageNo][i].Name) m_SvIdList[i].Text = m_PageList[m_CurPageNo][i].Name;
                            if (m_DescList[i].Text != m_PageList[m_CurPageNo][i].Description)
                            {
                                m_DescList[i].Text = m_PageList[m_CurPageNo][i].Description;
                                if (m_TrendLoggingItems.Contains(m_DescList[i].Text)) m_DescList[i].BackColor = Color.Yellow;
                                else m_DescList[i].BackColor = Color.White;
                            }
                            string value = m_PageList[m_CurPageNo][i].Value;
                            if (m_ValueList[i].Text != value) m_ValueList[i].Text = value;
                        }
                        else
                        {
                            if (m_SvIdList[i].Text != string.Empty) m_SvIdList[i].Text = string.Empty;
                            if (m_DescList[i].Text != string.Empty)
                            {
                                m_DescList[i].Text = string.Empty;
                                m_DescList[i].BackColor = Color.White;
                            }
                            if (m_ValueList[i].Text != string.Empty) m_ValueList[i].Text = string.Empty;
                        }
                    }
                }

                string pageText = string.Format("PAGE : {0} / {1}", m_CurPageNo + 1, m_PageList.Count);
                if (lblPageNo.Text != pageText) lblPageNo.Text = pageText;
            }
            catch (Exception err)
            {
                ExceptionLog.WriteLog(err.ToString());
            }
        }
        #endregion

        #region Methods - button event
        private void btnMoveFirst_MouseClick(object sender, MouseEventArgs e)
        {
            m_CurPageNo = 0;
        }

        private void btnMovePrev_MouseClick(object sender, MouseEventArgs e)
        {
            if (m_PageList.Count > m_CurPageNo - 1)
                m_CurPageNo--;
        }

        private void btnMoveNext_MouseClick(object sender, MouseEventArgs e)
        {
            if (m_PageList.Count > m_CurPageNo + 1)
                m_CurPageNo++;
        }

        private void btnMoveLast_MouseClick(object sender, MouseEventArgs e)
        {
            m_CurPageNo = (uint)(m_PageList.Count - 1);
        }
        #endregion
    }
}
