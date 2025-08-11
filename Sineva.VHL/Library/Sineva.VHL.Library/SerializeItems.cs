using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sineva.VHL.Library
{
    [Serializable]
    public class SerializeArrayObject<T>
    {
        private int m_Id;
        private T m_Object;

        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        public T Object
        {
            get { return m_Object; }
            set { m_Object = value; }
        }
    }
    [Serializable]
    public class SerializeArray_Int32
    {
        #region Field
        private int m_Id;
        private Int32 m_Value;
        #endregion

        #region Property
        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        public Int32 Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
        #endregion

        #region Constructor
        public SerializeArray_Int32()
        {
        }
        #endregion
    }
    [Serializable]
    public class SerializeArray_UInt32
    {
        #region Field
        private int m_Id;
        private UInt32 m_Value;
        #endregion

        #region Property
        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        public UInt32 Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
        #endregion

        #region Constructor
        public SerializeArray_UInt32()
        {
        }
        #endregion
    }
    [Serializable]
    public class SerializeArray_Single
    {
        #region Field
        private int m_Id;
        private Single m_Value;
        #endregion

        #region Property
        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        public Single Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
        #endregion

        #region Constructor
        public SerializeArray_Single()
        {
        }
        #endregion
    }
    [Serializable]
    public class SerializeArray_Double
    {
        #region Field
        private int m_Id;
        private Double m_Value;
        #endregion

        #region Property
        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        public Double Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
        #endregion

        #region Constructor
        public SerializeArray_Double()
        {
        }
        #endregion
    }
    [Serializable]
    public class SerializeArray_String
    {
        #region Field
        private int m_Id;
        private String m_Value;
        #endregion

        #region Property
        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        public String Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
        #endregion

        #region Constructor
        public SerializeArray_String()
        {
        }
        #endregion
    }
}
