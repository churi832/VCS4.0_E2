using System;
using System.Windows.Media;

namespace Sineva.VHL.Library.Common
{
    [Serializable()]
    public class SerializableColor
    {
        #region Fields
        private string _Name = string.Empty;
        private byte _A;
        private byte _R;
        private byte _G;
        private byte _B;
        #endregion

        #region Properties
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public byte A
        {
            get { return _A; }
            set { _A = value; }
        }
        public byte R
        {
            get { return _R; }
            set { _R = value; }
        }
        public byte G
        {
            get { return _G; }
            set { _G = value; }
        }
        public byte B
        {
            get { return _B; }
            set { _B = value; }
        }
        #endregion

        public SerializableColor(byte a, byte r, byte g, byte b)
        {
            _A = a;
            _R = r;
            _G = g;
            _B = b;
        }

        public SerializableColor(Color color)
            : this(color.A, color.R, color.G, color.B)
        {
        }

        public Color GetColor()
        {
            return Color.FromArgb(_A, _R, _G, _B);
        }

        public bool CompareWith(SerializableColor color)
        {
            try
            {
                bool result = true;

                result &= (_Name == color.Name);
                result &= (_A == color.A);
                result &= (_R == color.R);
                result &= (_G == color.G);
                result &= (_B == color.B);

                return result;
            }
            catch
            {
                return false;
            }
        }

        public static implicit operator SerializableColor(Color color)
        {
            return new SerializableColor(color);
        }

        public static implicit operator Color(SerializableColor colour)
        {
            return Color.FromArgb(colour._A, colour._R, colour._G, colour._B);
        }
    }
}
