using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sineva.VHL.Library;
using Sineva.VHL.Data;

namespace Sineva.VHL.Data.LogIn
{
	public partial class LogInForm : Form
	{
		enum LogInAction
		{
			None,
			LogIn,
			LogOut,
			PassChange,
			NewAccount,
			DeleteAccount,
			//Exit,
		}
		//127, 225
		private LogInAction m_Action = LogInAction.None;
        private int m_PermissionKey = -1;

		public LogInForm()
		{
			InitializeComponent();

            string message = string.Empty;
            if(AccountManager.Instance.SortedListAccount.Count == 0)
            {
                string id = "Default Admin";
                string pw = string.Empty;
                if(AccountManager.Instance.CreateAccount(id, pw, AuthorizationLevel.Administrator, out message))
                    MessageBox.Show(string.Format("Default Administrator Account is Created!!!"));
                else
                    MessageBox.Show(message);
            }
            this.listBox1.DataSource = AccountManager.Instance.StoredAccounts;

			this.lblNewPass.Visible = false;
			this.tbNewPass.Visible = false;

			this.lblConfirmPass.Visible = false;
			this.tbConfirmPass.Visible = false;
			
			this.btnDo.Visible = false;

            this.cbLevel.Items.AddRange(Enum.GetNames(typeof(UserLevels)));
            this.cbLevel.SelectedIndex = 0;
            this.cbLevel.Visible = false;
            this.lblUserLevel.Visible = false;

			if (AccountManager.Instance.IsLogIn)
			{
				ShowControl(true);
				this.btnLogIn.Visible = false;
                this.listBox1.Enabled = false;

                tbId.Text = AccountManager.Instance.CurAccount.UserName;

                this.listBox1.Enabled = false;
			}
			else
			{
				ShowControl(false);
				this.btnLogOut.Visible = false;
				//this.Size = new Size(this.Size.Width, 135);
			}

            m_PermissionKey = -1;
		}

        public LogInForm(int key)
            : this()
        {
            if(key >= 0)
            {
                ShowControl(false);
                this.btnLogOut.Visible = false;

                this.btnLogIn.Visible = true;
                this.listBox1.Enabled = true;

                m_PermissionKey = key;
            }
        }

		private void btnOk_Click(object sender, EventArgs e)
		{
            string message = string.Empty;
			if (AccountManager.Instance.LogIn(this.tbId.Text, this.tbPass.Text, out message))
            {
                if (m_PermissionKey >= 0)
                    EventHandlerManager.Instance.FireLoginPermissionRequest(m_PermissionKey, new string[] { this.tbId.Text, this.tbPass.Text });
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                Dispose();
            }
            else
            {
                MessageBox.Show(message);
            }
		}

		private void btnDo_Click(object sender, EventArgs e)
		{
            string message = string.Empty;

            string id = this.tbId.Text;
            string pw = this.tbPass.Text;
            string pwNew = this.tbNewPass.Text;
            string pwConfirm = this.tbConfirmPass.Text;

            switch(m_Action)
            {
            case LogInAction.NewAccount:
                {
                    if(cbLevel.SelectedItem == null)
                    {
                        MessageBox.Show("User Level is not Selected");
                        return;
                    }
                    if(cbLevel.SelectedIndex > (int)AccountManager.Instance.CurAccount.Level)
                    {
                        string msg = string.Format("{0} Authority Level User cannot create Account {1} Level User", AccountManager.Instance.CurAccount.Level.ToString(), cbLevel.SelectedItem.ToString());
                        MessageBox.Show(msg);
                        return;
                    }

                    AuthorizationLevel level = AuthorizationLevel.Operator;
                    if(!Enum.TryParse(cbLevel.SelectedItem.ToString(), out level))
                        level = AuthorizationLevel.Operator;
                    if(AccountManager.Instance.CreateAccount(id, pw, level, out message))
                    {
                        MessageBox.Show(message);
                    }
                    else
                    {
                        MessageBox.Show(message);
                    }                    
                }
                break;

            case LogInAction.DeleteAccount:
                {
                    AccountManager.Instance.DeleteAccount(this.tbId.Text, out message);
                    MessageBox.Show(message);
                }
                break;

            case LogInAction.PassChange:
                {
                    AccountManager.Instance.ChangePassword(this.tbId.Text, this.tbPass.Text, this.tbNewPass.Text, this.tbConfirmPass.Text, out message);
                    MessageBox.Show(message);
                }
                break;
            }

			this.listBox1.DataSource = AccountManager.Instance.StoredAccounts;
            //this.listBox1.Update();
            //this.listBox1.Refresh();
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox1.SelectedValue != null)
			{
				this.tbId.Text = this.listBox1.SelectedValue.ToString();
				this.tbPass.Text = "";
				this.tbNewPass.Text = "";
				this.tbConfirmPass.Text = "";
			}
		}

