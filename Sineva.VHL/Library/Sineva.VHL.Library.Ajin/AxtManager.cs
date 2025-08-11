using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sineva.VHL.Library.Servo;

namespace Sineva.VHL.Library.Ajin
{
    [Serializable]
    public class AxtManager
    {
        public static readonly AxtManager instance = new AxtManager();

        #region Fields
        private List<AxtAxisCtrl> m_AxisCtrls = new List<AxtAxisCtrl>();
        #endregion

        #region Properties
        #endregion

        #region Constructor
        private AxtManager()
        {

        }
        #endregion

        #region [Methods]
        public bool Initialize()
        {
            foreach(_AxisBlock block in ServoManager.Instance.AxisBlocks)
            {
                if(block.ControlFamily == ServoControlFamily.AXT)
                {
                    ( block as AxisBlockAXT ).Initialize();
                    m_AxisCtrls.Add( new AxtAxisCtrl( block as AxisBlockAXT ) );
                }
            }
            bool rv = true;

            foreach(AxtAxisCtrl ctrl in m_AxisCtrls)
            {
                rv &= ctrl.Initialize();
            }

            return rv;
        }
        #endregion
    }
}
