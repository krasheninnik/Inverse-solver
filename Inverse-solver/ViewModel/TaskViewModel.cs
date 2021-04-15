using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            this.CalculateTaskCommand = new CalculateTaskCommand(this);
            this.InitTaskCommand = new InitTaskCommand(this);
            this.HeatmapBuilder = new HeatmapBuilder();
        }

        HeatmapBuilder HeatmapBuilder { get; set; }

        private PlotModel myModel;

        public PlotModel MyModel
        {
            get { return myModel; }
            set { myModel = value; OnPropertyChanged(); }
        }


        CalculatingTask task { get; set; }

        // Commands
        public CalculateTaskCommand CalculateTaskCommand { get; set; }
        public OpenSettingsFormCommand OpenSettingsFormCommand { get; set; }

        public InitTaskCommand InitTaskCommand { get; set; }

        public void OpenSettingsForm()
        {
            SettingsForm sf = new SettingsForm();
            sf.DataContext = this;
            sf.Show();
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

            this.MyModel = HeatmapBuilder.buildCategorizedMap(xGrid, zGrid, resultsValues);
        }

        public void InitTask()
        {
            task.InitInverseTask(Hx, Hy, X0, Y0, Z0,
                MeasuredValues.ToArray(), MeasuredValues.Count,
                Xstart, Xend, XstepsAmount,
                Ystart, Yend, YstepsAmount,
                Zstart, Zend, ZstepsAmount,
                out int nodesSize, out int elemsSize);

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

