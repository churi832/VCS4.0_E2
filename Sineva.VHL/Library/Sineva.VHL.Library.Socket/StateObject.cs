using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sineva.VHL.Library.OcsSocket
{
    [Guid("79DA9F27-12BF-4A27-A3C3-252E35C8DC1B")]
    [ComVisible(true)]
    public class StateObject
    {
        public const int BufferSize = 4096;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder ReceivedString = new StringBuilder();
        public System.Net.Sockets.Socket WorkSocket = null;
    }
}
