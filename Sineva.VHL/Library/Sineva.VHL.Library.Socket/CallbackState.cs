namespace Sineva.VHL.Library.OcsSocket
{
    public class CallbackObject
    {
        #region Fields
        private ClientObject _Client;
        private byte[] _Stream;
        #endregion

        #region Properties
        public ClientObject Client
        {
            get { return _Client; }
            set { _Client = value; }
        }

        public byte[] Stream
        {
            get { return _Stream; }
            set { _Stream = value; }
        }
        #endregion

        #region Constructors
        public CallbackObject()
        {
        }
        #endregion

        #region Methods
        #endregion
    }
}