		private void linkLabelNewAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			m_Action = LogInAction.NewAccount;

			this.btnDo.Text = "New Account";
			this.btnDo.Visible = true;

            this.lblUserLevel.Visible = true;
            this.cbLevel.Visible = true;

			this.lblNewPass.Visible = false;
			this.tbNewPass.Visible = false;

			this.lblConfirmPass.Visible = false;
			this.tbConfirmPass.Visible = false;

            this.tbId.Text = "Type New ID";
            this.tbPass.Text = string.Empty;
		}

		private void linkLabelDeleteAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
            if(AccountManager.Instance.CurAccount.Level != AuthorizationLevel.Administrator)
                return;

			m_Action = LogInAction.DeleteAccount;

            this.listBox1.Enabled = true;

			this.btnDo.Text = "Delete";
			this.btnDo.Visible = true;

            this.lblUserLevel.Visible = false;
            this.cbLevel.Visible = false;

            this.lblNewPass.Visible = false;
			this.tbNewPass.Visible = false;

			this.lblConfirmPass.Visible = false;
			this.tbConfirmPass.Visible = false;
		}

		private void linkLabelChangePassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			m_Action = LogInAction.PassChange;

			this.btnDo.Text = "Change";
			this.btnDo.Visible = true;

            this.lblUserLevel.Visible = false;
            this.cbLevel.Visible = false;

			this.lblNewPass.Visible = true;
			this.tbNewPass.Visible = true;

			this.lblConfirmPass.Visible = true;
			this.tbConfirmPass.Visible = true;

            this.tbPass.Text = string.Empty;
		}


		private void ClearText()
		{
			this.tbId.Text = "";
			this.tbNewPass.Text = "";
			this.tbPass.Text = "";
			this.tbConfirmPass.Text = "";
		}

		private void ShowControl(bool show)
		{
            if(show == true)
            {
                this.linkLabelDeleteAccount.Visible = AccountManager.Instance.CurAccount.Level == AuthorizationLevel.Administrator;
                this.linkLabelNewAccount.Visible = AccountManager.Instance.CurAccount.Level < AuthorizationLevel.Maintenance;
                
                this.linkLabelChangePassword.Visible = show;
            }
            else
            {
                this.linkLabelChangePassword.Visible = show;
                this.linkLabelDeleteAccount.Visible = show;
                this.linkLabelNewAccount.Visible = show;

                this.btnDo.Text = "";
            }
		}

		private void btnLogIn_Click(object sender, EventArgs e)
		{
            string message = string.Empty;
			if (AccountManager.Instance.LogIn(this.tbId.Text, this.tbPass.Text, out message))
			{
				ShowControl(true);
				this.btnLogIn.Visible = false;
				this.btnLogOut.Visible = true;

                this.listBox1.Enabled = false;

                this.lblUserLevel.Visible = false;
                this.cbLevel.Visible = false;
                //this.Size = new Size(this.Size.Width, 225);

                if (m_PermissionKey > 0)
                {
                    // Recipe Change Permission : m_PermissionKey = 1
                    EventHandlerManager.Instance.FireLoginPermissionRequest(m_PermissionKey, new string[] { this.tbId.Text, this.tbPass.Text });
                }
            }
            else
            {
                MessageBox.Show(message);
            }
            this.Dispose();
		}

		private void btnLogOut_Click(object sender, EventArgs e)
		{
			AccountManager.Instance.LogOff();
            this.listBox1.Enabled = true;

			ShowControl(false);

            this.lblUserLevel.Visible = false;
            this.cbLevel.Visible = false;

			this.lblNewPass.Visible = false;
			this.tbNewPass.Visible = false;

			this.lblConfirmPass.Visible = false;
			this.tbConfirmPass.Visible = false;

			this.btnLogIn.Visible = true;
			this.btnLogOut.Visible = false;
			this.ClearText();
			this.btnDo.Visible = false;

			//this.Size = new Size(this.Size.Width, 135);
		}
	}
}
