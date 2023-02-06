#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace RAA_Level2
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // put any code needed for the form here

            // open form
            MyForm currentForm = new MyForm()
            {
                Width = 475,
                Height = 400,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Topmost = true,
            };

            currentForm.ShowDialog();

            // get form data and do something
            if (currentForm.DialogResult == false)
            {
                return Result.Cancelled;
            }

            // Check if Imperial units were selected
            string unitSelected = null;
            if (currentForm.rbImperial.IsChecked == true)
            {
                unitSelected = "Imperial";
            }
            // Check if Metric units were selected
            if (currentForm.rbMetric.IsChecked == true)
            {
                unitSelected = "Metric";
            }

            // Check if CreateFloorPanels was selected
            bool createFloolPanels = false;
            if (currentForm.cbxCreateFloorPanels.IsChecked == true)
            {
                createFloolPanels = true;
            }
            // Check if CreateCeilingPlans was selected
            bool createCeilingPlans = false;
            if (currentForm.cbxCreateCeilingPlans.IsChecked == true)
            {
                createCeilingPlans = true;
            }

            
            //Get CSV data
            string[] csvData = System.IO.File.ReadAllLines(currentForm.txbInput.Text);
            //Convert csvData to an array split at the comma
            List<string[]> dataList = new List<string[]>();
            foreach (string row in csvData)
            {
                string[] cellString = row.Split(',');
                dataList.Add(cellString);
            }
            // remove header row
            dataList.RemoveAt(0);


            CreateLevelsFromCSV(doc, dataList, unitSelected);

            

            return Result.Succeeded;
        }

        private void CreateLevelsFromCSV(Document Doc, List<string[]> DataList, string selectedUnitType)
        {
            foreach (var row in DataList)
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

        //private void CreateCeilingPlans(List<string[]> dataList)
        //{
        //    lblInfo.Content = "CreateCeilingPlans Method not yet implemented";
        //}

        //private void CreateFloorPlans(List<string[]> dataList)
        //{
        //    lblInfo.Content = "CreateFloorPlans Method not yet implemented";
        //}
    }
}
