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
            string enteredPath = VerifyCSVPath();
            if (enteredPath != null)
            {
                this.DialogResult = false;
                this.Close();
            }

        }

        private void ProcessCSV0()
        {
            // Verify is a good file path
            string enteredPath = VerifyCSVPath();
            if (enteredPath != null)
            {
                lblInfo.Content = "Processing...";

                //Get CSV data
                string[] csvData = System.IO.File.ReadAllLines(enteredPath);

                List <string[]> dataList = new List <string[] >();
                foreach (string data in csvData)
                {
                    string[] cellString = data.Split(',');
                    dataList.Add(cellString);
                }

                // remove header row
                dataList.RemoveAt(0);

                // Verify Unity type selected
                string selectedUnitType =  VerifySelectedUnits();
                if(selectedUnitType != null)
                {
                    CreateLevelsFromCSV(dataList, selectedUnitType);

                    // Get the types seected
                    if (cbxCreateFloorPanels.IsChecked == true || cbxCreateCeilingPlans.IsChecked == true)
                    {
                        if (cbxCreateFloorPanels.IsChecked == true)
                        {
                            CreateFloorPlans(dataList);
                        }
                        if (cbxCreateCeilingPlans.IsChecked == true)
                        {
                            CreateCeilingPlans(dataList);
                        }
                    }
                    else { lblInfo.Content = "Level types have NOT been checked off."; } // Alert the user if no level types have been selected
                    
                    
                }
            }
            else
            {
                LblInfo_Clear();                
            }
        }

        private void CreateLevelsFromCSV(List<string[]> dataList, string selectedUnitType)
        {
            foreach (var row in dataList)
            {
                string levelName = row[0];
                string imperialValue = row[1];
                string metricValue = row[2];

                string elevationValue = "";
                if (selectedUnitType == "Imperial") { elevationValue = imperialValue; }
                if (selectedUnitType == "Metric") { elevationValue = metricValue; }

                double actualNumber;
                bool convertNumber = double.TryParse(elevationValue, out actualNumber);

                if (convertNumber == false)
                {
                    continue;
                }

                // same code as TryParse
                double actualNumber2 = 0;
                try
                {
                    actualNumber2 = double.Parse(elevationValue);
                }
                catch (Exception)
                {
                    TaskDialog.Show("Error", "The item in the number column is not a number");
                }

                if (convertNumber == false)
                {
                    TaskDialog.Show("Error", "The item in the number column is not a number");
                }

                double metricConvert = actualNumber * 3.28084;
                //Level currentLevel = Level.Create(doc, metricConvert);
                //currentLevel.Name = text;

                //ViewFamilyType planVFT = GetViewFamilyTypeByName(doc, "Floor Plan", ViewFamily.FloorPlan);
                //ViewFamilyType ceilingPlanVFT = GetViewFamilyTypeByName(doc, "Ceiling Plan", ViewFamily.CeilingPlan);

                //ViewPlan plan = ViewPlan.Create(doc, planVFT.Id, currentLevel.Id);
                //ViewPlan ceilingPlan = ViewPlan.Create(doc, ceilingPlanVFT.Id, currentLevel.Id);
            }
            
        }

        private void CreateCeilingPlans(List<string[]> dataList)
        {
            lblInfo.Content = "CreateCeilingPlans Method not yet implemented";
        }

        private void CreateFloorPlans(List<string[]> dataList)
        {
            lblInfo.Content = "CreateFloorPlans Method not yet implemented";
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
