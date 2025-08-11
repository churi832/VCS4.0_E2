/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team HJYOU
 * Issue Date	: 23.01.16
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sineva.VHL.Library
{
    [Serializable]
    public class XSeqFunc
    {
        #region Fields
        private int m_SeqNo = 0;
        private int m_ReturnSeqNo = 0;
        private int m_AlarmId = 0;
        private UInt32 m_StartTicks = 0;
        private List<UInt32> m_ExtraStartTicks = new List<UInt32>();
        private string m_Name = "";
        private SortedList<int, string> m_SeqCaseMemoLists = new SortedList<int, string>();// case no, case 설명
        private XSeqCmd m_SeqCmd = new XSeqCmd();
        private XInitTag m_InitTag = new XInitTag();
        private bool m_SubSeqAlarmOccupied = false;
        private double m_TactTime = 0;
        private int m_TaskLayer = 0;
        private double m_ScanTime = 0;
        private bool m_SeqNoReset = false; //Sequence No 초기화 되지 않는 경우 발생
        #endregion

        #region Properties
        public string SeqName
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public int TaskLayer
        {
            get { return m_TaskLayer; }
            set { m_TaskLayer = value; }
        }
        [XmlIgnore]
        public enSeqState SeqState
        {
            get
            {
                if (scm.Fail || AlarmId > 0) return enSeqState.ALARM;
                else if (scm.Abort || scm.Pause) return enSeqState.STOP;
                else if (scm.Ing) return enSeqState.RUN;
                else return enSeqState.READY;
            }
        }
        [Browsable(false), XmlIgnore]
        public SortedList<int, string> SeqCaseMemoLists
        {
            get { return m_SeqCaseMemoLists; }
            set { m_SeqCaseMemoLists = value; }
        }
        [Browsable(false), XmlIgnore]
        public int SeqNo
        {
            get { return m_SeqNo; }
            set 
            { 
                if (m_SeqNoReset) m_SeqNoReset = false;
                m_SeqNo = value;
            }
        }
        [Browsable(false), XmlIgnore]
        public int ReturnSeqNo
        {
            get { return m_ReturnSeqNo; }
            set { m_ReturnSeqNo = value; }
        }
        [Browsable(false), XmlIgnore]
        public int AlarmId
        {
            get { return m_AlarmId; }
            set { m_AlarmId = value; }
        }
        [Browsable(false), XmlIgnore]
        public UInt32 StartTicks
        {
            get { return m_StartTicks; }
            set { m_StartTicks = value; }
        }
        [Browsable(false), XmlIgnore]
        public List<UInt32> StartTicksExtra
        {
            get { return m_ExtraStartTicks; }
            set { m_ExtraStartTicks = value; }
        }
        public XSeqCmd scm
        {
            get { return m_SeqCmd; }
            set { m_SeqCmd = value; }
        }
        [Browsable(false), XmlIgnore]
        public XInitTag InitTag
        {
            get { return m_InitTag; }
            set { m_InitTag = value; }
        }
        [Browsable(false), XmlIgnore]
        public bool SubSeqAlarmOccupied
        {
            get { return m_SubSeqAlarmOccupied; }
            set { m_SubSeqAlarmOccupied = value; }
        }
        public double TactTime
        {
            get { return m_TactTime; }
            set { m_TactTime = value; }
        }
        public double ScanTime
        {
            get { return m_ScanTime; }
            set { m_ScanTime = value; }
        }
        #endregion

        #region Constructor
        public XSeqFunc()
        {
        }
        #endregion

        #region Methods

        public override string ToString()
        {
            if (string.IsNullOrEmpty(m_Name))
            {
                return this.GetType().Name;
            }
            else
            {
                return m_Name;
            }
        }

        public virtual void InitSeq()
        {
            this.SeqNo = 0;
            m_SeqNoReset = true;
        }

        public virtual int Do()
        {
            int result = -1;
            int nSeqNo = this.SeqNo;

            switch (nSeqNo)
            {
                case 0:
                    break;
            }

            this.SeqNo = nSeqNo;

            return result;
        }
        public virtual int Do(object para1)
        {
            int result = -1;
            int nSeqNo = this.SeqNo;

            switch (nSeqNo)
            {
                case 0:
                    break;
            }

            this.SeqNo = nSeqNo;

            return result;
        }

        public virtual int Do(object para1, object para2)
        {
            int result = -1;
            int nSeqNo = this.SeqNo;

            switch (nSeqNo)
            {
                case 0:
                    break;
            }

            this.SeqNo = nSeqNo;

            return result;
        }

        public virtual int Do(object para1, object para2, object para3)
        {
            int result = -1;
            int nSeqNo = this.SeqNo;

            switch (nSeqNo)
            {
                case 0:
                    break;
            }

            this.SeqNo = nSeqNo;

            return result;
        }

        public virtual void SeqAbort()
        {
            this.SeqNo = 0;
            m_SeqNoReset = true;
        }
        //public double GetElapsedTicks()
        //{
        //    TimeSpan diff = DateTime.Now - StartTime;
        //    return diff.TotalMilliseconds;
        //}
        public UInt32 GetElapsedTicks()
        {
            return XFunc.GetTickCount() - m_StartTicks;
        }

        public void SetExtraStartTicks(int count)
        {
            for (int i = 0; i < count; i++)
            {
                m_ExtraStartTicks.Add(XFunc.GetTickCount());
            }
        }

        public UInt32 GetElapsedTicks(int id)
        {
            return XFunc.GetTickCount() - m_ExtraStartTicks[id];
        }
        #endregion
    }
}
