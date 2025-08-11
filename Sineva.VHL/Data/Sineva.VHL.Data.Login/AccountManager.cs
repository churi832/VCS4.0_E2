/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V1.0
 * Programmer	: you hyoun jai
 * Date			: 
 * Description	: 
 * 
 ****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Sineva.VHL.Data;
using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Library;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Reflection.Emit;
using System.Security.Principal;

namespace Sineva.VHL.Data.LogIn
{
	public class AccountManager
	{
		public delegate void LogInEventHandler(bool logIn, DataItem_UserInfo account);

		#region Singleton
		public readonly static AccountManager Instance = new AccountManager();
		#endregion

		#region Fields
        private SortedList<string, DataItem_UserInfo> m_SortedListAccount = new SortedList<string, DataItem_UserInfo>();
        private DataItem_UserInfo m_CurAccount = null;
		private bool m_IsLogIn = false;
        private bool m_IsTimeout = false;
		#endregion

		#region Event
		public event LogInEventHandler AccountLogInOut;
		#endregion

		#region Properties
        [XmlIgnore()]
        public string[] StoredAccounts
		{
			get { return SortedListAccount.Keys.ToArray(); }
		}
        [Browsable(false), XmlIgnore()]
        public DataItem_UserInfo CurAccount
        {
            get { return m_CurAccount; }
        }

		[Browsable(false), XmlIgnore()]
		public bool IsLogIn
		{
			get { return m_IsLogIn; }
		}
        [Browsable(false), XmlIgnore()]
        public bool IsTimeout
        {
            get { return m_IsTimeout; }
        }

        [Browsable(false), XmlIgnore()]
        public SortedList<string, DataItem_UserInfo> SortedListAccount { get => m_SortedListAccount; set => m_SortedListAccount = value; }
        #endregion

        #region Constructor
        private AccountManager()
		{
		}
		#endregion

		#region Methods
		public bool Initialize()
		{
            bool initiated = true;
            SortedListAccount.Clear();
            foreach (KeyValuePair<string, DataItem_UserInfo> account in DatabaseHandler.Instance.DictionaryUserList)
            {
                if (!SortedListAccount.ContainsKey(account.Key))
                    SortedListAccount.Add(account.Key, account.Value);
            }

            return initiated;
		}

        public bool PasswordCheck(string id, string pass)
        {
            if(!SortedListAccount.ContainsKey(id)) return false;

            foreach(DataItem_UserInfo account in SortedListAccount.Values)
            {
                if(account.UserName == id)
                {
                    string encryptedPw = XFunc.PasswordEncrypt(id, pass);
                    return string.Equals(account.Password, encryptedPw);
                }
            }

            return false;
        }

        public AuthorizationLevel GetUserLevel(string id)
        {
            if(!SortedListAccount.ContainsKey(id)) return AuthorizationLevel.Operator;
            foreach(DataItem_UserInfo account in SortedListAccount.Values)
            {
                if(account.UserName == id)
                {
                    return account.Level;
                }
            }

            return AuthorizationLevel.Operator;
        }

        public bool LogIn(string id, string pass, out string msg)
        {
            msg = string.Empty;
            if(!SortedListAccount.ContainsKey(id))
            {
                msg = string.Format("Invalid Operatin : Account [{0}] is not exist", id);
                return false;
            }

            foreach(DataItem_UserInfo account in SortedListAccount.Values)
            {
                if(account.UserName == id)
                {
                    string accountPw = XFunc.PasswordEncrypt(account.Level.ToString(), account.Password);
                    string encryptedPw = XFunc.PasswordEncrypt(id, pass);
                    if(string.Equals(accountPw, encryptedPw) == false)
                    {
                        msg = "Invalid Operation : Password is not match";
                        return false;
                    }

                    m_CurAccount = account;
                    if(AccountLogInOut != null)
                    {
                        this.AccountLogInOut(true, account);
                    }
                    m_IsLogIn = true;

                    // LogIn / LogOut : : m_PermissionKey = -1 (LogIn = 0 / LogOut = -1)
                    int permission_key = 0;
                    EventHandlerManager.Instance.FireLoginPermissionRequest(permission_key, new string[] { account.UserName, pass });

                    return true;
                }
            }

            m_IsLogIn = false;
            return false;
        }

