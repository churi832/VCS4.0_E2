using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
    public partial class SwipeViewBook : UserControl
    {
        #region Field
        private readonly char SymbolSelected = '●';
        private readonly char SymbolHidden = '○';

        private SortedList<int, SwipeView> m_ViewPagesList = new SortedList<int, SwipeView>();
        private int m_TouchAreaHeight = 25;
        private int m_CurrentPageNo = 0;
        private System.Windows.Forms.Timer m_TimerTouchSensor = null;

        private bool m_TouchHoverBegin = false;
        #endregion

        #region Property
        public int TouchAreaHeight
        {
            get => m_TouchAreaHeight;
            set
            {
                if(value > 20) m_TouchAreaHeight = value;
                tableLayoutPanel1.RowStyles[2].Height = m_TouchAreaHeight;
            }
        }
        #endregion

        #region Constructor
        public SwipeViewBook()
        {
            InitializeComponent();

            if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                m_TimerTouchSensor = new Timer();
                m_TimerTouchSensor.Interval = 10;
                m_TimerTouchSensor.Tick += TimerTouchSensor_Tick;
                m_TimerTouchSensor.Start();
            }

            tableLayoutPanel1.RowStyles[2].Height = m_TouchAreaHeight;
        }
        #endregion

        #region Event Handler
        private void TimerTouchSensor_Tick(object sender, EventArgs e)
        {
            if(touchLabel1.IsTouchSwipeLeft)
            {
                SwipePage(m_CurrentPageNo + 1);
            }
            else if(touchLabel1.IsTouchSwipeRight)
            {
                SwipePage(m_CurrentPageNo - 1);
            }
            else if(touchLabel1.IsTouchHovering)
            {
                m_TouchHoverBegin = true;

                int offset = (int)((float)(panelPages.Width / touchLabel1.Width) * touchLabel1.TouchHoveringLastPixels.X);
                HoverPage(offset);
            }
            else
            {
                if(m_TouchHoverBegin)
                {
                    SwipePage(m_CurrentPageNo);
                    m_TouchHoverBegin = false;
                }
            }
        }
        private void CbViewPageList_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string pageName = (sender as ComboBox).SelectedItem.ToString();
            int pageNo = -1;
            foreach(KeyValuePair<int, SwipeView> pair in m_ViewPagesList)
            {
                if(pair.Value.PageName == pageName)
                {
                    pageNo = pair.Key;
                    break;
                }
            }

            if(pageNo > 0)
            {
                SwipePage(pageNo);
            }
        }
        #endregion

        #region Method
        public bool AddPage(SwipeView view)
        {
            for(int i = 0; i < m_ViewPagesList.Values.Count; i++)
                if(m_ViewPagesList.Values[i].PageName == view.PageName) return false;

            panelPages.SuspendLayout();
            cbViewPageList.SuspendLayout();

            view.Location = new Point(0, 0);
            view.Dock = DockStyle.None;
            view.Size = panelPages.Size;
            view.Margin = new Padding(0);
            view.Padding = new Padding(0);

            int newPageNo = 0;
            do
            {
                newPageNo++;
            } while(m_ViewPagesList.ContainsKey(newPageNo) == true);
            view.PageNo = newPageNo;
            m_ViewPagesList.Add(newPageNo, view);
            panelPages.Controls.Add(view);

            cbViewPageList.Items.Clear();
            foreach(KeyValuePair<int, SwipeView> pair in m_ViewPagesList)
            {
                pair.Value.PageNo = pair.Key;
                pair.Value.Location = new Point(pair.Value.Width * (pair.Key - 1), 0);

                foreach(UserControl ctrl in panelPages.Controls)
                {
                    bool typeEquals = false;
                    typeEquals |= ctrl.GetType() == typeof(SwipeView);
                    typeEquals |= ctrl.GetType().BaseType == typeof(SwipeView);
                    if(typeEquals && (ctrl as SwipeView).PageName == pair.Value.PageName)
                    {
                        (ctrl as SwipeView).PageNo = pair.Value.PageNo;
                        ctrl.Location = pair.Value.Location;
                        break;
                    }
                }
                cbViewPageList.Items.Add(pair.Value.PageName);
            }

            m_CurrentPageNo = m_ViewPagesList.Keys.First();
            panelPages.ResumeLayout();


            UpdatePageSymbol();

            return true;
        }
        private void UpdatePageSymbol()
        {
            string pageSymbol = string.Empty;
            for(int i = 0; i < m_ViewPagesList.Keys.Count; i++)
                pageSymbol += m_ViewPagesList.Keys[i] == m_CurrentPageNo ? SymbolSelected : SymbolHidden;
            touchLabel1.Text = pageSymbol;

            lblPageCurrent.Text = m_CurrentPageNo.ToString();
            lblPageTotal.Text = m_ViewPagesList.Keys.Count.ToString();
        }
        private void SwipePage(int targetPageNo)
        {
            if(m_ViewPagesList.ContainsKey(targetPageNo) == false) targetPageNo = m_CurrentPageNo;

            foreach(KeyValuePair<int, SwipeView> pair in m_ViewPagesList)
            {
                int pageNo = pair.Key;
                Point newLocation = new Point((pageNo - targetPageNo) * panelPages.Width, 0); // 가로 방향만 대응

                pair.Value.SetPageSwipe(newLocation, 200);
            }
            m_CurrentPageNo = targetPageNo;
            UpdatePageSymbol();

            cbViewPageList.SelectedItem = m_ViewPagesList[m_CurrentPageNo].PageName;
        }
        private void HoverPage(int offset)
        {
            foreach(KeyValuePair<int, SwipeView> pair in m_ViewPagesList)
            {
                int pageNo = pair.Key;
                Point newLocation = new Point((pageNo - m_CurrentPageNo) * panelPages.Width + offset, 0);

                pair.Value.SetPageHover(newLocation);
            }
        }
        #endregion
    }
}
