using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NMSMerge;

namespace NMSMerge
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool ValidateSettings()
        {
            if (!System.IO.Directory.Exists(Settings.Default.ModsFolder))
                return false;

            if (!System.IO.Directory.Exists(Settings.Default.GameFolder))
                return false;

            if (!System.IO.File.Exists(System.IO.Path.Combine(Settings.Default.GameFolder, @"Binaries\NMS.exe")))
                return false;

            return true;
        }
    }
}
