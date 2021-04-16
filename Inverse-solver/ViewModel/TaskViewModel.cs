using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Inverse_solver.Model;
using Inverse_solver.ViewModel.Commands;
using Inverse_solver.ViewModel.Converters;
using Inverse_solver.Views;
using OxyPlot;

namespace Inverse_solver.ViewModel
{
    public class TaskViewModel : INotifyPropertyChanged
    {

        public TaskViewModel()
        {
            this.task = new CalculatingTask();
            this.OpenSettingsFormCommand = new OpenSettingsFormCommand(this);
            this.OpenDiscrepancyViewCommand = new OpenDiscrepancyViewCommand(this);
            this.CalculateTaskCommand = new CalculateTaskCommand(this);
            this.InitTaskCommand = new InitTaskCommand(this);
            this.GraphicsBuilder = new GraphicsBuilder();
        }

        GraphicsBuilder GraphicsBuilder { get; set; }

        // Model to show results of Inverse task
        private PlotModel heatmapModel;

        public PlotModel HeatmapModel
        {
            get { return heatmapModel; }
            set { heatmapModel = value; OnPropertyChanged(); }
        }

        // Model to show discrepancy graph
        private PlotModel discrepancyModel;

        public PlotModel DiscrepancyModel
        {
            get { return discrepancyModel; }
            set { discrepancyModel = value; OnPropertyChanged(); }
        }

        CalculatingTask task { get; set; }

        // Commands
        public CalculateTaskCommand CalculateTaskCommand { get; set; }
        public OpenSettingsFormCommand OpenSettingsFormCommand { get; set; }

        public OpenDiscrepancyViewCommand OpenDiscrepancyViewCommand { get; set; }

        public InitTaskCommand InitTaskCommand { get; set; }

        public void OpenSettingsForm()
        {
            this.IsTaskCalculated = true;
            CommandManager.InvalidateRequerySuggested();
            SettingsForm sf = new SettingsForm();
            sf.DataContext = this;
            sf.Show();
        }


        public void OpenDiscrepancyView()
        {
            DiscrepancyView dv = new DiscrepancyView();
            dv.DataContext = this;
            this.discrepancyModel = this.GraphicsBuilder.buildDiscrepancyGraph(
                new List<double> { 0, 10, 20, 30, 40, 50, 60, 70, 80, 80, 90, 100 },
                new List<double> { 0, 10, 20, 30, 40, 50, 60, 50, 40, 30, 20, 10 }
                );
            dv.Show();
        }


        public void CalculateTask()
        {
            // this part for modeling test case
            // kiddna mock from dll...


            List<double> xGrid = new List<double> { 0, 10, 20, 30, 40, 50, 60 };
            List<double> yGrid = new List<double> { 0, 3, 6, };
            List<double> zGrid = new List<double> { 0, 10, 20, 30, 40};

            int pointsInXY = xGrid.Count() * yGrid.Count();
            int pointsInX = xGrid.Count();
            int pointsInY = yGrid.Count();
            int pointsInZ = zGrid.Count();

            Ylevel = 0;
            elemsInX = pointsInX - 1;
            elemsInY = pointsInY - 1;
            elemsInZ = pointsInZ - 1;
            elemsInXY = elemsInX * elemsInY;

            ElemsSize = elemsInX * elemsInY * elemsInZ;
            NodesSize = pointsInX * pointsInY * pointsInZ;

            Nodes = new Value[NodesSize];
            int ni = 0;
            for (int zi = 0; zi < zGrid.Count(); zi++)
            {
                for (int yi = 0; yi < yGrid.Count(); yi++)
                {
                    for (int xi = 0; xi < xGrid.Count(); xi++)
                    {
                        Nodes[ni++] = new Value(xGrid[xi], yGrid[yi], zGrid[zi]);
                    }
                }
            }


            FiniteElems = new FiniteElem[ElemsSize];
      

            int ei = 0;
            for (int zi = 0; zi < zGrid.Count() - 1; zi++)
            {
                for (int yi = 0; yi < yGrid.Count() - 1; yi++)
                {
                    for (int xi = 0; xi < xGrid.Count() - 1; xi++)
                    {
                        int n0 = zi * pointsInXY + yi * pointsInX + xi;
                        int n1 = zi * pointsInXY + yi * pointsInX + xi + 1;
                        int n2 = zi * pointsInXY + (yi + 1) * pointsInX + xi;
                        int n3 = zi * pointsInXY + (yi + 1) * pointsInX + xi + 1;
                        int n4 = (zi + 1) * pointsInXY + yi * pointsInX + xi;
                        int n5 = (zi + 1) * pointsInXY + yi * pointsInX + xi + 1;
                        int n6 = (zi + 1) * pointsInXY + (yi + 1) * pointsInX + xi;
                        int n7 = (zi + 1) * pointsInXY + (yi + 1) * pointsInX + xi + 1;
                        int value = (xi + 1) * (yi + 1) * (zi + 1);

                        Value p = new Value(ei, ei, ei);
                        FiniteElems[ei++] = new FiniteElem(n0, n1, n2, n3, n4, n5, n6, n7, p);
                    }
                }
            }





            resultsValues = new List<List<double>>();
            for (int zi = 0; zi < zGrid.Count() - 1; zi++)
            {
                var values = new List<double>();
                for (int xi = 0; xi < xGrid.Count() - 1; xi++)
                {
                    values.Add(FiniteElems[zi * elemsInXY + Ylevel * elemsInX + xi].P.Z);
                }

                resultsValues.Add(values);
            }

            this.HeatmapModel = GraphicsBuilder.buildHeatmap(xGrid, zGrid, resultsValues);
            this.IsTaskCalculated = true;
        }

