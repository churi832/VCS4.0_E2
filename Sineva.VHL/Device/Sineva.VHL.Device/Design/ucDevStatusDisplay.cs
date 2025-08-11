using Sineva.VHL.Library;
using Sineva.VHL.Library.IO;
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

    public partial class ucDevStatusDisplay : UCon, IUpdateUCon
    {

        #region Fields
        private string m_ExceptionMessage = string.Empty;
        private IoTag m_FirstStateIoTag = new IoTag();
        private IoTag m_SecondStateIoTag = new IoTag();
        private enDeviceType m_DeviceType = enDeviceType.AntiDrop;
        private Color m_BackColorOfFirstState;
        private Color m_BackColorOfSecondState;
        private Color m_BackColorOfMiddleState;
        private Color m_ForeColorOfFirstState;
        private Color m_ForeColorOfSecondState;
        private Color m_ForeColorOfMiddleState;
        #endregion

        #region Constructor
        public ucDevStatusDisplay() : base(OperateMode.Manual)
        {
            InitializeComponent();
        }
        #endregion

        #region Property
        [Category("! Setting - Device Select"), Description("Steer:Left AntiDrop:Lock OutRigger:Lock Gripper:Open")]
        public IoTag FirstStateIoTag
        {
            get { return m_FirstStateIoTag; }
            set { m_FirstStateIoTag = value; }
        }
        [Category("! Setting - Device Select"), Description("Steer:Right AntiDrop:UnLock OutRigger:UnLock Gripper:Close")]
        public IoTag SecondStateIoTag
        {
            get { return m_SecondStateIoTag; }
            set { m_SecondStateIoTag = value; }
        }
        [Category("! setting - device select")]
        public enDeviceType CurrentDeviceType
        {
            get { return m_DeviceType; }
            set { m_DeviceType = value; }
        }

        [Category("! Setting - UI")]
        public Color BackColorOfFirstState
        {
            get { return m_BackColorOfFirstState; }
            set { m_BackColorOfFirstState = value; }
        }
        [Category("! Setting - UI")]
        public Color BackColorOfSecondState
        {
            get { return m_BackColorOfSecondState; }
            set { m_BackColorOfSecondState = value; }
        }
        [Category("! Setting - UI")]
        public Color BackColorOfMiddleState
        {
            get { return m_BackColorOfMiddleState; }
            set { m_BackColorOfMiddleState = value; }
        }
        [Category("! Setting - UI")]
        public Color ForeColorOfFirstState
        {
            get { return m_ForeColorOfFirstState; }
            set { m_ForeColorOfFirstState = value; }
        }
        [Category("! Setting - UI")]
        public Color ForeColorOfSecondState
        {
            get { return m_ForeColorOfSecondState; }
            set { m_ForeColorOfSecondState = value; }
        }
        [Category("! Setting - UI")]
        public Color ForeColorOfMiddleState
        {
            get { return m_ForeColorOfMiddleState; }
            set { m_ForeColorOfMiddleState = value; }
        }
        [Category("! Setting - UI")]
        public string TextValue
        {
            get { return this.label1.Text; }
            set { this.label1.Text = value; }
        }
        [Category("! Setting - UI")]
        public Font TextFont
        {
            get { return this.label1.Font; }
            set { this.label1.Font = value; }
        }
        #endregion

        #region Method
        public bool Initialize()
        {
            return true;
        }

        public void UpdateState()
        {
            if (!this.Visible)
                return;

            try
            {
                if (m_FirstStateIoTag != null && m_FirstStateIoTag.GetChannel() != null && m_SecondStateIoTag != null && m_SecondStateIoTag.GetChannel() != null)
                {
                    switch (CurrentDeviceType)
                    {
                        case enDeviceType.AntiDrop:
                        case enDeviceType.OutRigger:
                            if (m_FirstStateIoTag.GetDi() && !m_SecondStateIoTag.GetDi())
                            {
                                label1.Text = "Lock";
                                label1.BackColor = m_BackColorOfFirstState;
                                label1.ForeColor = m_ForeColorOfFirstState;
                            }
                            else if (!m_FirstStateIoTag.GetDi() && m_SecondStateIoTag.GetDi())
                            {
                                label1.Text = "UnLock";
                                label1.BackColor = m_BackColorOfSecondState;
                                label1.ForeColor = m_ForeColorOfSecondState;
                            }
                            else
                            {
                                label1.Text = "None";
                                label1.BackColor = m_BackColorOfMiddleState;
                                label1.ForeColor = m_ForeColorOfMiddleState;
                            }
                            break;
                        case enDeviceType.Gripper:
                            if (m_FirstStateIoTag.GetDi() && !m_SecondStateIoTag.GetDi())
                            {
                                label1.Text = "Open";
                                label1.BackColor = m_BackColorOfFirstState;
                                label1.ForeColor = m_ForeColorOfFirstState;
                            }
                            else if (!m_FirstStateIoTag.GetDi() && m_SecondStateIoTag.GetDi())
                            {
                                label1.Text = "Close";
                                label1.BackColor = m_BackColorOfSecondState;
                                label1.ForeColor = m_ForeColorOfSecondState;
                            }
                            else
                            {
                                label1.Text = "None";
                                label1.BackColor = m_BackColorOfMiddleState;
                                label1.ForeColor = m_ForeColorOfMiddleState;
                            }
                            break;
                        case enDeviceType.Steer:
                            if (m_FirstStateIoTag.GetDi() && !m_SecondStateIoTag.GetDi())
                            {
                                label1.Text = "Left";
                                label1.BackColor = m_BackColorOfFirstState;
                                label1.ForeColor = m_ForeColorOfFirstState;
                            }
                            else if (!m_FirstStateIoTag.GetDi() && m_SecondStateIoTag.GetDi())
                            {
                                label1.Text = "Right";
                                label1.BackColor = m_BackColorOfSecondState;
                                label1.ForeColor = m_ForeColorOfSecondState;
                            }
                            else
                            {
                                label1.Text = "None";
                                label1.BackColor = m_BackColorOfMiddleState;
                                label1.ForeColor = m_ForeColorOfMiddleState;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    label1.Text = "None";
                    label1.BackColor = m_BackColorOfMiddleState;
                    label1.ForeColor = m_ForeColorOfMiddleState;
                }
            }
            catch (Exception e)
            {
                if (m_ExceptionMessage != e.Message)
                {
                    m_ExceptionMessage = e.Message;
                    string log = string.Format("UserControl : [{0}]-[{1}]\n{2}", this.GetType(), this.Name, e.Message);
                    ExceptionLog.WriteLog(log);
                }
            }
        }
        #endregion
    }
}
