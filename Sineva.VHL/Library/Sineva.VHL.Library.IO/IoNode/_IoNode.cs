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


namespace Sineva.VHL.Library.IO
{
	public enum IoBusType
	{
		NotDefine,
		MP2100,
		BnR,
		CrevisModbus,
		Mxp,
	}

	[Serializable()]
	public abstract class _IoNode : INodeFactory
	{
        #region Fields
        protected IoBusType m_IoNodeBusType = IoBusType.NotDefine;
        protected int m_Id;
		protected List<_IoTerminal> m_Terminals = new List<_IoTerminal>();
		protected string m_Name = "";
		#endregion

		#region Properties
		public IoBusType IoNodeBusType
		{
			get { return m_IoNodeBusType; }
		}
		protected int Id
		{
			get { return m_Id; }
			set { m_Id = value; }
		}

		public List<_IoTerminal> Terminals
		{
			get { return m_Terminals; }
			set { m_Terminals = value; }
		}
		#endregion
		
		//protected IoPart m_NodeCoupler = null;
		//protected IoPart m_NodeEndModule = null;

		#region Constructor
		public _IoNode()
		{
		}
		#endregion

		#region Methods
		public List<_IoTerminal> GetTerminals()
		{
			return m_Terminals;
		}

		public static List<_IoTerminal> GetTerminalTypes()
		{
			List<_IoTerminal> terminalTypes = new List<_IoTerminal>();
			Assembly asm = Assembly.Load((typeof(IoManager)).Namespace);
			Type[] types = asm.GetTypes();
			foreach (Type type in types)
			{
				if (XFunc.CheckTypeCompatibility(typeof(_IoTerminal), type, Compatibility.Compatible))
				{
					if (type.IsAbstract == false)
					{
						_IoTerminal terminal = Activator.CreateInstance(type) as _IoTerminal;
						terminalTypes.Add(terminal);
					}
				}
			}
			return terminalTypes;
		}
		public static List<_IoTerminal> GetTerminalTypes(IoBusType busType)
		{
			List<_IoTerminal> terminalTypes = new List<_IoTerminal>();
			Assembly asm = Assembly.Load((typeof(IoManager)).Namespace);
			Type[] types = asm.GetTypes();
			foreach(Type type in types)
			{
				if(XFunc.CheckTypeCompatibility(typeof(_IoTerminal), type, Compatibility.Compatible))
				{
					if(type.IsAbstract == false)
					{
						_IoTerminal terminal = Activator.CreateInstance(type) as _IoTerminal;
						if(busType == IoBusType.NotDefine || terminal.IoTermBusType == busType)
							terminalTypes.Add(terminal);
					}
				}
			}
			return terminalTypes;
		}
		#endregion

		public override string ToString()
		{
			if (string.IsNullOrEmpty(m_Name))
			{
				return this.GetType().ToString();
			}
			else
			{
				return m_Name;
			}
		}

		#region INodeFactory 멤버

		public _IoNode CreateObject()
		{
			return Activator.CreateInstance(this.GetType()) as _IoNode;
		}

		#endregion
	}
}
