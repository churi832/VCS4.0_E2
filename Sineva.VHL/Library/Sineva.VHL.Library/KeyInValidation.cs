using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sineva.VHL.Library
{
	[Serializable()]
	public class KeyInValidation
	{
		#region Fields
		public string High = "";
		public string Low = "";
		public OptionFormat Format = OptionFormat.None;
		#endregion

		#region Properties
		public bool LimitCheckUse
		{
			get
			{
				bool check = false;
				check |= !string.IsNullOrEmpty(High);
				check |= !string.IsNullOrEmpty(Low);

				return check;
			}
		}
		#endregion

		#region Methods
		public bool CheckValidation(char ch, string currentValue)
		{
			// jemoon : Format 별로 유효성 검사 조건 구현 필요함
			bool ok = true;
			try
			{
				switch (this.Format)
				{
					case OptionFormat.Digit:
						{
							if (!(char.IsDigit(ch) || char.IsControl(ch)))
							{
								ok = false;
							}
						}
						break;
					case OptionFormat.Float:
						{
							if (!(char.IsDigit(ch) || char.IsControl(ch) || (ch == '.')))
							{
								ok = false;
								break;
							}
							else if ((ch == '.'))
							{
								if (currentValue != null)
								{
									if (currentValue.Contains("."))
									{
										ok = false;
									}
								}
							}
						}
						break;
					case OptionFormat.IpAddress:
						{
							if (!(char.IsDigit(ch) || char.IsControl(ch) || (ch == '.')))
							{
								ok = false;
							}
							else if ((ch == '.'))
							{
								char[] token = { '.' };
								string[] temp = currentValue.Split(token);

								if (temp == null || temp.Length >= 4)
								{
									ok = false;
								}
							}
						}
						break;
				}
			}
			catch (Exception err)   //Don't Use XFunc.ExceptionHandler.Add(err);
			{
				MessageBox.Show(err.ToString());
				ExceptionLog.WriteLog(err.ToString());
			}

			return ok;
		}

		public string CheckRange(string value)
		{
			try
			{
				if (this.LimitCheckUse)
				{
					// jemoon : Format 별로 유효성 검사 조건 구현 필요함
					switch (this.Format)
					{
						case OptionFormat.Digit:
							{
								if (string.IsNullOrEmpty(value)) value = "0";

								if (!string.IsNullOrEmpty(this.High))
								{
									int high = Convert.ToInt32(this.High);
									int curVal = Convert.ToInt32(value);
									if (curVal > high)
									{
										value = this.High;
									}
								}
								if (!string.IsNullOrEmpty(this.Low))
								{
									int low = Convert.ToInt32(this.Low);
									int curVal = Convert.ToInt32(value);
									if (curVal < low)
									{
										value = this.Low;
									}
								}

								value = Convert.ToInt32(value).ToString();
							}
							break;
						case OptionFormat.Float:
							{
								if (string.IsNullOrEmpty(value)) value = "0.0";
								if (value == ".") value = "0.0";

								if (!string.IsNullOrEmpty(this.High))
								{
									float high = Convert.ToSingle(this.High);
									float curVal = Convert.ToSingle(value);
									if (curVal > high)
									{
										value = this.High;
									}
								}
								if (!string.IsNullOrEmpty(this.Low))
								{
									float low = Convert.ToSingle(this.Low);
									float curVal = Convert.ToSingle(value);
									if (curVal < low)
									{
										value = this.Low;
									}
								}

								if (!value.EndsWith("."))
								{
									value = Convert.ToSingle(value).ToString();
								}
							}
							break;
					}
				}

				if (this.Format == OptionFormat.IpAddress)
				{
					char[] token = { '.' };
					string[] splitStrings = value.Split(token);

					if (splitStrings.Length == 1 && string.IsNullOrEmpty(splitStrings[0]))
					{
						value = "127.0.0.1";
					}
					else
					{
						int count = splitStrings.Length;
						value = "";
						for (int i = 0; i < count; i++)
						{
							if ((splitStrings[i] == "") && (i != count - 1))
							{
								splitStrings[i] = "0";
							}
							else if ((splitStrings[i] != ""))
							{
								int n = Convert.ToInt32(splitStrings[i]);
								if (n > 256) n = 255;
								{
									splitStrings[i] = n.ToString();
								}
							}

							value += splitStrings[i];
							if (i < count - 1) value += ".";
						}
					}
				}
			}
			catch (Exception err)   //Don't Use XFunc.ExceptionHandler.Add(err);
			{
				MessageBox.Show(err.ToString());
				ExceptionLog.WriteLog(err.ToString());
			}

			return value;
		}

		private string[] GetOptionListByOptionFormat()
		{
			return OptionFormatHelper.GetOptionListByOptionFormat(this.Format);
		}

		public string ShowEditDialog(string caption, string curValue)
		{
			string newValue = "";
			string[] values = GetOptionListByOptionFormat();

			if (Format == OptionFormat.IpAddress)
			{
				KeyPadIpAddressBox dlg = new KeyPadIpAddressBox(curValue);
				dlg.Caption = caption;
				dlg.Validation = this;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					newValue = dlg.NewValue;
				}
				else newValue = curValue;

				// KSS 110609
				dlg.Dispose();
			}
			else if (values == null)
			{
				KeyPadTextBox dlg = new KeyPadTextBox(curValue);
				dlg.Caption = caption;
				dlg.Validation = this;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					newValue = dlg.NewValue;
				}
				else newValue = curValue;

				// KSS 110609
				dlg.Dispose();
			}
			else
			{
				DlgSelectValue dlg = new DlgSelectValue(curValue, values);
				dlg.Caption = caption;
				dlg.ItemSelectMode = DlgSelectValue.SelectMode.ItemList;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					newValue = dlg.SelectedValue;
				}
				else newValue = curValue;

				// KSS 110609
				dlg.Dispose();
			}

			return newValue;
		}
		#endregion
	}
}
