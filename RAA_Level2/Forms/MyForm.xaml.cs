using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using static System.Net.Mime.MediaTypeNames;


namespace RAA_Level2
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class MyForm : Window
    {
        public MyForm()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = @"C:\";
            openFile.Filter = "csv files (*.csv)|*.csv";

            if (openFile.ShowDialog() == true)
            {
                txbInput.Text = openFile.FileName;
            }
            else
            {
                txbInput.Text = "";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            //ProcessCSV();
            string enteredPath = ProcessCSV();
        }

        private string ProcessCSV()
        {
            // Verify the user entered a good file path
            string enteredPath = VerifyCSVPath();
            if (enteredPath != null)
            {
                // Verify Unit type selected. If not Metric or Imperial units have been selected,
                // the user to select one of the unit options.
                string selectedUnitType = VerifySelectedUnits();
                if (selectedUnitType != null)
                {
                    lblInfo.Content = "Processing...";
                    LblInfo_Clear();
                    this.DialogResult = true;
                    this.Close();
                    return selectedUnitType;
                }
                
            }
            else
            {
                LblInfo_Clear();   
                return null;
            }
            return null;
        }

        private string VerifySelectedUnits() 
        {
            if (rbImperial.IsChecked == true)
            {
                lblInfo.Content = "Imperial is checkd";
                LblInfo_Clear();
                return "Imperial";

            }
            else if (rbMetric.IsChecked == true)
            {
                lblInfo.Content = "Metric is checkd";
                LblInfo_Clear();
                return "Metric";

            }
            else
            {
                lblInfo.Content = "Please select a desired Unit type";
                LblInfo_Clear();
                return null;
                
            }
        }

        private async void LblInfo_Clear()
        {
            var t = Task.Delay(5000); 
            await t;
            lblInfo.Content = "-";
        }

        private string VerifyCSVPath()
        {
            // clear the lblInfo 
            lblInfo.Content = "-";
            
            // get the text from tbxInput
            string csvPath = txbInput.Text;
            if (File.Exists(csvPath))  // test if path exists
            {
                return csvPath;
            }
            else if (csvPath == "")   // if txbInput is empty.
            {
                //TaskDialog.Show("Error", "You did not enter a file path");
                lblInfo.Content = "You did not enter a file path!";
            }
            else
            {
                lblInfo.Content = "Wrong path: " + csvPath;  // If input path was entered wrong
            }
            return null;
        }
    }
}
