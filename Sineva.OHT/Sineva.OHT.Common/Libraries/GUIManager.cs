using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace Sineva.OHT.Common
{
    public class GUIManager : Singleton<GUIManager>
    {
        #region Fields
        private List<int> _ActivatedDialog = new List<int>();
        #endregion

        #region Properties
        #endregion

        #region Events
        #endregion

        #region Constructors
        private GUIManager()
        {
        }
        #endregion

        #region Methods
        public Control[] GetRecursiveControls(Control container)
        {
            try
            {
                List<Control> controls = new List<Control>();

                foreach (Control control in container.Controls)
                {
                    controls.Add(control);
                    if (control.Controls.Count > 0)
                    {
                        controls.AddRange(GetRecursiveControls(control));
                    }
                }

                return controls.ToArray();
            }
            catch
            {
                return null;
            }
        }
        public ToolStripMenuItem[] GetRecursiveMenuItems(MenuStrip menu)
        {
            try
            {
                List<ToolStripMenuItem> menuItems = new List<ToolStripMenuItem>();

                foreach (ToolStripMenuItem subMenu in menu.Items)
                {
                    menuItems.Add(subMenu);

                    if (subMenu.DropDownItems.Count > 0)
                    {
                        menuItems.AddRange(GetRecursiveToolStripMenuItem(subMenu));
                    }
                }

                return menuItems.ToArray();
            }
            catch
            {
                return new List<ToolStripMenuItem>().ToArray();
            }
        }
        public ToolStripMenuItem[] GetRecursiveToolStripMenuItem(ToolStripMenuItem menuItem)
        {
            try
            {
                List<ToolStripMenuItem> menuItems = new List<ToolStripMenuItem>();

                foreach (ToolStripItem subItem in menuItem.DropDownItems)
                {
                    if (subItem.GetType() != typeof(ToolStripMenuItem)) continue;

                    menuItems.Add(subItem as ToolStripMenuItem);

                    if ((subItem as ToolStripMenuItem).DropDownItems.Count > 0)
                    {
                        menuItems.AddRange(GetRecursiveToolStripMenuItem(subItem as ToolStripMenuItem));
                    }
                }

                return menuItems.ToArray();
            }
            catch
            {
                return new List<ToolStripMenuItem>().ToArray();
            }
        }
        public System.Windows.Controls.MenuItem[] GetRecursiveMenus(System.Windows.Controls.Menu menu)
        {

            try
            {
                List<System.Windows.Controls.MenuItem> result = new List<System.Windows.Controls.MenuItem>();

                foreach (object item in menu.Items)
                {
                    if (item.GetType() != typeof(System.Windows.Controls.MenuItem)) continue;

                    result.Add((item as System.Windows.Controls.MenuItem));
                    if ((item as System.Windows.Controls.MenuItem).Items.Count > 0)
                    {
                        result.AddRange(GetRecursiveSubMenus((item as System.Windows.Controls.MenuItem)));
                    }
                }

                return result.ToArray();
            }
            catch
            {
                return new List<System.Windows.Controls.MenuItem>().ToArray();
            }
        }
        public System.Windows.Controls.MenuItem[] GetRecursiveSubMenus(System.Windows.Controls.MenuItem menuItem)
        {
            try
            {
                List<System.Windows.Controls.MenuItem> result = new List<System.Windows.Controls.MenuItem>();

                foreach (object item in menuItem.Items)
                {
                    if (item.GetType() != typeof(System.Windows.Controls.MenuItem)) continue;

                    result.Add((item as System.Windows.Controls.MenuItem));
                    if ((item as System.Windows.Controls.MenuItem).Items.Count > 0)
                    {
                        result.AddRange(GetRecursiveSubMenus((item as System.Windows.Controls.MenuItem)));
                    }
                }

                return result.ToArray();
            }
            catch (Exception ex)
            {
                return new List<System.Windows.Controls.MenuItem>().ToArray();
            }
        }
        public void SetDoubleBuffer(Control container)
        {
            try
            {
                Control[] controls = GetRecursiveControls(container);
                if (controls == null) return;

                foreach (Control control in controls)
                {
                    typeof(Control).InvokeMember("DoubleBuffered",
                                                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                                                null,
                                                control,
                                                new object[] { true });
                }
            }
            catch
            {
                throw new Exception();
            }
        }
        public int GenerateDialogID()
        {
            try
            {
                int dialogID = 0;

                lock (_ActivatedDialog)
                {
                    while (true)
                    {
                        if (_ActivatedDialog.Contains(dialogID) == false)
                        {
                            _ActivatedDialog.Add(dialogID);
                            break;
                        }
                        dialogID++;
                    }
                }

                return dialogID;
            }
            catch
            {
                return -1;
            }
        }
        public void DialogClosed(int id)
        {
            try
            {
                lock (_ActivatedDialog)
                {
                    if (_ActivatedDialog.Contains(id) == true)
                    {
                        _ActivatedDialog.Remove(id);
                    }
                }
            }
            catch
            {
            }
        }
        #endregion
    }
}
