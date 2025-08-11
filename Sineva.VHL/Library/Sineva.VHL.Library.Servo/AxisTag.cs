/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V1.0
 * Programmer	: Software Group
 * Issue Date	: 23.02.20
 * Description	: 
 * 
 ****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sineva.VHL.Library.Servo
{
    [Serializable]
    [Editor(typeof(UIEditorAxisTag), typeof(UITypeEditor))]
    public class AxisTag
    {
        #region Fields
        private _Axis m_Axis = null;
        private bool m_AxisFindDone = false;
        private bool m_AxisFindNone = false;
        #endregion

        #region Properties
        public string AxisName { get; set; }
        public int AxisId { get; set; }
        #endregion

        #region Constructor
        public AxisTag()
        {
        }

        public AxisTag(string name, int Id)
        {
            AxisName = name;
            AxisId = Id;
        }
        #endregion

        #region Methods
        public _Axis GetAxis()
        {
            if (m_AxisFindNone) return null;
            if (m_AxisFindDone == false)
            {
                m_Axis = ServoManager.Instance.GetAxisByName(AxisName);
                m_AxisFindDone = true;
                if (m_Axis == null && !m_AxisFindNone)
                {
                    m_AxisFindNone = true;
                    MessageBox.Show(string.Format("There is no 'Axis Id = {0}, Axis Name = {1}' in the Axis define", AxisId, AxisName), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (XFunc.IsRunTime())
                {
                    if (!m_AxisFindNone && AxisId != m_Axis.AxisId)
                    {
                        AxisId = m_Axis.AxisId;
                        EventHandlerManager.Instance.InvokeConfigErrorHappened(string.Format("There is difference 'Tag_Axis ID = {0}, Axis ID = {1}' in the AXIS {2}", AxisId, m_Axis.AxisId, AxisName));
                    }
                }
            }

            return m_Axis;
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
