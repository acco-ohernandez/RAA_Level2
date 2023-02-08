#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls;

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
            bool createFloorPanels = false;
            if (currentForm.cbxCreateFloorPanels.IsChecked == true)
            {
                createFloorPanels = true;
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


            using (Transaction t = new Transaction(doc))
            {
                t.Start("Create Levesl");
                CreateLevelsFromCSV(doc, dataList, unitSelected, createFloorPanels, createCeilingPlans); 
                t.Commit();
            }

            

            return Result.Succeeded;
        }//EndOf_Execute

        internal static string GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }

        private void CreateLevelsFromCSV(Document Doc, List<string[]> DataList, string selectedUnitType, bool createFloorPanels, bool createCeilingPlans)
        {
            int levelCount      = 0;
            int CeilingCount    = 0;
            int FloorPanelCount = 0;

            foreach (var row in DataList)
            {
                string levelName = row[0];
                string imperialValue = row[1];
                string metricValue = row[2];

                // Convert the elevation string values to a Double elevationValue
                double elevationValue = 0;
                if (selectedUnitType == "Imperial") { double.TryParse(imperialValue, out elevationValue); }
                if (selectedUnitType == "Metric") 
                {
                    double.TryParse(metricValue, out elevationValue);
                    double levelElevation = elevationValue * 3.28084;
                }

                if (elevationValue == 0)
                {
                    continue;
                }

                // Create the levels
                Level currentLevel = Level.Create(Doc, elevationValue);
                currentLevel.Name = levelName;

                if (createFloorPanels)
                {
                    // Create the Floor Plan View
                    ViewFamilyType planVFT = GetVFTypeByName(Doc, "Floor Plan", ViewFamily.FloorPlan);
                    ViewPlan currentPlan = ViewPlan.Create(Doc, planVFT.Id, currentLevel.Id);
                    FloorPanelCount++;
                }
                if (createCeilingPlans)
                {
                    // Create the Ceiling View
                    ViewFamilyType ceilingPlanVFT = GetVFTypeByName(Doc, "Ceiling Plan", ViewFamily.CeilingPlan);
                    ViewPlan currentCeilingPlan = ViewPlan.Create(Doc, ceilingPlanVFT.Id, currentLevel.Id);
                    CeilingCount++;
                }
                levelCount++;


            }
            TaskDialog.Show("Info", "Created " + levelCount + " Levels\nCreated " + FloorPanelCount + " Floor Plan Views\nCreated " + CeilingCount + " Ceiling Plan Views");
        }//EndOf_CreateLevelsFromCSV

        private ViewFamilyType GetVFTypeByName(Document doc, string vftName, ViewFamily vfFloorPlan)
        {
            var collector = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType));
            foreach (ViewFamilyType currentVFT in collector)
            {
                if (currentVFT.Name == vftName && currentVFT.ViewFamily == vfFloorPlan)
                    return currentVFT;
            }
            return null;
        }
    }//EndOf_Command
}