        public void InitTask()
        {
            task.InitInverseTask(Hx, Hy, X0, Y0, Z0,
                MeasuredValues.ToArray(), MeasuredValues.Count,
                Xstart, Xend, XstepsAmount,
                Ystart, Yend, YstepsAmount,
                Zstart, Zend, ZstepsAmount,
                out int nodesSize, out int elemsSize);

            this.IsTaskInitializated = true;
            this.IsTaskCalculated = false;
            /*  Comment untill develop it:
            NodesSize = nodesSize;
            ElemsSize = elemsSize;

            Nodes = new Value[NodesSize];
            task.GetNodes(Nodes, NodesSize);
            */
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Caller Member Name allows not to pass property name in the function,
        // it will be passed automatically
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        
        // to control buttons activity (add later):
        private bool isTaskInitializated;

        public bool IsTaskInitializated
        {
            get { return isTaskInitializated; }
            set { isTaskInitializated = value;
                //OnPropertyChanged("CalculateTaskCommand");
                //CommandManager.InvalidateRequerySuggested();
            }
        }


        private bool isTaskCalculated;

        public bool IsTaskCalculated
        {
            get { return isTaskCalculated; }
            set
            {
                isTaskCalculated = value;
                //OnPropertyChanged("OpenDiscrepancyViewCommand");
               // CommandManager.InvalidateRequerySuggested();
            }
        }
        
        
        // Task settings props:
        // For measures grid:
        public int Hx { get; set; }
        public int Hy { get; set; }
        public double X0 { get; set; }
        public double Y0 { get; set; }
        public double Z0 { get; set; }
        // For Measures:
        public List<Value> MeasuredValues { get; set; }
        // For space grid:
        public double Xstart { get; set; }
        public double Xend { get; set; }
        public int XstepsAmount { get; set; }
        public double Ystart { get; set; }
        public double Yend { get; set; }
        public int YstepsAmount { get; set; }
        public double Zstart { get; set; }
        public double Zend { get; set; }
        public int ZstepsAmount { get; set; }

        // Calculating result:
        public int ElemsSize { get; set; }
        public FiniteElem[] FiniteElems { get; set; }

        public int NodesSize { get; set; }
        public Value[] Nodes { get; set; }

        public int elemsInX { get; set; }
        public int elemsInXY { get; set; }

        public int elemsInY { get; set; }
        public int elemsInZ { get; set; }

        public int Ylevel { get; set; }
        public List<List<double>> resultsValues { get; set; }
    }
}

