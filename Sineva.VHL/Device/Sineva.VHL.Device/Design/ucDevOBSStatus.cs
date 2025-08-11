using Sineva.VHL.Device.Assem;
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

namespace Sineva.VHL.Device
{
    public partial class ucDevOBSStatus : UCon, IUpdateUCon
    {
        #region Enum
        public enum MeterAreaDirection
        {
            Left,
            Right,
            Top,
            Bottom,
        }
        public enum FrontSensorType
        {
            OBS,
            SOS,
        }
        #endregion

        #region Fields
        private uint m_PreAreaNo = 0;
        private enFrontDetectState m_PreState = enFrontDetectState.enNone;
        private DevOBS m_DevOBS = null;
        private DevSOS m_DevSoS = null;
        private Font m_TextFont = DefaultFont;
        private Color m_TextColor = Color.FromArgb(255, 77, 59);
        private MeterAreaDirection m_AreaDirection = MeterAreaDirection.Left;
        private FrontSensorType m_SensorType = FrontSensorType.OBS;
        private uint m_AreaNo;
        private Rectangle m_RectWorking;
        private Color m_Level1Color = Color.Yellow;
        private Color m_Level2Color = Color.Orange;
        private Color m_Level3Color = Color.Red;
        private enFrontDetectState m_OldFrontState = enFrontDetectState.enStop;
        #endregion

        #region Events
        public event DelVoid_Int ShowObsSimulationTestDlg = null;
        #endregion

        #region Property
        [Description("DevOBS"), Category("Custom")]
        public DevOBS DevOBS
        {
            get { return m_DevOBS; }
            set { m_DevOBS = value; }
        }

        [Description("DevSoS"), Category("Custom")]
        public DevSOS DevSoS
        {
            get { return m_DevSoS; }
            set { m_DevSoS = value; }
        }

        [Description("TextFont"), Category("Custom")]
        public Font TextFont
        {
            get { return m_TextFont; }
            set
            {
                m_TextFont = value;
                Refresh();
            }
        }
        [Description("TextColor"), Category("Custom")]
        public Color TextColor
        {
            get { return m_TextColor; }
            set
            {
                m_TextColor = value;
                Refresh();
            }
        }
        [Description("AreaDirection"), Category("Custom")]
        public MeterAreaDirection AreaDirection
        {
            get { return m_AreaDirection; }
            set
            {
                m_AreaDirection = value;
                Refresh();
            }
        }
        [Description("FrontSensorType"), Category("Custom")]
        public FrontSensorType SensorType
        {
            get { return m_SensorType; }
            set
            {
                m_SensorType = value;
            }
        }


        [Description("AreaNo"), Category("Custom")]
        public uint AreaNo
        {
            get { return m_AreaNo; }
            set
            {
                m_AreaNo = value;
                Refresh();
            }
        }
        [Description("Level1Color"), Category("Custom")]
        public Color Level1Color
        {
            get { return m_Level1Color; }
            set
            {
                m_Level1Color = value;
                Refresh();
            }
        }
        [Description("Level2Color"), Category("Custom")]
        public Color Level2Color
        {
            get { return m_Level2Color; }
            set
            {
                m_Level2Color = value;
                Refresh();
            }
        }
        [Description("Level3Color"), Category("Custom")]
        public Color Level3Color
        {
            get { return m_Level3Color; }
            set
            {
                m_Level3Color = value;
                Refresh();
            }
        }
        #endregion

        #region Constructor
        public ucDevOBSStatus() : base(OperateMode.Manual)
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SizeChanged += UcOBSStatus_SizeChanged;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Size = new Size(90, 180);
        }
        #endregion

