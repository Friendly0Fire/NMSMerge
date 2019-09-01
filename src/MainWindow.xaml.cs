using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NMSMerge;
using NMSMerge.Properties;

namespace NMSMerge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsWorking { get; private set; } = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ReloadMods_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OpenModsFolder_OnClick(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.ModsFolder != string.Empty)
            {
                if (!System.IO.Directory.Exists(Settings.Default.ModsFolder))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(Settings.Default.ModsFolder);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(
                            "Unfortunately, the current mods folder does not exist and could not be created. Please change the folder and try again.", "Wrong mods folder", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                Process.Start(Settings.Default.ModsFolder);
            } else
                MessageBox.Show(
                    "Please set a valid mods folder first.",
                    "No mods folder", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowSettingsWindowAndUpdate()
        {
            SettingsWindow window = new SettingsWindow();
            if (window.ShowDialog() ?? false)
            {
                // TODO: Reload anything that may be affected by settings changes
            }
            else if (!App.ValidateSettings())
            {
                var result = MessageBox.Show("Some settings are currently invalid. Would you like to change them? Refusing will exit the mod manager.",
                    "Invalid settings", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if(result == MessageBoxResult.Yes)
                    ShowSettingsWindowAndUpdate();
                else
                    Close();
            }
        }

        private void Settings_OnClick(object sender, RoutedEventArgs e)
            => ShowSettingsWindowAndUpdate();

        private void Exit_OnClick(object sender, RoutedEventArgs e) => Close();

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (IsWorking)
            {
                var result = MessageBox.Show("There is still an operation in progress. Are you sure you want to exit?",
                    "Operation in progress", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.No)
                    e.Cancel = true;
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!App.ValidateSettings())
                ShowSettingsWindowAndUpdate();
        }
    }
}
