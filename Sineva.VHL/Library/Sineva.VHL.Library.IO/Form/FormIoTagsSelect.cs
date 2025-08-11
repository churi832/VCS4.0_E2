using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sineva.VHL.Library.IO
{
	public partial class FormIoTagsSelect : Form
	{
		public List<IoTag> m_Tags;
		private List<IoTag> m_SelectedIoTags = new List<IoTag>();
		private static IoType m_IoType;
        private static string m_FilterText = string.Empty;
        private static bool m_FilterChecked = false;

		public FormIoTagsSelect(List<IoTag> Tags)
		{
			InitializeComponent();

			if (IoManager.Instance.Initialize())
			{
				this.comboBox1.Items.Add(IoType.DI);
				this.comboBox1.Items.Add(IoType.DO);
				this.comboBox1.Items.Add(IoType.AI);
				this.comboBox1.Items.Add(IoType.AO);
                this.comboBox1.Items.Add(IoType.AX);

                this.comboBox1.SelectedItem = m_IoType;

                this.textBox1.Text = m_FilterText;
                this.checkBox1.Checked = m_FilterChecked;
			}

			if (Tags != null)
            {
				foreach (IoTag tag in Tags) m_SelectedIoTags.Add(tag);
				this.listBox2.DataSource = m_SelectedIoTags;
			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.comboBox1.SelectedItem == null)
				return;

			m_IoType = (IoType)this.comboBox1.SelectedItem;
			switch (m_IoType)
			{
				case IoType.DI:
					this.listBox1.DataSource = IoManager.DiChannels;
					break;
				case IoType.DO:
					this.listBox1.DataSource = IoManager.DoChannels;
					break;
				case IoType.AI:
					this.listBox1.DataSource = IoManager.AiChannels;
					break;
				case IoType.AO:
					this.listBox1.DataSource = IoManager.AoChannels;
					break;
                case IoType.AX:
                    this.listBox1.DataSource = IoManager.AxChannels;
                    break;
            }
        }

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (this.checkBox1.Checked)
			{
				switch (m_IoType)
				{
					case IoType.DI:
						this.listBox1.DataSource = Filterling(IoManager.DiChannels, this.textBox1.Text, ' ', ',');
						break;
					case IoType.DO:
						this.listBox1.DataSource = Filterling(IoManager.DoChannels, this.textBox1.Text, ' ', ',');
						break;
					case IoType.AI:
						this.listBox1.DataSource = Filterling(IoManager.AiChannels, this.textBox1.Text, ' ', ',');
						break;
					case IoType.AO:
						this.listBox1.DataSource = Filterling(IoManager.AoChannels, this.textBox1.Text, ' ', ',');
						break;
                    case IoType.AX:
                        this.listBox1.DataSource = Filterling(IoManager.AxChannels, this.textBox1.Text, ' ', ',');
                        break;
                }
            }
			else
			{
				switch (m_IoType)
				{
					case IoType.DI:
						this.listBox1.DataSource = IoManager.DiChannels;
						break;
					case IoType.DO:
						this.listBox1.DataSource = IoManager.DoChannels;
						break;
					case IoType.AI:
						this.listBox1.DataSource = IoManager.AiChannels;
						break;
					case IoType.AO:
						this.listBox1.DataSource = IoManager.AoChannels;
						break;
                    case IoType.AX:
                        this.listBox1.DataSource = IoManager.AxChannels;
                        break;
                }
            }

            m_FilterChecked = this.checkBox1.Checked;
		}

		private List<IoChannel> Filterling(List<IoChannel> source, string input, params char[] separator)
		{
			List<IoChannel> list = new List<IoChannel>();
			string[] keys = input.Split(separator);
			if (keys.Length > 0)
			{
				foreach (IoChannel dev in source)
				{
					bool matchAll = true;
					foreach (string key in keys)
					{
						matchAll &= dev.Name.ToLower().Contains(key.ToLower());
					}
					if (matchAll)
					{
						list.Add(dev);
					}
				}
			}
			if (list.Count > 0)
				return list;
			else
				return source;
		}

		private void textBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				this.checkBox1.Checked = false;
				this.checkBox1.Checked = true;
			}
		}

        private void btnAdd_Click(object sender, EventArgs e)
        {
			int selected_counts = this.listBox1.SelectedItems.Count;
			if (selected_counts == 0) return;

			for (int i = 0; i < selected_counts; i++)
			{
				IoChannel channel = this.listBox1.SelectedItems[i] as IoChannel;
				IoTag tag = new IoTag();
				tag.Name = channel.Name;
				tag.IoType = channel.IoType;

				m_SelectedIoTags.Add(tag);
			}

			this.listBox2.DataSource = null;
			this.listBox2.Refresh();
			this.listBox2.DataSource = m_SelectedIoTags;
		}

		private void btnDelete_Click(object sender, EventArgs e)
        {
			int selected_counts = this.listBox2.SelectedItems.Count;
			if (selected_counts == 0) return;

			for (int i = 0; i < selected_counts; i++)
			{
				IoTag tag = this.listBox2.SelectedItems[i] as IoTag;
				m_SelectedIoTags.Remove(tag);
			}

			this.listBox2.DataSource = null;
			this.listBox2.Refresh();
			this.listBox2.DataSource = m_SelectedIoTags;
		}

        private void btnOk_Click(object sender, EventArgs e)
        {
			m_FilterText = textBox1.Text;

			m_Tags = new List<IoTag>();
			if (m_SelectedIoTags.Count > 0)
            {
				m_Tags.AddRange(m_SelectedIoTags);
            }
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Dispose();
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Dispose();
		}
	}
}
