using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace Sineva.OHT.Common
{
    [Serializable()]
    [Editor(typeof(ConfigFileSelector), typeof(UITypeEditor))]
    public class FileSelect
    {
        #region Fields
        private string m_File = "Select File";
        #endregion

        #region Properties
        public string SelectedFile
        {
            get { return m_File; }
            set { m_File = value; }
        }
        #endregion

        #region Constructor
        public FileSelect()
        {

        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return m_File;
        }
        #endregion
    }
}
