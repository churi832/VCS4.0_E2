using System;
using System.ComponentModel;

namespace Sineva.OHT.Common
{
    [Serializable()]
    //[Editor(typeof(ConfigFolderSelector), typeof(UITypeEditor))]
    [Editor(typeof(ConfigFolderSelector_WPF), typeof(ConfigFolderSelector_WPF))]
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
