using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Sineva.VHL.Library
{
    /// <summary>
    /// FormFolderSelector.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FormFolderSelect : UserControl, Xceed.Wpf.Toolkit.PropertyGrid.Editors.ITypeEditor
    {
        #region Fields
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(FolderSelect), typeof(FormFolderSelect), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion

        #region Properties
        public FolderSelect Value
        {
            get { return (FolderSelect)GetValue(ValueProperty);}
            set { SetValue(ValueProperty, value); }
        }
        #endregion

        #region Constructor
        public FormFolderSelect()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            var binding = new Binding("Value");
            binding.Source = propertyItem;
            binding.Mode= propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(this, ValueProperty, binding);
            return this;
        }
        private void btnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK )
            {
                if (!string.IsNullOrEmpty(dlg.SelectedPath))
                {
                    Value = new FolderSelect() { SelectedFolder= dlg.SelectedPath };
                }
            }
        }
        #endregion
    }
}
