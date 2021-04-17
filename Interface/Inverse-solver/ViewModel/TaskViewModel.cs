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
            this.InverseTask = new InverseTask();

            // Commands:
            this.OpenSettingsFormCommand = new OpenSettingsFormCommand(this);
            this.OpenDiscrepancyViewCommand = new OpenDiscrepancyViewCommand(this);
            this.CalculateTaskCommand = new CalculateTaskCommand(this);
            this.InitTaskCommand = new InitTaskCommand(this);
            this.InitTaskCommandTestCase = new InitTaskCommandTestCase(this);

            //  Graphics
            this.GraphicsBuilder = new GraphicsBuilder();
        }

        private GraphicsBuilder GraphicsBuilder { get; set; }

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

        private InverseTask InverseTask { get; set; }

        #region CommandsDefinition
        public CalculateTaskCommand CalculateTaskCommand { get; private set; }
        public OpenSettingsFormCommand OpenSettingsFormCommand { get; private set; }

        public OpenDiscrepancyViewCommand OpenDiscrepancyViewCommand { get; private set; }

        public InitTaskCommand InitTaskCommand { get; private set; }
        public InitTaskCommandTestCase InitTaskCommandTestCase { get; private set; }
        #endregion

        #region CommandsFunctions
        public void OpenSettingsForm()
        {
            CommandManager.InvalidateRequerySuggested();
            SettingsForm sf = new SettingsForm();
            sf.DataContext = this;
            sf.Show();
        }

        public void InitTaskTestCase()
        {
            // set test case parameters
            Hx = 100;
            Nx = 10;
            Hy = 1;
            Ny = 4;
            X0 = 1900;
            Y0 = 0;
            Z0 = 0;

            Alpha = 10;

            MeasuredValues = new List<Value>();
            for (int i = 0; i < (Nx + 1) * (Ny + 1); i++) MeasuredValues.Add(new Value(i, i, i));


            Xstart = 2000;
            Xend = 3000;
            XstepsAmount = 2;

            Ystart = 0;
            Yend = 4;
            YstepsAmount = 2;

            Zstart = -1000;
            Zend = -500;
            ZstepsAmount = 2;

            // call init function
            this.InitTask();
        }

        public void InitTask(/*IClosable window*/)
        {
            InverseTask.Init(Hx, Nx, Hy, Ny, new Value(X0, Y0, Z0),
                MeasuredValues.ToArray(), MeasuredValues.Count,
                Xstart, Xend, XstepsAmount,
                Ystart, Yend, YstepsAmount,
                Zstart, Zend, ZstepsAmount,
                Alpha);

            OnPropertyChanged("YResultGridLayers");
            this.IsTaskInitializated = true;
            this.IsTaskCalculated = false;
            //window.Close();
        }

        public void CalculateTask()
        {
            // memory for FiniteElems allocated in Init Method
            InverseTask.CalculateTask();
            this.HeatmapModel = GraphicsBuilder.buildHeatmap(InverseTask.GridInfo, InverseTask.ResultsValues);
            this.IsTaskCalculated = true;
        }
        public void OpenDiscrepancyView()
        {
            DiscrepancyView dv = new DiscrepancyView();
            dv.DataContext = this;
            DiscrepancyModel = GraphicsBuilder.buildDiscrepancyGraph(InverseTask.XMeasureGrid, InverseTask.DiscrepancyValues);
            dv.Show();

            // display discrepancy for first y layer
            YMeasureLayerIndex = 0;
        }
        #endregion

        // To control buttons activity (add later):
        public bool IsTaskInitializated { get; set; }
        public bool IsTaskCalculated { get; set; }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // Caller Member Name allows not to pass property name in the function,
        // it will be passed automatically
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        // Contains information about Y layers, make it possible to see results on each Y layer.
        public double[] YResultGridLayers { get { return InverseTask.YResultGridLayers; }}
        public double[] YMeasureGridLayers{  get { return InverseTask.YMeasureGridLayers; } }

        // Indexes for changing displayed results by selected Y
        public int YResultLayerIndex
        {
            get { return InverseTask.YResultLayerIndex; }
            set
            {
                InverseTask.YResultLayerIndex = value;
                // Prop InverseTask.ResultsValues depends on InverseTask.YResultsLayerIndex, so.. its updated
                HeatmapModel = GraphicsBuilder.buildHeatmap(InverseTask.GridInfo, InverseTask.ResultsValues);
            }
        }

        private int yMeasureLayerIndex;
        public int YMeasureLayerIndex
        {
            get { return yMeasureLayerIndex; }
            set
            {
                yMeasureLayerIndex = value;
                // Update discrepancy values for this Y level
                InverseTask.GetDiscrepancy(yMeasureLayerIndex);
                DiscrepancyModel = GraphicsBuilder.buildDiscrepancyGraph(InverseTask.XMeasureGrid, InverseTask.DiscrepancyValues);
            }
        }

        #region SettingsParameters
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
        #endregion
    }
}

