using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Sineva.OHT.Common
{
    /// <summary>
    /// Interaction logic for ConfigFolderSelector_WPF.xaml
    /// </summary>
    public partial class ConfigFolderSelector_WPF : UserControl, Xceed.Wpf.Toolkit.PropertyGrid.Editors.ITypeEditor
    {
        public ConfigFolderSelector_WPF()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(FolderSelect), typeof(ConfigFolderSelector_WPF), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public FolderSelect Value
        {
            get { return (FolderSelect)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            var binding = new Binding("Value");
            binding.Source = propertyItem;
            binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(this, ValueProperty, binding);
            return this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(dlg.SelectedPath))
                {
                    Value = new FolderSelect() { SelectedFolder = dlg.SelectedPath };
                }
            }
        }
    }
}
