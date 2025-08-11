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
    [Editor(typeof(UIEditorFileSelect), typeof(UITypeEditor))]
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
