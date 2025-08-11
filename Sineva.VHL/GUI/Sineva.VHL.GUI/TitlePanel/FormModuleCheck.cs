using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sineva.VHL.Library;

namespace Sineva.VHL.GUI
{
    public partial class FormModuleCheck : Form
    {
        #region Field
        private readonly string _MessageNormal = "MODULE OK";
        private readonly string _MessageAbnormal = "MODULE ERR";
        private LabelLamp m_HeaderLamp = null;
        private SortedList<string, LabelLamp> m_MemberLamps = new SortedList<string, LabelLamp>();
        private bool m_InstanceCreated = false;
        #endregion

        #region Constructor
        public FormModuleCheck()
        {
            InitializeComponent();
            this.TopLevel = false;

            // Add Header
            m_HeaderLamp = new LabelLamp();
            m_HeaderLamp.DisplayText = _MessageAbnormal;
            m_HeaderLamp.SetStatus(false);
            m_HeaderLamp.Size = new Size(this.Width, 24);
            m_HeaderLamp.TextColor = Color.Black;
            m_HeaderLamp.Location = new Point(0, 0);
            m_HeaderLamp.Margin = new Padding(0);
            m_HeaderLamp.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(m_HeaderLamp);

            if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                m_HeaderLamp.MouseHover += HeaderLamp_MouseHover;
                m_HeaderLamp.MouseLeave += HeaderLamp_MouseLeave;
                this.Load += FormModuleCheck_Load;
            }
        }
        #endregion

        #region Event Handler
        private void HeaderLamp_MouseLeave(object sender, EventArgs e)
        {
            this.Size = m_HeaderLamp.Size;
        }

        private void HeaderLamp_MouseHover(object sender, EventArgs e)
        {
            int height = m_HeaderLamp.Height;
            for(int i = 0; i < m_MemberLamps.Values.Count; i++)
            {
                height += m_MemberLamps.Values[i].Height;
            }
            this.Size = new Size(m_HeaderLamp.Width, height);
        }
        private void FormModuleCheck_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.BringToFront();
        }
        #endregion

        #region Method
        public void CreateControl(Panel container)
        {
            if(m_InstanceCreated) return;

            m_HeaderLamp.TextColor = container.ForeColor;
            m_HeaderLamp.BackColor = container.BackColor;
            m_HeaderLamp.Size = this.Size = container.Size;

            int memberId = 0;
            foreach(KeyValuePair<string, LabelLamp> pair in m_MemberLamps)
            {
                pair.Value.TextColor = container.ForeColor;
                pair.Value.BackColor = container.BackColor;
                pair.Value.Size = container.Size;
                pair.Value.Location = new Point(0, ++memberId * m_HeaderLamp.Height);
            }

            // Form이 놓여질 TopLevel Form의 위치를 찾는다.
            
            Point location = new Point(container.Location.X, container.Location.Y);
            Control parent = container.Parent;
            while (parent != container.TopLevelControl)
            {
                location.X += parent.Location.X;
                location.Y += parent.Location.Y;
                parent = parent.Parent;
            }
            this.Location = location;
            this.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            parent.Controls.Add(this);
            this.Visible = true;
            
            m_InstanceCreated = true;
        }
        public bool AddModule(string name, LabelLamp.LampKind lampSelect)
        {
            if(m_MemberLamps.ContainsKey(name) == false)
            {
                LabelLamp newLamp = m_HeaderLamp.Clone();
                newLamp.DisplayText = name;
                newLamp.LampKindSelect = lampSelect;
                newLamp.SetStatus(false);
                newLamp.Margin = new Padding(0);
                newLamp.Size = m_HeaderLamp.Size;
                newLamp.Location = new Point(0, (m_MemberLamps.Keys.Count + 1) * m_HeaderLamp.Height);
                newLamp.BackColor = m_HeaderLamp.BackColor;
                newLamp.TextColor = m_HeaderLamp.TextColor;
                newLamp.BorderStyle = BorderStyle.FixedSingle;
                m_MemberLamps.Add(name, newLamp);

                this.Controls.Add(newLamp);
                return true;
            }
            return false;
        }
        public void SetStatus(string name, bool on)
        {
            if(m_MemberLamps.ContainsKey(name) && m_MemberLamps[name].GetStatus() != on)
                m_MemberLamps[name].SetStatus(on);
            else
                return;     // 쓸데없이 foreach 들어가지 않도록 상태변경 없으면 걍 넘겨버리자.

            bool allNormal = true;
            foreach(KeyValuePair<string, LabelLamp> pair in m_MemberLamps)
            {
                allNormal &= pair.Value.GetStatus();
            }
            if(allNormal && m_HeaderLamp.GetStatus() == false) { m_HeaderLamp.SetStatus(true); m_HeaderLamp.DisplayText = _MessageNormal; }
            else if(!allNormal && m_HeaderLamp.GetStatus() == true) { m_HeaderLamp.SetStatus(false); m_HeaderLamp.DisplayText = _MessageAbnormal; }
        }
        #endregion
    }
}
