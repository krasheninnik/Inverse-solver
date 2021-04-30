﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Inverse_solver.Model;
using Inverse_solver.ViewModel.Commands;
using Inverse_solver.ViewModel.Converters;
using Inverse_solver.Views;
using Newtonsoft.Json;
using OxyPlot;

namespace Inverse_solver.ViewModel
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        public TaskViewModel()
        {
            this.InverseTask = new InverseTask();
            this.InitParameters = new InitParameters();

            // Startup init:
            ResultComponentsToShowList = new List<string>{ "X", "Y", "Z" };
            ResultComponentToShow = ResultComponentsToShowList[0];

            // Commands:
            this.OpenSettingsFormCommand = new OpenSettingsFormCommand(this);
            this.OpenDiscrepancyViewCommand = new OpenDiscrepancyViewCommand(this);
            this.OpenMagneticInductionViewCommand = new OpenMagneticInductionViewCommand(this);
            this.CalculateTaskCommand = new CalculateTaskCommand(this, (ex) => StatusMessage = ex.Message);
            this.InitTaskCommand = new InitTaskCommand(this, (ex) => StatusMessage = ex.Message);
            this.SetInitParametersFromFileCommand = new SetInitParametersFromFileCommand(this, (ex) => StatusMessage = ex.Message);
            this.SaveInitParametersToFileCommand = new SaveInitParametersToFileCommand(this, (ex) => StatusMessage = ex.Message);

            //  Graphics
            this.GraphicsBuilder = new GraphicsBuilder();
        }

        private string statusMessage;

        public string StatusMessage
        {
            get { return statusMessage; }
            set { statusMessage = value; OnPropertyChanged(); }
        }


        #region GraphicsBuilderAndModels
        private GraphicsBuilder GraphicsBuilder { get; set; }

        // Model to show results of Inverse task
        private PlotModel heatmapModel;

        public PlotModel HeatmapModel
        {
            get { return heatmapModel; }
            set
            {
                heatmapModel = value;
                OnPropertyChanged();
            }
        }

        // Model to show discrepancy graph
        private PlotModel discrepancyModel;

        public PlotModel DiscrepancyModel
        {
            get { return discrepancyModel; }
            set { discrepancyModel = value; OnPropertyChanged(); }
        }

        // Model to show magnetic induction graph
        private PlotModel magneticInductionModel;

        public PlotModel MagneticInductionModel
        {
            get { return magneticInductionModel; }
            set { magneticInductionModel = value; OnPropertyChanged(); }
        }

        public bool DiscrepancyViewOpened = false;
        public bool MagneticInductionViewOpened = false;
        #endregion

        private InverseTask InverseTask { get; set; }

        #region CommandsDefinition
        public CalculateTaskCommand CalculateTaskCommand { get; private set; }
        public OpenSettingsFormCommand OpenSettingsFormCommand { get; private set; }
        public OpenMagneticInductionViewCommand OpenMagneticInductionViewCommand { get; private set; }
        public OpenDiscrepancyViewCommand OpenDiscrepancyViewCommand { get; private set; }

        public InitTaskCommand InitTaskCommand { get; private set; }
        public SetInitParametersFromFileCommand SetInitParametersFromFileCommand { get; private set; }
        public SaveInitParametersToFileCommand SaveInitParametersToFileCommand { get; private set; }
        #endregion

        #region CommandsFunctions
        public void OpenSettingsForm()
        {
            CommandManager.InvalidateRequerySuggested();
            SettingsForm sf = new SettingsForm();
            sf.DataContext = this;
            sf.Show();
        }

        public async Task SetInitParametersFromFile()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "json files (*.json)|*.json";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    await Task.Run(async () =>
                    {
                        // Deserialize JSON directly from a file and init variables
                        using (StreamReader file = File.OpenText(filePath))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            InitParameters = (InitParameters)serializer.Deserialize(file, typeof(InitParameters));
                        }
                    });
                }
            }
        }

        public async Task SaveInitParametersToFile()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "json files (*.json)|*.json";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = saveFileDialog.FileName;

                    await Task.Run(async () =>
                    {
                        // serialize JSON directly to a file
                        using (StreamWriter file = File.CreateText(filePath))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            serializer.Serialize(file, InitParameters);
                        }
                    });
                }
            }
        }

        public async Task InitTask(/*IClosable window*/)
        {
            await Task.Run(() =>
            {
                InverseTask.Init(InitParameters);
            });
            OnPropertyChanged("YResultGridLayers");
            this.IsTaskInitializated = true;
            this.IsTaskCalculated = false;
            //window.Close();

            // Update commands CanExecute states
            CommandManager.InvalidateRequerySuggested();
        }

        public async Task CalculateTask()
        {
            await Task.Run(() =>
            {
                InverseTask.CalculateTask();
            });

            this.HeatmapModel = GraphicsBuilder.buildHeatmap(InverseTask.GridInfo, InverseTask.ResultsValues, ResultComponentToShow);
            this.IsTaskCalculated = true;

            // Update commands CanExecute states
            CommandManager.InvalidateRequerySuggested();
        }

        public void OpenMagneticInductionView()
        {
            MagneticInductionViewOpened = true;
            MagneticInductionView view = new MagneticInductionView();
            view.DataContext = this;
            view.Show();

            // display discrepancy for first y layer
            YMagnIndMeasureLayerIndex = 0;
        }

        public void OpenDiscrepancyView()
        {
            DiscrepancyViewOpened = true;
            DiscrepancyView view = new DiscrepancyView();
            view.DataContext = this;
            view.Show();
            // display discrepancy for first y layer
            YDiscrepancyMeasureLayerIndex = 0;
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
        public double[] YResultGridLayers { get { return InverseTask.YResultGridLayers; } }
        public double[] YMeasureGrid { get { return InverseTask.YMeasureGrid; } }
        public double[] XMeasureGrid { get { return InverseTask.XMeasureGrid; } }

        private List<string> resultComponentsToShowList;
        public List<string> ResultComponentsToShowList
        {
            get { return resultComponentsToShowList; }
            private set { resultComponentsToShowList = value; OnPropertyChanged(); }
        }

        private string resultComponentToShow;
        public string ResultComponentToShow
        {
            get { return resultComponentToShow; }
            set
            {
                resultComponentToShow = value;
                if (InverseTask.ResultsValues.Count > 0)
                {
                    HeatmapModel = GraphicsBuilder.buildHeatmap(InverseTask.GridInfo, InverseTask.ResultsValues, ResultComponentToShow);
                }
                OnPropertyChanged();
            }
        }

        // Indexes for changing displayed results by selected Y
        public int YResultLayerIndex
        {
            get { return InverseTask.YResultLayerIndex; }
            set
            {
                InverseTask.YResultLayerIndex = value;
                // Prop InverseTask.ResultsValues depends on InverseTask.YResultsLayerIndex, so.. its updated
                HeatmapModel = GraphicsBuilder.buildHeatmap(InverseTask.GridInfo, InverseTask.ResultsValues, ResultComponentToShow);
            }
        }

        private int yDiscrepancyMeasureLayerIndex;
        public int YDiscrepancyMeasureLayerIndex
        {
            get { return yDiscrepancyMeasureLayerIndex; }
            set
            {
                yDiscrepancyMeasureLayerIndex = value;
                // Update discrepancy values for this Y level
                InverseTask.GetDiscrepancyByY(yDiscrepancyMeasureLayerIndex);
                DiscrepancyModel = GraphicsBuilder.buildDiscrepancyGraph(InverseTask.XMeasureGrid, InverseTask.DiscrepancyValuesByY, "X");
            }
        }

        private int xDiscrepancyMeasureLayerIndex;
        public int XDiscrepancyMeasureLayerIndex
        {
            get { return xDiscrepancyMeasureLayerIndex; }
            set
            {
                xDiscrepancyMeasureLayerIndex = value;
                // Update discrepancy values for this X level
                InverseTask.GetDiscrepancyByX(xDiscrepancyMeasureLayerIndex);
                DiscrepancyModel = GraphicsBuilder.buildDiscrepancyGraph(InverseTask.YMeasureGrid, InverseTask.DiscrepancyValuesByX, "Y");
            }
        }

        private int yMagnIndMeasureLayerIndex;
        public int YMagnIndMeasureLayerIndex
        {
            get { return yMagnIndMeasureLayerIndex; }
            set
            {
                yMagnIndMeasureLayerIndex = value;
                // Update discrepancy values for this Y level
                InverseTask.GetMagneticInductionByY(yMagnIndMeasureLayerIndex);
                MagneticInductionModel = GraphicsBuilder.buildMagneticInductionGraph(InverseTask.XMeasureGrid, InverseTask.MagnIndValuesByY, "X");
            }
        }

        private int xMagnIndMeasureLayerIndex;
        public int XMagnIndMeasureLayerIndex
        {
            get { return xMagnIndMeasureLayerIndex; }
            set
            {
                xMagnIndMeasureLayerIndex = value;
                // Update discrepancy values for this X level
                InverseTask.GetMagneticInductionByX(xMagnIndMeasureLayerIndex);
                MagneticInductionModel = GraphicsBuilder.buildMagneticInductionGraph(InverseTask.YMeasureGrid, InverseTask.MagnIndValuesByX, "Y");
            }
        }


        private InitParameters initParameters;
        public InitParameters InitParameters
        {
            get { return initParameters; }
            set { initParameters = value; OnPropertyChanged(); }
        }
    }
}

