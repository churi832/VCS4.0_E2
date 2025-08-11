using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.IO
{
    [Serializable]
    public class DevIoAnalogInput : _DeviceIo
    {
        #region Field
        #endregion

        #region Property
        #endregion

        #region Constructor
        public DevIoAnalogInput()
        {
            this.IoItem = new IoChannel(IoType.AI);
        }
        public DevIoAnalogInput(IoChannel item)
        {
            this.IoItem = item;
            this.MyName = item.Name;
            this.DeviceId = item.Id;
        }
        #endregion

        #region Override
        public override void SetIoChannel(IoChannel ch)
        {
            this.IoItem = ch;
        }
        public override IoChannel GetIoChannel()
        {
            return IoItem;
        }
        public override IoType GetIoType()
        {
            return IoItem.IoType;
        }
        public override string GetStateString()
        {
            return IoItem.State;
        }
        #endregion

        #region Method
        #endregion
    }
}
