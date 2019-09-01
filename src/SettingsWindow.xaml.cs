using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NMSMerge
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void BtnOK_OnClick(object sender, RoutedEventArgs e)
        {
            if (System.IO.Directory.Exists(txtModsFolder.Text))
                Settings.Default.ModsFolder = txtModsFolder.Text;
            else
            {
                MessageBox.Show("Mods folder is invalid.", "Settings error", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }

            if (System.IO.Directory.Exists(txtGameFolder.Text) && System.IO.File.Exists(System.IO.Path.Combine(txtGameFolder.Text, @"Binaries\NMS.exe")))
                Settings.Default.GameFolder = txtGameFolder.Text;
            else
            {
                MessageBox.Show("Game folder is invalid.", "Settings error", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }

            Settings.Default.Save();
            DialogResult = true;
            Close();
        }
    }
}
