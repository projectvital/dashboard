/*
    Project VITAL.Dashboard
    Copyright (C) 2017 - Universiteit Hasselt
    This project has been funded with support from the European Commission (Project number: 2015-BE02-KA203-012317). 

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using LMG.Infrastructure.Analytics;
using LMG.Infrastructure.Analytics.Daemon.Objects.Extensions;
using LMG.Infrastructure.Analytics.Daemon.Objects.Helpers;
using LMG.Infrastructure.Analytics.Daemon.Objects.Types;
using LMG.Infrastructure.Analytics.Helpers;
using LMG.Infrastructure.Analytics.Objects;
using LMG.Infrastructure.Analytics.Objects.DB;
using LMG.Infrastructure.Analytics.Objects.DB.Base;
using LMG.Infrastructure.Analytics.Objects.Helpers;
using LMG.Infrastructure.Analytics.Objects.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using TinCan;

namespace LMG.Infrastructure.Analytics.Daemon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                if (IsDaemonMode("pull"))
                    ActionHelper.Main_Pull();
                else if (IsDaemonMode("push"))
                    ActionHelper.Main_Push();
                else if (IsDaemonMode("import_json"))
                    XApiImportHelper.ImportJsonStatementsInFolder("Export");
                else if (IsDaemonMode("calculate_performancegroup"))
                    DataPreparationHelper.CalculateStudentPerformanceGroup();
                else if (IsDaemonMode("import_metadata"))
                    MetadataImportHelper.ImportMetadataFromKnownFilesToDatabase();
                else if (IsDaemonMode("convert_jsontocsv"))
                    DataConversionHelper.ConvertJsonToCsv();
            }
            catch(Exception ex)
            {
                if(ConfigurationManager.AppSettings["DebugMode"] == "true")
                    MessageBox.Show(ex.Message + " " + ex.StackTrace);
            }
            Environment.Exit(0);
        }

        public bool IsDaemonMode(string key)
        {
            return ("" + ConfigurationManager.AppSettings["DaemonMode"]).ToLower() == (""+key).ToLower();
        }

        


        
    }
}
