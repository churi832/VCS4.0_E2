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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
	[Serializable()]
	public class ValidationTextBox : System.Windows.Forms.TextBox
	{
		#region Fields
		private KeyInValidation m_Validation = null;
		private KeyPadInfo m_KeyPadInfo = null;
		private bool m_UsedInKeyPad = false;
		private object m_RefTag = null;
		#endregion

		#region Properties
		[Category("HIT : Setting")]
		public OptionFormat DataFormat
		{
			get { return m_Validation.Format; }
			set { m_Validation.Format = value; }
		}
		[Category("HIT : Setting"), Browsable(false), XmlIgnore()]
		public bool LimitCheck
		{
			get
			{
				bool check = false;
				check |= !string.IsNullOrEmpty(m_Validation.High);
				check |= !string.IsNullOrEmpty(m_Validation.Low);

				return check;
			}
		}
		[Category("HIT : Setting")]
		public string LimitHigh
		{
			get { return m_Validation.High; }
			set { m_Validation.High = value; }
		}
		[Category("HIT : Setting")]
		public string LimitLow
		{
			get { return m_Validation.Low; }
			set { m_Validation.Low = value; }
		}
		[Category("HIT : Setting")]
		public KeyPadInfo KeyPadInfo
		{
			get { return m_KeyPadInfo; }
			set
			{
				m_KeyPadInfo = value;
				SetEventHandler();
			}
		}
		[Category("HIT : Setting"), XmlIgnore(), Browsable(false)]
		public bool UsedInKeyPad
		{
			get { return m_UsedInKeyPad; }
			set
			{
				m_UsedInKeyPad = value;
				SetEventHandler();
			}
		}
		[Category("HIT : Setting")]
		public object ReferenceTag
		{
			get { return m_RefTag; }
			set { m_RefTag = value; }
		}
		#endregion

		private EventHandler m_DelClickEvent = null;
		private KeyPressEventHandler m_DelKeyPressSkip = null;
		private KeyPressEventHandler m_DelKeyPress = null;
		private KeyEventHandler m_DelKeyDown = null;
		//jemoon : 090507 - TextChange 할때 range를 Check하다 보니 Key입력이 굉장히 불편하다.
		//그래서 Click event시 KeyPad Dialog를 띄우고 OK 눌렀을때 Range check 하도록 수정.
		//private EventHandler m_DelTextChanged = null;

		public ValidationTextBox()
		{
			// 숫자입력시 한글모드전환을 방지하기 위해 
			// 꼭 필요한 imemode 설정 : 사랑할꺼야~
			this.ImeMode = ImeMode.Off;

			m_Validation = new KeyInValidation();
			m_KeyPadInfo = new KeyPadInfo();

			m_DelClickEvent = new EventHandler(TextBoxClick);
			m_DelKeyPressSkip = new KeyPressEventHandler(TextBoxKeyPressSkip);
			m_DelKeyPress = new KeyPressEventHandler(TextBoxKeyPress);

			//m_DelTextChanged = new EventHandler(TextBoxTextChanged);
			m_DelKeyDown = new KeyEventHandler(TexBoxKeyDown);

			SetEventHandler();
		}

		private void SetEventHandler()
		{
			// jemoon : 중복방지를 위해서 등록된거 삭제하고 다시 등록
			this.Click -= m_DelClickEvent;
			this.KeyPress -= m_DelKeyPressSkip;
			this.KeyPress -= m_DelKeyPress;
			this.KeyDown -= m_DelKeyDown;
			//this.TextChanged -= m_DelTextChanged;

			//jemoon : 090507 - TextChange 할때 range를 Check하다 보니 Key입력이 굉장히 불편하다.
			//그래서 Click event시 KeyPad Dialog를 띄우고 OK 눌렀을때 Range check 하도록 한다.
			//Keypad에 포함된 경우가 아닌데, 키패드 지정이 안되면 기본 키패드로 지정한다.
			if (!m_UsedInKeyPad)
			{
				if (m_KeyPadInfo == null || m_KeyPadInfo.KeyPadType == null)
				{
					m_KeyPadInfo = new KeyPadInfo();
					m_KeyPadInfo.KeyPadType = typeof(KeyPadTextBox);
				}
			}

			if (!m_UsedInKeyPad &&
				m_KeyPadInfo != null &&
				m_KeyPadInfo.KeyPadType != null)
			{
				this.Click += m_DelClickEvent;
				this.KeyPress += m_DelKeyPressSkip;
			}
			else
			{
				this.KeyDown += m_DelKeyDown;
				//this.TextChanged += m_DelTextChanged;
				this.KeyPress += m_DelKeyPress;
			}
		}

		private void TextBoxClick(object sender, EventArgs e)
		{
			//jemoon : 090911 - ReadOnly 속성일 경우 수정 하지 못하게 처리
			if (this.ReadOnly) return;

			KeyPad keyPad = m_KeyPadInfo.CreateInstance();

			keyPad.Validation = m_Validation;
			keyPad.OldValue = this.Text;
			keyPad.NewValue = this.Text;
			keyPad.ShowDialog();

			if (keyPad.DialogResult == DialogResult.OK)
			{
				this.Text = keyPad.NewValue;
				SendKeys.Send("{END}");
			}
		}

		private void TextBoxKeyPressSkip(object sender, KeyPressEventArgs e)
		{
			e.Handled = true;
		}

		private void TextBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			if (false == m_Validation.CheckValidation(e.KeyChar, this.Text))
			{
				// jemoon : validation check fail 이면 key 입력 취소
				e.Handled = true;
			}
		}

		private void TexBoxKeyDown(object sender, KeyEventArgs e)
		{
			//키다운이 발생했는데,입력포맷이 String, None 아닌경우에
			//한글모드로 들어오면 ime모드를 바꿔주고 skip 해야 한다.
			if (m_Validation.Format == OptionFormat.String || m_Validation.Format == OptionFormat.None)
			{
				return;
			}
			else if (this.ImeMode == ImeMode.Hangul)
			{
				this.ImeMode = ImeMode.Off;
			}
		}

		private void TextBoxTextChanged(object sender, EventArgs e)
		{
			string checkedValue = m_Validation.CheckRange(this.Text);
			if (!string.Equals(this.Text, checkedValue))
			{
				this.Text = checkedValue;
				SendKeys.Send("{END}");
			}
		}

		public void SetValidationInfo(KeyInValidation validataion)
		{
			m_Validation = validataion;
		}
	}
}
