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
        private bool TESTCASE = false;

        public TaskViewModel()
        {
            this.InverseTask = new InverseTask();
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
            set { heatmapModel = value;
                OnPropertyChanged(); }
        }

        // Model to show discrepancy graph
        private PlotModel discrepancyModel;

        public PlotModel DiscrepancyModel
        {
            get { return discrepancyModel; }
            set { discrepancyModel = value; OnPropertyChanged(); }
        }

        InverseTask InverseTask { get; set; }

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
            if (TESTCASE)
            {
                XMeasureGrid = new double[] { 0, 10, 20, 30, 40, 50, 60, 70, 80, 80, 90, 100 };
                DiscrepancyValues = new double[] { 0, 10, 20, 30, 40, 50, 60, 50, 40, 30, 20, 10 };
                YMeasureGridLayers = new double[] { -200, -100, 0 };
            }
            DiscrepancyModel = GraphicsBuilder.buildDiscrepancyGraph(XMeasureGrid, DiscrepancyValues);
            dv.Show();
        }


        //private List<double> xGrid = new List<double> { 0, 10, 20, 30, 40, 50, 60 };
        //private List<double> yGrid = new List<double> { 0, 3, 6, 9, 12, 15 };
        //private List<double> zGrid = new List<double> { 0, 10, 20, 30, 40 };
        public void CalculateTask()
        {
            // this part for modeling test case
            // kiddna mock from dll...
    
            if (TESTCASE)
            {
                //var yGridLayersTemp = new double[yGrid.Count-1];
                //for (int i = 1; i < yGrid.Count; i++)
                //{
                //    yGridLayersTemp[i-1] = ((yGrid[i] + yGrid[i - 1]) / 2);
                //}
                //YResultGridLayers = yGridLayersTemp;

                //int pointsInXY = xGrid.Count() * yGrid.Count();
                //int pointsInX = xGrid.Count();
                //int pointsInY = yGrid.Count();
                //int pointsInZ = zGrid.Count();

                //elemsInX = pointsInX - 1;
                //elemsInY = pointsInY - 1;
                //elemsInZ = pointsInZ - 1;

                //ElemsSize = elemsInX * elemsInY * elemsInZ;
                //NodesSize = pointsInX * pointsInY * pointsInZ;

                //Nodes = new Value[NodesSize];
                //int ni = 0;
                //for (int zi = 0; zi < zGrid.Count(); zi++)
                //{
                //    for (int yi = 0; yi < yGrid.Count(); yi++)
                //    {
                //        for (int xi = 0; xi < xGrid.Count(); xi++)
                //        {
                //            Nodes[ni++] = new Value(xGrid[xi], yGrid[yi], zGrid[zi]);
                //        }
                //    }
                //}


                //FiniteElems = new FiniteElem[ElemsSize];


                //int ei = 0;
                //for (int zi = 0; zi < zGrid.Count() - 1; zi++)
                //{
                //    for (int yi = 0; yi < yGrid.Count() - 1; yi++)
                //    {
                //        for (int xi = 0; xi < xGrid.Count() - 1; xi++)
                //        {
                //            int n0 = zi * pointsInXY + yi * pointsInX + xi;
                //            int n1 = zi * pointsInXY + yi * pointsInX + xi + 1;
                //            int n2 = zi * pointsInXY + (yi + 1) * pointsInX + xi;
                //            int n3 = zi * pointsInXY + (yi + 1) * pointsInX + xi + 1;
                //            int n4 = (zi + 1) * pointsInXY + yi * pointsInX + xi;
                //            int n5 = (zi + 1) * pointsInXY + yi * pointsInX + xi + 1;
                //            int n6 = (zi + 1) * pointsInXY + (yi + 1) * pointsInX + xi;
                //            int n7 = (zi + 1) * pointsInXY + (yi + 1) * pointsInX + xi + 1;
                //            int value = (xi + 1) * (yi + 1) * (zi + 1);

                //            Value p = new Value(ei, ei, ei);
                //            FiniteElems[ei++] = new FiniteElem(n0, n1, n2, n3, n4, n5, n6, n7, p);
                //        }
                //    }
                //}

                //ResultsValues = new List<List<double>>();
                //for (int zi = 0; zi < zGrid.Count() - 1; zi++)
                //{
                //    var values = new List<double>();
                //    for (int xi = 0; xi < xGrid.Count() - 1; xi++)
                //    {
                //        values.Add(FiniteElems[zi * elemsInXY + YResultLayerIndex * elemsInX + xi].P.Z);
                //    }

                //    resultsValues.Add(values);
                //}
            }
            else
            {
                // memory for FiniteElems allocated in Init Method
                InverseTask.CalculateTask(FiniteElems);
             }

            for(int i = 0; i < FiniteElems.Length; i++)
            {
              //  FiniteElems[i].P = new Value(i, i, i);
            }

            this.HeatmapModel = GraphicsBuilder.buildHeatmap(GridInfo, ResultsValues);
            this.IsTaskCalculated = true;
        }

        private List<List<double>> resultsValues;

        public List<List<double>> ResultsValues
        {
            get {
                int elemsInXY = GridInfo.elemsInX * GridInfo.elemsInY;
                resultsValues = new List<List<double>>();
                for (int zi = 0; zi < GridInfo.elemsInZ; zi++)
                {
                    var values = new List<double>();
                    for (int xi = 0; xi < GridInfo.elemsInX; xi++)
                    {
                        values.Add(FiniteElems[zi * elemsInXY + YResultLayerIndex * GridInfo.elemsInX + xi].P.Z);
                    }

                    resultsValues.Add(values);
                };
                return resultsValues;
            }
            set { resultsValues = value; }
        }

        public void InitTask()
        {
            Hx = 100;           
            Nx = 3;
            Hy = 1;
            Ny = 4;
            X0 = 1900;
            Y0 = 0;
            Z0 = 0;

            Alpha = 10;

            MeasuredValues = new List<Value>();
            for (int i = 0; i < (Nx + 1) * (Ny + 1) ; i++) MeasuredValues.Add(new Value(i, i, i));


            Xstart = 2000;
            Xend = 3000;
            XstepsAmount = 2;

            Ystart = 0;
            Yend = 1;
            YstepsAmount = 1;

            Zstart = -1000;
            Zend = -500;
            ZstepsAmount = 2;

            InverseTask.Init(Hx, Nx, Hy, Ny, new Value(X0, Y0, Z0),
                MeasuredValues.ToArray(), MeasuredValues.Count,
                Xstart, Xend, XstepsAmount,
                Ystart, Yend, YstepsAmount,
                Zstart, Zend, ZstepsAmount,
                Alpha);

            InverseTask.GetGridInformation(out GridInformation gridInfo);
            GridInfo = gridInfo;

            double[] _yResultGridLayers = new double[GridInfo.yResultsLayersSize];

            // initialize grids:
            Nodes = new Value[GridInfo.pointsSize];
            YResultGridLayers = new double[GridInfo.yResultsLayersSize];
            InverseTask.GetResultGrids(Nodes, YResultGridLayers);

            XMeasureGrid = new double[GridInfo.xMeasureLayersSize];
            DiscrepancyValues = new double[GridInfo.xMeasureLayersSize];
            YMeasureGridLayers = new double[GridInfo.yMeasureLayersSize];
            InverseTask.GetMeasureGrids(XMeasureGrid, YMeasureGridLayers);

            // allocate memory for FE
            FiniteElems = new FiniteElem[GridInfo.elemsSize];

            this.IsTaskInitializated = true;
            this.IsTaskCalculated = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Caller Member Name allows not to pass property name in the function,
        // it will be passed automatically
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        private double[] yResultGridLayers;

        public double[] YResultGridLayers
        {
            get { return yResultGridLayers; }
            set
            {
                yResultGridLayers = value;
                OnPropertyChanged();
            }
        }

        private int yResultLayerIndex;
        public int YResultLayerIndex
        {
            get { return yResultLayerIndex; }
            set
            {
                yResultLayerIndex = value;
                HeatmapModel = GraphicsBuilder.buildHeatmap(GridInfo, ResultsValues);
            }
        }

        private double[] XMeasureGrid { get; set; }

        private double[] DiscrepancyValues;

        private double[] yMeasureGridLayers;

        public double[]  YMeasureGridLayers
        {
            get { return yMeasureGridLayers; }
            set
            {
                yMeasureGridLayers = value;
                OnPropertyChanged();
            }
        }

        private int yMeasureLayerIndex;
        public int YMeasureLayerIndex
        {
            get { return yMeasureLayerIndex; }
            set
            {
                yMeasureLayerIndex = value;
                // update discrepancy values for this Y level
                InverseTask.GetDiscrepancy(yMeasureLayerIndex, DiscrepancyValues);
                // redraw model:
                DiscrepancyModel = GraphicsBuilder.buildDiscrepancyGraph(XMeasureGrid, DiscrepancyValues);
            }
        }

        // to control buttons activity (add later):
        private bool isTaskInitializated;

        public bool IsTaskInitializated
        {
            get { return isTaskInitializated; }
            set
            {
                isTaskInitializated = value;
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
        public int Nx { get; set; }
        public int Ny { get; set; }
        public double Alpha { get; set; }

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

        private GridInformation GridInfo;
    }
}

