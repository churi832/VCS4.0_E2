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
    public delegate void SwipeViewPageChanged(int a);
    public partial class SwipeViewContainer : UserControl
    {
        #region Field
        private System.ComponentModel.IContainer components = null;

        private Panel panelPages = new Panel();
        private TouchLabel touchLabel1 = new TouchLabel();

        private SortedList<int, SwipeView> m_ViewPagesList = new SortedList<int, SwipeView>();
        private int m_CurrentPageNo = 0;
        private System.Windows.Forms.Timer m_TimerTouchSensor = null;

        private bool m_TouchHoverBegin = false;

        private PictureBox pictureBoxTop = new PictureBox();
        private PictureBox pictureBoxBot = new PictureBox();
        private PictureBox pictureBoxLeft = new PictureBox();
        private PictureBox pictureBoxRight = new PictureBox();
        private PictureBox pictureBoxLT = new PictureBox();
        private PictureBox pictureBoxRT = new PictureBox();
        private PictureBox pictureBoxLB = new PictureBox();
        private PictureBox pictureBoxRB = new PictureBox();
        private List<PictureBox> pictureBoxs = new List<PictureBox>();
        private List<Image> overImages = new List<Image>();
        private List<Image> leaveImages = new List<Image>();
        #endregion
        #region Events
        //public event DelVoid_Int ChangedWindow = null;
        public event SwipeViewPageChanged ChangedWindow = null;
        #endregion

        public SwipeViewContainer()
        {
            this.panelPages = new System.Windows.Forms.Panel();
            this.touchLabel1 = new TouchLabel();

            this.SuspendLayout();
            // 
            // panelPages
            // 
            this.panelPages.Location = new System.Drawing.Point(0, 0);
            this.panelPages.Name = "panelPages";
            this.panelPages.Size = new System.Drawing.Size(200, 100);
            this.panelPages.TabIndex = 0;
            // 
            // touchLabel1
            // 
            this.touchLabel1.Location = new System.Drawing.Point(0, 0);
            this.touchLabel1.Name = "touchLabel1";
            this.touchLabel1.Size = new System.Drawing.Size(100, 23);
            this.touchLabel1.TabIndex = 0;
            this.touchLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.touchLabel1.UserBackColorDefault = System.Drawing.Color.OrangeRed;
            this.touchLabel1.UserBackColorMouseDown = System.Drawing.Color.LightBlue;
            this.touchLabel1.UserBackColorMouseEnter = System.Drawing.Color.YellowGreen;
            this.touchLabel1.UserForeColorDefault = System.Drawing.Color.White;
            this.touchLabel1.UserForeColorMouseDown = System.Drawing.Color.White;
            this.touchLabel1.UserForeColorMouseEnter = System.Drawing.Color.Blue;
            // 
            // SwipeViewContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Name = "SwipeViewContainer";
            this.Size = new System.Drawing.Size(250, 250);
            this.Resize += new System.EventHandler(this.SwipeViewContainer_Resize);

            #region pictureFrame
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRB)).BeginInit();

            // 
            // pictureBoxRight
            // 
            this.pictureBoxRight.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pictureBoxRight.Location = new System.Drawing.Point(140, 0);
            this.pictureBoxRight.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxRight.Name = "pictureBoxRight";
            this.pictureBoxRight.Size = new System.Drawing.Size(10, 150);
            this.pictureBoxRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxRight.TabIndex = 3;
            this.pictureBoxRight.TabStop = false;
            this.pictureBoxRight.LocationChanged += new System.EventHandler(this.eventPictureBoxRight_LocationChanged);
            // 
            // pictureBoxBot
            // 
            this.pictureBoxBot.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxBot.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pictureBoxBot.Location = new System.Drawing.Point(10, 140);
            this.pictureBoxBot.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxBot.Name = "pictureBoxBot";
            this.pictureBoxBot.Size = new System.Drawing.Size(130, 10);
            this.pictureBoxBot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxBot.TabIndex = 1;
            this.pictureBoxBot.TabStop = false;
            this.pictureBoxBot.LocationChanged += new System.EventHandler(this.eventPictureBoxBot_LocationChanged);
            // 
            // pictureBoxTop
            // 
            this.pictureBoxTop.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBoxTop.Location = new System.Drawing.Point(10, 0);
            this.pictureBoxTop.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxTop.Name = "pictureBoxTop";
            this.pictureBoxTop.Size = new System.Drawing.Size(130, 10);
            this.pictureBoxTop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxTop.TabIndex = 0;
            this.pictureBoxTop.TabStop = false;
            // 
            // pictureBoxLeft
            // 
            this.pictureBoxLeft.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBoxLeft.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxLeft.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxLeft.Name = "pictureBox3";
            this.pictureBoxLeft.Size = new System.Drawing.Size(10, 150);
            this.pictureBoxLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLeft.TabIndex = 2;
            this.pictureBoxLeft.TabStop = false;
            // 
            // pictureBoxLT
            // 
            this.pictureBoxLT.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxLT.Location = new System.Drawing.Point(10, 10);
            this.pictureBoxLT.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxLT.Name = "pictureBoxLT";
            this.pictureBoxLT.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxLT.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLT.TabIndex = 0;
            this.pictureBoxLT.TabStop = false;
            // 
            // pictureBoxRT
            // 
            this.pictureBoxRT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxRT.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxRT.Location = new System.Drawing.Point(120, 10);
            this.pictureBoxRT.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxRT.Name = "pictureBoxRT";
            this.pictureBoxRT.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxRT.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxRT.TabIndex = 4;
            this.pictureBoxRT.TabStop = false;
            // 
            // pictureBoxLB
            // 
            this.pictureBoxLB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBoxLB.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxLB.Location = new System.Drawing.Point(10, 120);
            this.pictureBoxLB.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxLB.Name = "pictureBoxLB";
            this.pictureBoxLB.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxLB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLB.TabIndex = 4;
            this.pictureBoxLB.TabStop = false;
            // 
            // pictureBoxRB
            // 
            this.pictureBoxRB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxRB.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxRB.Location = new System.Drawing.Point(120, 120);
            this.pictureBoxRB.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxRB.Name = "pictureBox8";
            this.pictureBoxRB.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxRB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxRB.TabIndex = 4;
            this.pictureBoxRB.TabStop = false;
            pictureBoxs.Add(pictureBoxLT);
            pictureBoxs.Add(pictureBoxRT);
            pictureBoxs.Add(pictureBoxLB);
            pictureBoxs.Add(pictureBoxRB);
            pictureBoxs.Add(pictureBoxTop);
            pictureBoxs.Add(pictureBoxBot);
            pictureBoxs.Add(pictureBoxLeft);
            pictureBoxs.Add(pictureBoxRight);

            overImages.Add(Properties.Resources.FrameLT);
            overImages.Add(Properties.Resources.FrameRT);
            overImages.Add(Properties.Resources.FrameLB);
            overImages.Add(Properties.Resources.FrameRB);
            overImages.Add(Properties.Resources.FrameTop);
            overImages.Add(Properties.Resources.FrameBot);
            overImages.Add(Properties.Resources.FrameLeft);
            overImages.Add(Properties.Resources.FrameRight);
            leaveImages.Add(Properties.Resources.FrameLT1);
            leaveImages.Add(Properties.Resources.FrameRT1);
            leaveImages.Add(Properties.Resources.FrameLB1);
            leaveImages.Add(Properties.Resources.FrameRB1);
            leaveImages.Add(Properties.Resources.FrameTop1);
            leaveImages.Add(Properties.Resources.FrameBot1);
            leaveImages.Add(Properties.Resources.FrameLeft1);
            leaveImages.Add(Properties.Resources.FrameRight1);

            for(int i = 0; i < pictureBoxs.Count; i++)
            {
                pictureBoxs[i].BackgroundImage = leaveImages[i];
                this.Controls.Add(pictureBoxs[i]);
            }

            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRB)).EndInit();
            this.ResumeLayout(false);
            #endregion

            this.SuspendLayout();
            // 
            // panelPages
            // 
            this.panelPages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPages.Location = new System.Drawing.Point(10, 10);
            this.panelPages.Margin = new System.Windows.Forms.Padding(10);
            this.panelPages.Name = "panelPages";
            this.panelPages.Size = new System.Drawing.Size(this.Size.Width - 20, this.Size.Height - 40); // 좌우 10씩 남기고  상하 10씩 그리고 Label 20
            this.panelPages.TabIndex = 0;
            // 
            // touchLabel1
            // 
            this.touchLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.touchLabel1.BackColor = System.Drawing.SystemColors.Control;
            this.touchLabel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.touchLabel1.Location = new System.Drawing.Point(20, this.Size.Height - 30);
            this.touchLabel1.Name = "touchLabel1";
            this.touchLabel1.Size = new System.Drawing.Size(this.Size.Width - 40, 20);
            this.touchLabel1.TabIndex = 0;
            this.touchLabel1.Text = "Touch";
            this.touchLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.touchLabel1.UserBackColorDefault = System.Drawing.SystemColors.Control;
            this.touchLabel1.UserBackColorMouseDown = System.Drawing.Color.LightBlue;
            this.touchLabel1.UserBackColorMouseEnter = System.Drawing.Color.YellowGreen;
            this.touchLabel1.UserForeColorDefault = System.Drawing.SystemColors.ControlText;
            this.touchLabel1.UserForeColorMouseDown = System.Drawing.Color.White;
            this.touchLabel1.UserForeColorMouseEnter = System.Drawing.Color.Blue;

            this.Controls.Add(this.touchLabel1);
            this.Controls.Add(this.panelPages);
            this.ResumeLayout(false);

            if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                m_TimerTouchSensor = new Timer();
                m_TimerTouchSensor.Interval = 10;
                m_TimerTouchSensor.Tick += TimerTouchSensor_Tick;
                m_TimerTouchSensor.Start();
            }
        }
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

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

        public bool AddPage(SwipeView view)
        {
            for(int i = 0; i < m_ViewPagesList.Values.Count; i++)
                if(m_ViewPagesList.Values[i].PageName == view.PageName) return false;

            panelPages.SuspendLayout();

            view.Location = new Point(0, 0);
            view.Dock = DockStyle.None;
            view.Size = panelPages.Size;
            view.Margin = new Padding(0);
            view.Padding = new Padding(0);
            view.MouseHover += View_MouseHover;
            view.MouseLeave += View_MouseLeave;

            int newPageNo = 0;
            do
            {
                newPageNo++;
            } while(m_ViewPagesList.ContainsKey(newPageNo) == true);
            view.PageNo = newPageNo;
            m_ViewPagesList.Add(newPageNo, view);
            view.ChangedPage += event_ChangedPage;
            panelPages.Controls.Add(view);

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
            }

            m_CurrentPageNo = m_ViewPagesList.Keys.First();
            panelPages.ResumeLayout();
            return true;
        }

        private void event_ChangedPage(int a)
        {
            if(ChangedWindow != null)
            {
                if(a == m_CurrentPageNo) ChangedWindow.Invoke(a);
            }
        }

        public void SetCurPage(int no)
        {
            SwipePage(no);
        }
        private void View_MouseLeave(object sender, EventArgs e)
        {
            for(int i = 0; i < pictureBoxs.Count; i++) pictureBoxs[i].BackgroundImage = leaveImages[i];
        }

        private void View_MouseHover(object sender, EventArgs e)
        {
            for(int i = 0; i < pictureBoxs.Count; i++) pictureBoxs[i].BackgroundImage = overImages[i];
        }

        #region Method
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
        #region Frame Box Method
        private void eventPictureBoxRight_LocationChanged(object sender, EventArgs e)
        {
            //Right 기준
            pictureBoxLT.Location = new System.Drawing.Point(pictureBoxTop.Location.X - 10, pictureBoxTop.Location.Y);
            pictureBoxRT.Location = new System.Drawing.Point(pictureBoxRight.Location.X - 10, pictureBoxRight.Location.Y);
            pictureBoxLB.Location = new System.Drawing.Point(pictureBoxTop.Location.X - 10, pictureBoxBot.Location.Y - 10);
            pictureBoxRB.Location = new System.Drawing.Point(pictureBoxRight.Location.X - 10, pictureBoxBot.Location.Y - 10);
        }
        private void eventPictureBoxBot_LocationChanged(object sender, EventArgs e)
        {
            // Bottom 기준
            pictureBoxLT.Location = new System.Drawing.Point(pictureBoxTop.Location.X - 10, pictureBoxTop.Location.Y);
            pictureBoxRT.Location = new System.Drawing.Point(pictureBoxRight.Location.X - 10, pictureBoxRight.Location.Y);
            pictureBoxLB.Location = new System.Drawing.Point(pictureBoxTop.Location.X - 10, pictureBoxBot.Location.Y - 10);
            pictureBoxRB.Location = new System.Drawing.Point(pictureBoxRight.Location.X - 10, pictureBoxBot.Location.Y - 10);
        }
        #endregion

        private void SwipeViewContainer_Resize(object sender, EventArgs e)
        {
            foreach (SwipeView page in panelPages.Controls) page.Size = panelPages.Size;
            SwipePage(m_CurrentPageNo);
        }
    }
}
