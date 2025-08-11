using Sineva.VHL.Data;
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
    public partial class DlgOpCallSingleMessage : GeneralForm
    {
        private const int m_MaxSentenceLength = 40;
        private const int m_FormBoarderHeight = 36;
        private const int m_DefaultWordWrapLength = 3;
        private const int m_AdditionalWordWrapHeight = 22;
        private readonly Size m_FormSize = new Size(580, 160);

        private int m_FormHeight = 0;

        private bool m_OpCommandSelected1 = false;
        private bool m_OpCommandSelected2 = false;

        public DlgOpCallSingleMessage()
        {
            InitializeComponent();

            InitControl();
        }

        private void InitControl()
        {
            this.Size = new Size(m_FormSize.Width, m_FormSize.Height);
            this.lblMessage.Font = new System.Drawing.Font("Arial", 9, FontStyle.Bold);

            m_FormHeight = this.Size.Height + (string.IsNullOrEmpty(this.Text) ? 0 : m_FormBoarderHeight);     // FormBoarder이 보일 경우, FormBoarderSize를 더해준다.
        }

        private void btnBuzzerOff_MouseClick(object sender, MouseEventArgs e)
        {
            GV.OperatorCallBuzzerOn = false;
        }

        private void btnConfirm_MouseClick(object sender, MouseEventArgs e)
        {
            GV.OperatorCallSelect1 = m_OpCommandSelected1;
            GV.OperatorCallSelect2 = m_OpCommandSelected2;

            GV.OperatorCallBuzzerOn = false;
            GV.OperatorCallConfirm = true;
            this.Visible = false;
        }

        public new void Show()
        {
            throw new Exception("Use 'ShowMessage(string)' method to show me.");
        }
        public new void ShowDialog()
        {
            throw new Exception("Use 'ShowMessage(string)' method to show me.");
        }
        public string GetMessage()
        {
            return this.lblMessage.Text;
        }
        public void ShowMessage(string message, bool alert = false)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    GV.OperatorCallBuzzerOn = false;
                    this.Visible = false;
                    return;
                }

                // Show Message for Operator Call
                this.lblMessage.Text = message;
                OpCallLog.WriteLog(message);

                GV.OperatorCallBuzzerOn = true;
                btnBuzzerOff.Visible = true;


                // Resize Form by message Length
                string[] seperatedMsg = message.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                int length = seperatedMsg.Length;
                for (int i = 0; i < seperatedMsg.Length; i++)
                {
                    // 한 문장의 길이가 MaxLength를 초과하는 경우, WordWrap Count를 증가시킨다.
                    // 솔직히, 입력문자에 따라 Label에 보여지는 크기가 일정하지가 않아서, 딱 맞아들어가게 잡지는 못하겠고... 여유를 좀 더 주지 뭐
                    length += (int)(seperatedMsg[i].Length / (m_MaxSentenceLength + 1));
                }

                int extendHeight = 0;
                if (length >= m_DefaultWordWrapLength)
                {
                    extendHeight = (int)((length - m_DefaultWordWrapLength + 1) * m_AdditionalWordWrapHeight);
                }

                bool visibleSizeOver = false;
                Size resize = new Size(m_FormSize.Width, m_FormSize.Height + extendHeight);
                if (resize.Height > Screen.PrimaryScreen.Bounds.Height)
                {
                    resize.Height = (int)(Screen.PrimaryScreen.Bounds.Height * 0.7);
                    visibleSizeOver = true;
                }

                if (alert) this.lblMessage.BackColor = Color.Red;
                else this.lblMessage.BackColor = Color.PaleGreen;

                btnSelect1.Visible = false;
                btnSelect2.Visible = false;

                this.Size = resize;
                this.Visible = true;

                if (visibleSizeOver)
                {
                    MessageBox.Show("Operator Call Message is Too Long.\nMessage could not be shown partially.");
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        public void ShowMessageSilence(string message, bool alert = false)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    GV.OperatorCallBuzzerOn = false;
                    this.Visible = false;
                    return;
                }

                // Show Message for Operator Call
                this.lblMessage.Text = message;
                OpCallLog.WriteLog(message);

                GV.OperatorCallBuzzerOn = false;
                btnBuzzerOff.Visible = false;


                // Resize Form by message Length
                string[] seperatedMsg = message.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                int length = seperatedMsg.Length;
                for (int i = 0; i < seperatedMsg.Length; i++)
                {
                    // 한 문장의 길이가 MaxLength를 초과하는 경우, WordWrap Count를 증가시킨다.
                    // 솔직히, 입력문자에 따라 Label에 보여지는 크기가 일정하지가 않아서, 딱 맞아들어가게 잡지는 못하겠고... 여유를 좀 더 주지 뭐
                    length += (int)(seperatedMsg[i].Length / (m_MaxSentenceLength + 1));
                }

                int extendHeight = 0;
                if (length >= m_DefaultWordWrapLength)
                {
                    extendHeight = (int)((length - m_DefaultWordWrapLength + 1) * m_AdditionalWordWrapHeight);
                }

                bool visibleSizeOver = false;
                Size resize = new Size(m_FormSize.Width, m_FormSize.Height + extendHeight);
                if (resize.Height > Screen.PrimaryScreen.Bounds.Height)
                {
                    resize.Height = (int)(Screen.PrimaryScreen.Bounds.Height * 0.7);
                    visibleSizeOver = true;
                }

                if (alert) this.lblMessage.BackColor = Color.Red;
                else this.lblMessage.BackColor = Color.PaleGreen;

                btnSelect1.Visible = false;
                btnSelect2.Visible = false;

                this.Size = resize;
                this.Visible = true;

                if (visibleSizeOver)
                {
                    MessageBox.Show("Operator Call Message is Too Long.\nMessage could not be shown partially.");
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        public void ShowMessage(string message, string[] buttonText, bool alert = false)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    GV.OperatorCallBuzzerOn = false;
                    this.Visible = false;
                    return;
                }

                // Show Message for Operator Call
                this.lblMessage.Text = message;
                OpCallLog.WriteLog(message);

                GV.OperatorCallBuzzerOn = true;


                // Resize Form by message Length
                string[] seperatedMsg = message.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                int length = seperatedMsg.Length;
                for (int i = 0; i < seperatedMsg.Length; i++)
                {
                    // 한 문장의 길이가 MaxLength를 초과하는 경우, WordWrap Count를 증가시킨다.
                    // 솔직히, 입력문자에 따라 Label에 보여지는 크기가 일정하지가 않아서, 딱 맞아들어가게 잡지는 못하겠고... 여유를 좀 더 주지 뭐
                    length += (int)(seperatedMsg[i].Length / (m_MaxSentenceLength + 1));
                }

                int extendHeight = 0;
                if (length >= m_DefaultWordWrapLength)
                {
                    extendHeight = (int)((length - m_DefaultWordWrapLength + 1) * m_AdditionalWordWrapHeight);
                }

                bool visibleSizeOver = false;
                Size resize = new Size(m_FormSize.Width, m_FormSize.Height + extendHeight);
                if (resize.Height > Screen.PrimaryScreen.Bounds.Height)
                {
                    resize.Height = (int)(Screen.PrimaryScreen.Bounds.Height * 0.8);
                    visibleSizeOver = true;
                }

                if (alert) this.lblMessage.BackColor = Color.Red;
                else this.lblMessage.BackColor = Color.PaleGreen;

                if (buttonText.Length > 0)
                {
                    btnSelect1.Visible = true;
                    btnSelect1.Text = buttonText[0];
                }
                else btnSelect1.Visible = false;
                if (buttonText.Length > 1)
                {
                    btnSelect2.Visible = true;
                    btnSelect2.Text = buttonText[1];
                }
                else btnSelect2.Visible = false;
                btnConfirm.Enabled = false;

                this.Size = resize;
                this.Visible = true;

                if (visibleSizeOver)
                {
                    MessageBox.Show("Operator Call Message is Too Long.\nMessage could not be shown partially.");
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.WriteLog(ex.ToString());
            }
        }

        private void btnSelect1_MouseClick(object sender, MouseEventArgs e)
        {
            btnConfirm.Enabled = true;
            m_OpCommandSelected1 = true;
            m_OpCommandSelected2 = false;
        }

        private void btnSelect2_MouseClick(object sender, MouseEventArgs e)
        {
            btnConfirm.Enabled = true;
            m_OpCommandSelected1 = false;
            m_OpCommandSelected2 = true;
        }
    }
}
