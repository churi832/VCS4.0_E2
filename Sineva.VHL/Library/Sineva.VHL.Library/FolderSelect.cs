/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.13 HJYOU
 * Description	: 
 * 
 ****************************************************************/
using System.ComponentModel;
using System.Drawing.Design;

namespace Sineva.VHL.Library
{
    [Editor(typeof(UIEditorFolderSelect), typeof(UITypeEditor))]
    public class FolderSelect
    {
        #region Fields
        private string m_Folder = "Select Folder";
        #endregion

        #region Properties
        public string SelectedFolder
        {
            get { return m_Folder; }
            set { m_Folder = value; }
        }
        #endregion

        #region Constructor
        public FolderSelect()
        {
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return m_Folder;
        }
        #endregion

    }
}
