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
    [Editor(typeof(UIEditorServoUnitTag), typeof(UITypeEditor))]
    public class ServoUnitTag
    {
        #region Fields
        private ServoUnit m_Servo = null;
        private bool m_ServoFindDone = false;
        private bool m_ServoFindNone = false;
        #endregion

        #region Properties
        public string ServoName { get; set; }
        #endregion

        #region Constructor
        public ServoUnitTag()
        {
        }
        public ServoUnitTag(string name)
        {
            ServoName = name;
        }
        #endregion

        #region Methods
        public ServoUnit GetServoUnit()
        {
            if (m_ServoFindNone) return null;
            if (m_ServoFindDone == false)
            {
                m_Servo = ServoManager.Instance.GetServoByName(ServoName);
                m_ServoFindDone = true;
                if (m_Servo == null && !m_ServoFindNone)
                {
                    m_ServoFindNone = true;
                    MessageBox.Show(string.Format("There is no Servo Name = {1} in the Servo Unit define", ServoName), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return m_Servo;
        }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(ServoName))
            {
                return base.ToString();
            }
            else
            {
                return string.Format("{0}", ServoName);
            }
        }
        #endregion
    }
}
