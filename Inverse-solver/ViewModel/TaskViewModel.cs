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
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Inverse_solver.ViewModel
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        public TaskViewModel()
        {
            this.task = new CalculatingTask();
            this.OpenSettingsFormCommand = new OpenSettingsFormCommand(this);
            this.InitTaskCommand = new InitTaskCommand(this);

            var model = new PlotModel { Title = "Results" };
            // Weekday axis (horizontal)

            // CATEGORY AXIS CODE====================================
            var categoryaxis = new CategoryAxis();
            for (int i = 0; i < 7; i++)
            {
                int value = i * 100;
                categoryaxis.ActualLabels.Add(value.ToString());

            }
            categoryaxis.Angle = 0; // 40;
            model.Axes.Add(categoryaxis);

            // Cake type axis (vertical)
            model.Axes.Add(new LinearAxis());
            // Color axis
            model.Axes.Add(new LinearColorAxis
            {
                Palette = OxyPalettes.Hot(200)
            });

            var rand = new Random();
            var data = new double[7, 5];
            for (int x = 0; x < 5; ++x)
            {
                for (int y = 0; y < 7; ++y)
                {
                    data[y, x] = rand.Next(0, 200) * (0.13 * (y + 1));
                }
            }

            var heatMapSeries = new HeatMapSeries
            {
                X0 = 0,
                X1 = 6,
                Y0 = 0,
                Y1 = 4,
                // XAxisKey = "WeekdayAxis",
                // YAxisKey = "CakeAxis",
                RenderMethod = HeatMapRenderMethod.Rectangles,
                LabelFontSize = 0.2, // neccessary to display the label
                Data = data,
            };

            model.Series.Add(heatMapSeries);
            this.MyModel = model;
        }

        public PlotModel MyModel { get; set; }


        CalculatingTask task { get; set; }

        // Commands
        public OpenSettingsFormCommand OpenSettingsFormCommand { get; set; }

        public InitTaskCommand InitTaskCommand { get; set; }

        public void OpenSettingsForm()
        {
            SettingsForm sf = new SettingsForm();
            sf.DataContext = this;
            sf.Show();
        }

        public void InitTask()
        {
            task.InitInverseTask(Hx, Hy, X0, Y0, Z0,
                MeasuredValues.ToArray(), MeasuredValues.Count,
                Xstart, Xend, XstepsAmount,
                Ystart, Yend, YstepsAmount,
                Zstart, Zend, ZstepsAmount,
                out int nodesSize, out int elemsSize
                );

            NodesSize = nodesSize;
            ElemsSize = elemsSize;

            //Nodes = new Value[NodesSize];
            task.GetNodes(Nodes, NodesSize);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Caller Member Name allows not to pass property name in the function,
        // it will be passed automatically
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        public int ElemsSize { get; set; }

        public int NodesSize { get; set; }
        public Value[] Nodes { get; set; }

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
    }
}

