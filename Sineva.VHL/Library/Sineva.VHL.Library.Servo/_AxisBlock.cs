/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V1.0
 * Programmer	: Software Group
 * Issue Date	: 23.02.20
 * Description	: 
 * 
 ****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Sineva.VHL.Library.Servo
{
    [Serializable]
	public abstract class _AxisBlock : IAxisBlockFactory
	{
		#region Fields
		protected List<_Axis> m_Axes = new List<_Axis>();
        protected ServoControlFamily m_ControlFamily = ServoControlFamily.YMp2100;
		private int m_BlockId = 0;
        private int m_StartAxisId = 0;
        protected static int m_SeqNo = 0;
		private string m_BlockStateMsg = string.Empty;
		#endregion

		#region Properties
        [Category("Axis Info....")]
        public List<_Axis> Axes
		{
			get { return m_Axes; }
			set { m_Axes = value; }
		}
        [Category("Axis Info....")]
        public int StartAxisId
        {
            get { return m_StartAxisId; }
            set { m_StartAxisId = value; }
        }
        [Category("Block Type")]
        public ServoControlFamily ControlFamily
        {
            get { return m_ControlFamily; }
            set { m_ControlFamily = value; }
        }
		[Category("Block Type")]
		public int BlockId
		{
			get { return m_BlockId; }
			set { m_BlockId = value; }
		}
		public string BlockStateMsg
		{
			get { return m_BlockStateMsg; }
			set { m_BlockStateMsg = value; }
		}
        #endregion

        #region Constructor
        #endregion

        #region Methods
        public _AxisBlock CreateObject()
		{
			_AxisBlock block = Activator.CreateInstance(this.GetType()) as _AxisBlock;
			block.BlockId = m_SeqNo++;
			return block;
		}

		public static List<_Axis> GetAxisTypes()
		{
			List<_Axis> axisTypes = new List<_Axis>();
			Assembly asm = Assembly.Load((typeof(ServoManager)).Namespace);
			Type[] types = asm.GetTypes();
			foreach (Type type in types)
			{
				if (XFunc.CheckTypeCompatibility(typeof(_Axis), type, Compatibility.Compatible))
				{
					if (type.IsAbstract == false)
					{
						_Axis axis = Activator.CreateInstance(type) as _Axis;
						axisTypes.Add(axis);
					}
				}
			}
			return axisTypes;
		}
		#endregion
	}
}
