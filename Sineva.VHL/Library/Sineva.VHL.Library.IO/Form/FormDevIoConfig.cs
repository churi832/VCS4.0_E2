using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;

namespace Sineva.VHL.Library.IO
{
    public partial class FormDevIoConfig : Form
    {
        private static Point m_FormDefaultLocation = new Point(0, 0);

        private IDevIoCollection m_OriginalCollection;//선택된 원본 IoCollection;
        private IDevIoCollection m_SelectedCollection = null; //UIEditor에 반환될 결과

        public IDevIoCollection SelectedCollection
        {
            get { return m_SelectedCollection; }
        }

        public FormDevIoConfig()
        {
            InitializeComponent();

        }

        public void Initialize(object collection)
        {
            Type objType = collection.GetType();

            m_OriginalCollection = collection as IDevIoCollection;
            m_SelectedCollection = m_OriginalCollection.CreateCollection() as IDevIoCollection;
            foreach(object obj in m_OriginalCollection)
            {
                m_SelectedCollection.Add(obj);
            }
            InitControl();
        }

        private void InitControl()
        {
            tbName.Text = m_OriginalCollection.Name;
            lblDevIoType.Text = string.Format("IO TYPE : {0}", m_OriginalCollection.DevIoType.ToString());

            listBox1.SuspendLayout();
            listBox1.Items.Clear();
            for(int i = 0; i < m_SelectedCollection.Count; i++)
            {
                _DeviceIo item = m_SelectedCollection.GetItem(i) as _DeviceIo;
                listBox1.Items.Add(item);
            }
            listBox1.ResumeLayout();

            this.viewIoEdit1.Initialize(IoManager.Instance);
        }

        private void FormDeviceSelect_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_FormDefaultLocation = this.Location;
        }

        private void FormDeviceSelect_Load(object sender, EventArgs e)
        {
            this.Location = m_FormDefaultLocation;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            List<string> names = this.viewIoEdit1.SelectedNames;

            listBox1.SuspendLayout();
            IoType devIoType = m_OriginalCollection.DevIoType;
            for(int i = 0; i < names.Count; i++)
            {
                IoChannel chNew = IoManager.Instance.GetChannelByName(devIoType, names[i]);
                if(chNew != null)
                {
                    bool exist = false;
                    foreach(_DeviceIo item in m_SelectedCollection)
                    {
                        IoChannel ch = item.GetIoChannel();
                        if(chNew.ToString() == ch.ToString()) exist = true;
                    }

                    if(exist == false)
                    {
                        _DeviceIo io = m_SelectedCollection.CreateNewItem();
                        io.SetIoChannel(chNew);
                        m_SelectedCollection.Add(io);
                        listBox1.Items.Add(io);
                    }
                }
            }
            listBox1.ClearSelected();
            listBox1.ResumeLayout();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach(var item in listBox1.SelectedItems)
            {
                m_SelectedCollection.Remove(item);
            }

            listBox1.Items.Clear();
            foreach(_DeviceIo item in m_SelectedCollection)
            {
                listBox1.Items.Add(item);
            }
            listBox1.ClearSelected();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {

        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {

        }
    }

    public class UIEditorDeviceIoSelect : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider sp, object value)
        {
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)sp.GetService(typeof(IWindowsFormsEditorService));

            if(edSvc == null)
            {   // uh oh.
                return value;
            }

            Type containerType = context.Instance.GetType();
            //string containerName = ((_Device)context.Instance).Name;
            _Device device = context.Instance as _Device;
            string containerName = (device != null) ? device.MyName : "";
            Type propertyType = context.PropertyDescriptor.PropertyType;
            string propertyName = context.PropertyDescriptor.DisplayName;

            //Type containedItemType = (value as GenericCollection).ContainedItemType;
            //FormObjectConfig ui = new FormObjectConfig();
            //ui.Initialize(containedItemType, key2);
            FormDevIoConfig ui = new FormDevIoConfig();
            ui.Text = containerName + " : " + propertyName;
            ui.Initialize(value);

            edSvc.ShowDialog(ui);

            if(ui.DialogResult == DialogResult.OK)
            {
                ICollection originalCollection = value as ICollection;
                originalCollection.Clear();

                ICollection selectedCollection = ui.SelectedCollection;
                foreach(object item in selectedCollection)
                {
                    originalCollection.Add(item);
                }
                //originalCollection.MaxSimulateCount = selectedCollection.MaxSimulateCount;
            }

            return value;
        }

        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }

    }
}