		public void LogOff()
		{
			this.m_CurAccount = null;
			this.m_IsLogIn = false;

			if (AccountLogInOut != null)
			{
				this.AccountLogInOut(false, m_CurAccount);
            }

            // LogIn / LogOut : : m_PermissionKey = -1 (LogIn = 0 / LogOut = -1)
            int permission_key = -1;
            EventHandlerManager.Instance.FireLoginPermissionRequest(permission_key, new string[] { "", "" });
        }
        public void LogOff(bool timeout = false)
        {
            this.m_CurAccount = null;
            this.m_IsLogIn = false;
            this.m_IsTimeout = timeout;
            if (AccountLogInOut != null)
            {
                this.AccountLogInOut(false, m_CurAccount);
            }

            // LogIn / LogOut : : m_PermissionKey = -1 (LogIn = 0 / LogOut = -1)
            int permission_key = -10;
            EventHandlerManager.Instance.FireLoginPermissionRequest(permission_key, new string[] { "", "" });
        }
        public bool CreateAccount(string id, string pass, AuthorizationLevel level, out string msg)
        {
            msg = string.Empty;
            if(SortedListAccount.ContainsKey(id))
            {
                msg = "New Account is exist, Already";
                return false;
            }

            string encryptedPw = XFunc.PasswordEncrypt(id, pass);
            DataItem_UserInfo newUserInfo = new DataItem_UserInfo(id, encryptedPw, "AMHS", "NewAccount", level);
            SortedListAccount.Add(id, newUserInfo);
            msg = string.Format("New Account [{0}/{1}] is created", level.ToString(), id);

            // Update Database ///////////////////////////////
            if (!DatabaseHandler.Instance.DictionaryUserList.ContainsKey(newUserInfo.UserName))
            {
                DatabaseHandler.Instance.DictionaryUserList.Add(newUserInfo.UserName, newUserInfo);
                DatabaseHandler.Instance.QueryUserList.Insert(newUserInfo);
            }
            ////////////////////////////////////////////////// 
            return true;
        }
        public bool DeleteAccount(string id, out string msg)
        {
            msg = string.Empty;
            if(m_CurAccount == null)
            {
                msg = "Invalid Operation : Current User is Unknown";
                return false;
            }
            if(!SortedListAccount.ContainsKey(id))
            {
                msg = "Invalid Operation : Remove Account is not Found";
                return false;
            }
            if(m_CurAccount.UserName == id)
            {
                msg = "Invalid Operation : Current Login Account cannot be removed";
                return false;
            }
            if(m_CurAccount.Level < AuthorizationLevel.Maintenance)
            {
                msg = "Invalid Operation : Current Login Account does not have authority";
                return false;
            }

            bool removeOk = false;
            foreach(KeyValuePair<string, DataItem_UserInfo> pair in SortedListAccount)
            {
                if(pair.Key == id)
                {
                    if(pair.Value.Level > m_CurAccount.Level)
                    {
                        msg = "Invalid Operation : Higher Authority Account cannot be removed";

                        return false;
                    }

                    removeOk = true;
                    SortedListAccount.Remove(pair.Key);
                    break;
                }
            }

            if(removeOk)
            {
                // Update Database ///////////////////////////////
                foreach (KeyValuePair<string, DataItem_UserInfo> account in DatabaseHandler.Instance.DictionaryUserList)
                {
                    if (account.Key == id)
                    {
                        DatabaseHandler.Instance.DictionaryUserList.Remove(account.Key);
                        DatabaseHandler.Instance.QueryUserList.Delete(account.Key);
                    }
                }
                ////////////////////////////////////////////////// 
            }
            else
            {
                msg = string.Format("Operatin Result : Account [{0}] is not removed", id);
            }
            return removeOk;
        }
        public bool ChangePassword(string id, string curPass, string newPass, string newPassConfirm, out string msg)
        {
            msg = string.Empty;
            if(string.Equals(newPass, newPassConfirm) == false)
            {
                msg = "Invalid Operation : New Password is not Confirmed";
                return false;
            }
            if(!SortedListAccount.ContainsKey(id))
            {
                msg = "Invalid Operation : Account is not Found";
                return false;
            }

            bool changeOk = false;
            foreach (KeyValuePair<string, DataItem_UserInfo> account in SortedListAccount)
            {
                if(account.Key == id)
                {
                    string encryptedPw = XFunc.PasswordEncrypt(id, curPass);
                    if(!string.Equals(account.Value.Password, encryptedPw))
                    {
                        msg = "Invalid Operation : Current Password Mismatch";
                        return false;
                    }

                    account.Value.Password = XFunc.PasswordEncrypt(id, newPass);
                    changeOk = true;
                    break;
                }
            }

            if(changeOk)
            {
                msg = "Operation Result : Password is changed";
                // Update Database ///////////////////////////////
                if (DatabaseHandler.Instance.DictionaryUserList.ContainsKey(id))
                {
                    DatabaseHandler.Instance.DictionaryUserList[id].Password = newPass;
                    DatabaseHandler.Instance.QueryUserList.Update(DatabaseHandler.Instance.DictionaryUserList[id]);
                }
                ////////////////////////////////////////////////// 
            }
            else
            {
                msg = "Operation Result : Password is not changed";
            }
            return changeOk;
        }
        public bool ChangeAccount(DataItem_UserInfo account)
        {
            if (account != null && SortedListAccount.ContainsKey(account.UserName))
            {
                m_CurAccount = account;
                if (AccountLogInOut != null) this.AccountLogInOut(true, account);
                m_IsLogIn = true;
            }
            return m_IsLogIn;
        }
        #endregion
	}
}
