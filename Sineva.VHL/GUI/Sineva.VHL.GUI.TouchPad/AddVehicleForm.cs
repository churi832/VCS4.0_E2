using System;
using System.Windows.Forms;

namespace Sineva.VHL.GUI.TouchPad
{
    public partial class AddVehicleForm : Form
    {
        //声明一个更新Address的委托
        public delegate void AddVehicleHandler(object sender, VehicleManager vehicleManager);
        //声明一个更新Address的事件
        public static event AddVehicleHandler AddVehicleOK;
        private string oldUsrname = "";


        public AddVehicleForm()
        {
            InitializeComponent();
        }

        private void btnUserInfoOK_Click(object sender, EventArgs e)
        {
            VehicleManager vehicleManager = new VehicleManager();
            vehicleManager.VehicleNum = this.txtUsername.Text;
            vehicleManager.Ip = this.txtUserIP.Text;

            AddVehicleOK(this, vehicleManager);
            this.Close();
        }

        private void btnUserInfoCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtEnter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnUserInfoOK_Click((object)sender, e);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void AddVehicleForm_Load(object sender, EventArgs e)
        {

        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;

//namespace DelegatePassValue
//{
//    public partial class frmAddress : Form
//    {
//        //声明一个更新Address的委托
//        public delegate void AddressUpdateHandler(object sender, AddressUpdateEventArgs e);
//        //声明一个更新Address的事件
//        public event AddressUpdateHandler AddressUpdated;

//        public frmAddress()
//        {
//            InitializeComponent();
//        }

//        private void btnOk_Click(object sender, EventArgs e)
//        {
//            var args = new AddressUpdateEventArgs(txtCountry.Text, txtState.Text, txtCity.Text, txtZipCode.Text);
//            AddressUpdated(this, args);
//            this.Dispose();
//        }

//        private void btnCancel_Click(object sender, EventArgs e)
//        {
//            this.Dispose();
//        }
//    }

//    public class AddressUpdateEventArgs : System.EventArgs
//    {
//        private string mCountry;
//        private string mState;
//        private string mCity;
//        private string mZipCode;
//        public AddressUpdateEventArgs(string sCountry, string sState, string sCity, string sZipCode)
//        {
//            this.mCountry = sCountry;
//            this.mState = sState;
//            this.mCity = sCity;
//            this.mZipCode = sZipCode;
//        }
//        public string Country { get { return mCountry; } }
//        public string State { get { return mState; } }
//        public string City { get { return mCity; } }
//        public string ZipCode { get { return mZipCode; } }
//    }
//}

//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;

//namespace DelegatePassValue
//{
//    public partial class frmMain : Form
//    {
//        public frmMain()
//        {
//            InitializeComponent();
//        }

//        private void btnSetAddress_Click(object sender, EventArgs e)
//        {
//            var frmAddr = new frmAddress();
//            frmAddr.AddressUpdated += new frmAddress.AddressUpdateHandler(AddressForm_ButtonClicked);
//            frmAddr.Show();
//        }

//        private void AddressForm_ButtonClicked(object sender, AddressUpdateEventArgs e)
//        {
//            txtCountry.Text = e.Country;
//            txtState.Text = e.State;
//            txtCity.Text = e.City;
//            txtZipCode.Text = e.ZipCode;
//        }

//        private void btnClose_Click(object sender, EventArgs e)
//        {
//            Application.Exit();
//        }
//    }
//}