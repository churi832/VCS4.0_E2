using Sineva.VHL.Library;
using Sineva.VHL.Library.Remoting;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Sineva.VHL.GUI.TouchPad
{
    [Serializable]
    public partial class NumSelectForm : Form
    {
        public delegate void VehicleNumChangeHandler();

        public static event VehicleNumChangeHandler VehicleNumChange;

        private bool first = true;

        public NumSelectForm()
        {
            InitializeComponent();

            IniFile.Instance.Initilize();
            Vehicle.Instance.Initilize();

            this.Load += NumSelectForm_Load;

            AddVehicleForm.AddVehicleOK += AddVehicleForm_AddVehicleOK;
        }

        private void AddVehicleForm_AddVehicleOK(object sender, VehicleManager vehicleManager)
        {
            Vehicle.Instance.AddVehicle(vehicleManager.VehicleNum, vehicleManager.Ip);
            listView1.Items.Add(new ListViewItem(new string[] { vehicleManager.VehicleNum }, -1, System.Drawing.SystemColors.Info, System.Drawing.Color.Empty, new System.Drawing.Font("微软雅黑", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)))));
            IniFile.IniData.Sections.AddSection(vehicleManager.VehicleNum);
            IniFile.IniData[vehicleManager.VehicleNum].AddKey("IP", vehicleManager.Ip);
            IniFile.Instance.Save();
        }

        private void NumSelectForm_Load(object sender, EventArgs e)
        {
            foreach (string s in Vehicle.Instance.VehicleMap.Keys)
            {
                ListAddVehicle(s, Vehicle.Instance.VehicleMap[s]);
            }
        }


        private void ListAddVehicle(string vehicleNum, string ip)
        {
            Vehicle.Instance.AddVehicle(vehicleNum, ip);

            listView1.Items.Add(new ListViewItem(new string[] {
            vehicleNum}, -1, System.Drawing.SystemColors.Info, System.Drawing.Color.Empty, new System.Drawing.Font("微软雅黑", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)))));

            IniFile.IniData.Sections.AddSection(vehicleNum);
            IniFile.IniData[vehicleNum].AddKey("IP", ip);
            IniFile.Instance.Save();
        }

        private void btnAddCehicle_Click(object sender, EventArgs e)
        {
            AddVehicleForm addVehicleForm = new AddVehicleForm();
            addVehicleForm.ShowDialog();
        }

        private void btnDeleteVehicle_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count <= 0)
            {
                CustomMessageBox customMessageBox = new CustomMessageBox("请先选择一个车辆！");
                customMessageBox.ShowDialog();
                return;
            }

            int index = 0;
            if (this.listView1.SelectedItems.Count > 0) // 判断listview是否有被选中项
            {
                index = this.listView1.SelectedItems[0].Index; // 取当前选中项的index
                string vehicleNum = this.listView1.Items[index].Text; // 获取被选中项的名称

                listView1.Items.Remove(listView1.Items[index]);

                IniFile.IniData.Sections.RemoveSection(vehicleNum);
                IniFile.Instance.Save();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count <= 0)
            {
                CustomMessageBox customMessageBox = new CustomMessageBox("请选择一辆天车！");
                customMessageBox.ShowDialog();
                return;
            }

            ListViewItem item = this.listView1.SelectedItems[0];
            string vewhicleNum = item.Text;
            string ip = Vehicle.Instance.GetVehicleIP(vewhicleNum);
            if (RemoteManager.PadInstance.Remoting != null)
            {
                RemoteManager.PadInstance.Remoting.RemoteGUI = null;
            }
            var rv = RemoteManager.PadInstance.Initialize(ConnectionMode.Client, CHANNEL_TYPE.TCP, ip);

            Program.SelectedNum = vewhicleNum;

            if (!first)
            {
                VehicleNumChange();
            }
            else
            {
                TaskHandler.Instance.InitTaskHandler();
            }

            first = false;
            this.Hide();
            var main = new MainForm();
            main.ShowDialog();
        }
    }
}
