using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sineva.VHL.Library;

namespace Sineva.VHL.Device.ServoControl
{
    [Serializable()]
    [Editor(typeof(UIEditorDevAxisTag), typeof(UITypeEditor))]
    public class DevAxisTag
    {
        #region Fields
        private _DevAxis m_DevAxis = null;
        private bool m_AxisFindDone = false;
        private bool m_AxisFindNone = false;
        private bool m_IsValid = true;
        #endregion

        #region Properties
        public string AxisName { get; set; }
        public int AxisId { get; set; }
        [Category("!Device IsValid")]
        public bool IsValid
        {
            get { return m_IsValid; }
            set { m_IsValid = value; }
        }
        #endregion

        #region Constructor
        public DevAxisTag()
        {
            m_IsValid = false;
        }

        public DevAxisTag(string name, int Id)
        {
            AxisName = name;
            AxisId = Id;
            m_IsValid = true;
        }
        #endregion

        #region Methods
        public _DevAxis GetDevAxis()
        {
            if (m_AxisFindNone || m_IsValid == false || AxisName == null) return null;
            if (m_AxisFindDone == false)
            {
                m_DevAxis = ServoControlManager.Instance.GetDevAxisByName(AxisName);
                m_AxisFindDone = true;
                if (m_DevAxis == null && !m_AxisFindNone)
                {
                    m_AxisFindNone = true;
                    //MessageBox.Show(string.Format("There is no 'Dev_Axis Id = {0}, Dev_Axis Name = {1}' in the AXIS define", AxisId, AxisName), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    EventHandlerManager.Instance.InvokeConfigErrorHappened(string.Format("There is no 'Dev_Axis Id = {0}, Dev_Axis Name = {1}' in the AXIS define", AxisId, AxisName));
                }
                if (m_DevAxis != null)
                {
                    if (AxisId != m_DevAxis.GetAxis().AxisId)
                    {
                        EventHandlerManager.Instance.InvokeConfigErrorHappened(string.Format("There is difference 'DevAxisTag ID = {0}, Axis ID = {1}' in the AXIS {2}", AxisId, m_DevAxis.GetAxis().AxisId, AxisName));
                        AxisId = m_DevAxis.GetAxis().AxisId;
                    }
                }
            }

            return m_DevAxis;
        }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(AxisName))
            {
                return base.ToString();
            }
            else
            {
                return string.Format("{0}.Axis{1}", AxisName, AxisId);
            }
        }
        #endregion
    }
}