        #region Method
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            SizeF txtSize = g.MeasureString("Area", m_TextFont);
            SizeF areaNoSize = g.MeasureString(m_AreaNo.ToString(), m_TextFont);
            int fltX = m_RectWorking.Left;
            int fltY = m_RectWorking.Top;
            int fontX = m_RectWorking.Left;
            int fontY = m_RectWorking.Top;
            switch (m_AreaDirection)
            {
                case MeterAreaDirection.Left:

                    var l1 = new Rectangle(new Point(fltX, fltY), new Size(m_RectWorking.Width / 3 - 5, m_RectWorking.Height - (int)txtSize.Height - 10));
                    g.DrawRectangle(new Pen(new SolidBrush(Color.White), 2), l1);
                    g.FillRectangle(new SolidBrush(m_Level1Color), l1);

                    var l2 = new Rectangle(new Point(fltX + m_RectWorking.Width / 3, fltY), new Size(m_RectWorking.Width / 3 - 5, m_RectWorking.Height - (int)txtSize.Height - 10));
                    g.DrawRectangle(new Pen(new SolidBrush(Color.White), 2), l2);
                    g.FillRectangle(new SolidBrush(m_Level2Color), l2);

                    var l3 = new Rectangle(new Point(fltX + m_RectWorking.Width / 3 * 2, fltY), new Size(m_RectWorking.Width / 3 - 5, m_RectWorking.Height - (int)txtSize.Height - 10));
                    g.DrawRectangle(new Pen(new SolidBrush(Color.White), 2), l3);
                    g.FillRectangle(new SolidBrush(m_Level3Color), l3);

                    fontX = m_RectWorking.Left + m_RectWorking.Width / 2 - (int)(txtSize.Width + areaNoSize.Width) / 2;
                    g.DrawString("Area", m_TextFont, new SolidBrush(m_TextColor), fontX, m_RectWorking.Height - (int)txtSize.Height);
                    g.DrawString(m_AreaNo.ToString(), m_TextFont, new SolidBrush(m_TextColor), fontX + txtSize.Width, m_RectWorking.Height - (int)txtSize.Height);

                    break;
                case MeterAreaDirection.Right:
                    l3 = new Rectangle(new Point(fltX, fltY), new Size(m_RectWorking.Width / 3 - 5, m_RectWorking.Height - (int)txtSize.Height - 10));
                    g.DrawRectangle(new Pen(new SolidBrush(Color.White), 2), l3);
                    g.FillRectangle(new SolidBrush(m_Level3Color), l3);

                    l2 = new Rectangle(new Point(fltX + m_RectWorking.Width / 3, fltY), new Size(m_RectWorking.Width / 3 - 5, m_RectWorking.Height - (int)txtSize.Height - 10));
                    g.DrawRectangle(new Pen(new SolidBrush(Color.White), 2), l2);
                    g.FillRectangle(new SolidBrush(m_Level2Color), l2);

                    l1 = new Rectangle(new Point(fltX + m_RectWorking.Width / 3 * 2, fltY), new Size(m_RectWorking.Width / 3 - 5, m_RectWorking.Height - (int)txtSize.Height - 10));
                    g.DrawRectangle(new Pen(new SolidBrush(Color.White), 2), l1);
                    g.FillRectangle(new SolidBrush(m_Level1Color), l1);

                    fontX = m_RectWorking.Left + m_RectWorking.Width / 2 - (int)(txtSize.Width + areaNoSize.Width) / 2;
                    g.DrawString("Area", m_TextFont, new SolidBrush(m_TextColor), fontX, m_RectWorking.Height - (int)txtSize.Height);
                    g.DrawString(m_AreaNo.ToString(), m_TextFont, new SolidBrush(m_TextColor), fontX + txtSize.Width, m_RectWorking.Height - (int)txtSize.Height);

                    break;
                case MeterAreaDirection.Top:
                    l1 = new Rectangle(new Point(fltX, fltY + 5), new Size(m_RectWorking.Width - 5, (m_RectWorking.Height - (int)txtSize.Height) / 3 - 10));
                    g.DrawRectangle(new Pen(new SolidBrush(Color.White), 2), l1);
                    g.FillRectangle(new SolidBrush(m_Level1Color), l1);

                    l2 = new Rectangle(new Point(fltX, (int)(fltY + (m_RectWorking.Height - txtSize.Height) / 3 + 5)), new Size(m_RectWorking.Width - 5, (m_RectWorking.Height - (int)txtSize.Height) / 3 - 10));
                    g.DrawRectangle(new Pen(new SolidBrush(Color.White), 2), l2);
                    g.FillRectangle(new SolidBrush(m_Level2Color), l2);

                    l3 = new Rectangle(new Point(fltX, (int)(fltY + (m_RectWorking.Height - txtSize.Height) / 3 * 2 + 5)), new Size(m_RectWorking.Width - 5, (m_RectWorking.Height - (int)txtSize.Height) / 3 - 10));
                    g.DrawRectangle(new Pen(new SolidBrush(Color.White), 2), l3);
                    g.FillRectangle(new SolidBrush(m_Level3Color), l3);

                    fontX = m_RectWorking.Left + m_RectWorking.Width / 2 - (int)(txtSize.Width + areaNoSize.Width) / 2;
                    g.DrawString("Area", m_TextFont, new SolidBrush(m_TextColor), fontX, m_RectWorking.Height - (int)txtSize.Height + 10);
                    g.DrawString(m_AreaNo.ToString(), m_TextFont, new SolidBrush(m_TextColor), fontX + txtSize.Width + 2, m_RectWorking.Height - (int)txtSize.Height + 10);


                    break;
                case MeterAreaDirection.Bottom:
                    l3 = new Rectangle(new Point(fltX, fltY + (int)txtSize.Height + 5), new Size(m_RectWorking.Width - 5, (m_RectWorking.Height - (int)txtSize.Height) / 3 - 10));
                    g.DrawRectangle(new Pen(new SolidBrush(Color.White), 2), l3);
                    g.FillRectangle(new SolidBrush(m_Level3Color), l3);

                    l2 = new Rectangle(new Point(fltX, (int)(fltY + (int)txtSize.Height + (m_RectWorking.Height - txtSize.Height) / 3/* + 5*/)), new Size(m_RectWorking.Width - 5, (m_RectWorking.Height - (int)txtSize.Height) / 3 - 10));
                    g.DrawRectangle(new Pen(new SolidBrush(Color.White), 2), l2);
                    g.FillRectangle(new SolidBrush(m_Level2Color), l2);

                    l1 = new Rectangle(new Point(fltX, (int)(fltY + (int)txtSize.Height + (m_RectWorking.Height - txtSize.Height) / 3 * 2 - 5)), new Size(m_RectWorking.Width - 5, (m_RectWorking.Height - (int)txtSize.Height) / 3 - 10));
                    g.DrawRectangle(new Pen(new SolidBrush(Color.White), 2), l1);
                    g.FillRectangle(new SolidBrush(m_Level1Color), l1);

                    fontX = m_RectWorking.Left + m_RectWorking.Width / 2 - (int)(txtSize.Width + areaNoSize.Width) / 2;
                    g.DrawString("Area", m_TextFont, new SolidBrush(m_TextColor), fontX, fltY);
                    g.DrawString(m_AreaNo.ToString(), m_TextFont, new SolidBrush(m_TextColor), fontX + txtSize.Width + 2, fltY);

                    break;
                default:
                    break;
            }

        }

        private void UcOBSStatus_SizeChanged(object sender, EventArgs e)
        {
            m_RectWorking = new Rectangle(5, 5, this.Width - 10, this.Height - 10);
        }

        private void ucOBSStatus_Load(object sender, EventArgs e)
        {
        }

        public void UpdateState()
        {
            try
            {
                if (m_SensorType == FrontSensorType.OBS)
                {
                    if (m_DevOBS != null)
                    {
                        uint no = m_DevOBS.GetOBS();
                        if (no != m_AreaNo) { m_AreaNo = no; Invalidate(); }
                        enFrontDetectState curState = m_DevOBS.GetFrontDetectState();
                        if (curState != m_OldFrontState)
                        {
                            switch (curState)
                            {
                                case enFrontDetectState.enNone:
                                    Level1Color = Color.Transparent;
                                    Level2Color = Color.Transparent;
                                    Level3Color = Color.Transparent;
                                    break;
                                case enFrontDetectState.enDeccelation1:
                                    Level1Color = Color.Yellow;
                                    Level2Color = Color.Transparent;
                                    Level3Color = Color.Transparent;
                                    break;
                                case enFrontDetectState.enDeccelation2:
                                    Level1Color = Color.Yellow;
                                    Level2Color = Color.Orange;
                                    Level3Color = Color.Transparent;
                                    break;
                                case enFrontDetectState.enStop:
                                    Level1Color = Color.Yellow;
                                    Level2Color = Color.Orange;
                                    Level3Color = Color.Red;
                                    break;
                                default:
                                    Level1Color = Color.Transparent;
                                    Level2Color = Color.Transparent;
                                    Level3Color = Color.Transparent;
                                    break;
                            }
                            m_OldFrontState = curState;
                        }
                    }
                }
                else if (m_SensorType == FrontSensorType.SOS)
                {
                    if (m_DevSoS != null)
                    {
                        uint no = m_DevSoS.GetOBS();
                        if (no != m_AreaNo) { m_AreaNo = no; Invalidate(); }
                        enFrontDetectState curState = m_DevSoS.GetFrontDetectState();
                        if (curState != m_OldFrontState)
                        {
                            int level = (int)curState;
                            int nn = (level - 1) / 3;
                            int mod = (level - 1) % 3;
                            if (curState == enFrontDetectState.enNone)
                            {
                                Level1Color = Color.Transparent;
                                Level2Color = Color.Transparent;
                                Level3Color = Color.Transparent;
                            }
                            else if (curState > enFrontDetectState.enDeccelation8)
                            {
                                Level1Color = Color.Red;
                                Level2Color = Color.Red;
                                Level3Color = Color.Red;
                            }
                            else
                            {
                                if (nn == 0)
                                {
                                    Level1Color = mod == 0 ? Color.Yellow : mod == 1 ? Color.Orange : Color.Red;
                                    Level2Color = Color.Transparent;
                                    Level3Color = Color.Transparent;
                                }
                                else if (nn == 1)
                                {
                                    Level1Color = Color.Red;
                                    Level2Color = mod == 0 ? Color.Yellow : mod == 1 ? Color.Orange : Color.Red;
                                    Level3Color = Color.Transparent;
                                }
                                else if (nn == 2)
                                {
                                    Level1Color = Color.Red;
                                    Level2Color = Color.Red;
                                    Level3Color = mod == 0 ? Color.Yellow : mod == 1 ? Color.Orange : Color.Red;
                                }
                            }

                            m_OldFrontState = curState;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        public bool Initialize()
        {
            bool rv = true;
            return rv;
        }
        #endregion

        private void ucDevOBSStatus_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (AppConfig.Instance.Simulation.MY_DEBUG)
            {
                ShowObsSimulationTestDlg?.Invoke((int)m_SensorType);
            }
        }
    }
}
