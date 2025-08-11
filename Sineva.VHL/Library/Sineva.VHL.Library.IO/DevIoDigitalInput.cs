using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library.IO
{
    [Serializable]
    public class DevIoDigitalInput : _DeviceIo
    {
        #region Field
        #endregion

        #region Property
        #endregion

        #region Constructor
        public DevIoDigitalInput()
        {
            this.IoItem = new IoChannel(IoType.DI);
        }
        public DevIoDigitalInput(IoChannel item)
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
            return this.IoItem;
        }
        public override IoType GetIoType()
        {
            return this.IoItem.IoType;
        }
        public override string GetStateString()
        {
            return this.IoItem.State;
        }
        #endregion

        #region Method
        #endregion
    }
}
