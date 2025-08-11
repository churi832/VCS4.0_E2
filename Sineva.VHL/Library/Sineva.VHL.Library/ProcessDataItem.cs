/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.17 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System;

namespace Sineva.VHL.Library
{
    [Serializable]
    public class ProcessDataItem
    {
        #region Field
        private ProcessDataKind m_Kind = ProcessDataKind.NONE;
        private string m_Name = string.Empty;
        private string m_Value = string.Empty;
        private int m_SequenceNo = 0;
        private string m_DfsName = string.Empty;
        private int m_DfsSeqNo = 0;
        #endregion

        #region Property
        public ProcessDataKind Kind
        {
            get { return m_Kind; }
            set { m_Kind = value; }
        }
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public string Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
        public int SequenceNo
        {
            get { return m_SequenceNo; }
            set { m_SequenceNo = value; }
        }
        public string DfsName
        {
            get { return m_DfsName; }
            set { m_DfsName = value; }
        }
        public int DfsSeqNo
        {
            get { return m_DfsSeqNo; }
            set { m_DfsSeqNo = value; }
        }
        #endregion

        #region Constructor
        public ProcessDataItem()
        {
        }
        public ProcessDataItem(ProcessDataKind kind, string name, string value, int seqNo = 0)
        {
            m_Kind = kind;
            m_Name = name;
            m_Value = value;
            m_SequenceNo = seqNo;
            m_DfsName = string.Empty;
            m_DfsSeqNo = -1;
        }
        public ProcessDataItem(ProcessDataKind kind, string name, string value, string subject, int seqNo)
        {
            m_Kind = kind;
            m_Name = name;
            m_Value = value;
            m_DfsName = subject;
            m_DfsSeqNo = seqNo;
        }
        #endregion
    }
}
