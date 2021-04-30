using Inverse_solver.Model;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;

namespace Inverse_solver.ViewModel
{
    public class GraphicsBuilder
    {
        private Func<Value, double> CreateExtractComponentFunction(string componentToShow)
        {
            // Extract result component to show
            Func<Value, double> extractComponent;
            switch (componentToShow)
            {
                case "X":
                    extractComponent = value => value.X;
                    break;
                case "Y":
                    extractComponent = value => value.Y;
                    break;
                case "Z":
                    extractComponent = value => value.Z;
                    break;
                default:
                    throw new Exception($"There no such ComponentToShow: {componentToShow}");
            }

            return extractComponent;
        }


        public PlotModel buildHeatmap(GridInformation gridInformation, List<List<Value>> resultsValues, string ResultComponentToShow)
        {
            var model = new PlotModel { Title = "Results" };

            // Extract result component to show:
            Func<Value, double> extractComponent = CreateExtractComponentFunction(ResultComponentToShow);

            // find min and max for this model:
            double max = extractComponent(resultsValues[0][0]);
            double min = extractComponent(resultsValues[0][0]);

            for(int i = 0; i < resultsValues.Count; i++)
            {
                for (int j = 0; j < resultsValues[i].Count; j++)
                {
                    double curComp = extractComponent(resultsValues[i][j]);
                    if (curComp > max) max = curComp;
                    if (curComp < min) min = curComp;
                }
            }

            // kind of output result correction
            max += 0.1;
            min -= 0.1;

            // Color axis
            var colorAxis = new LinearColorAxis
            {
                Position = AxisPosition.Right,
                Palette = OxyPalettes.Gray(200),
                Minimum = min,
                Maximum = max
            };
            model.Axes.Add(colorAxis);

            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X coordinate, m" });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Z coordinate, m" });

            // Display values source:
            var data = new double[gridInformation.elemsInX, gridInformation.elemsInZ];
            for (int z = 0; z < gridInformation.elemsInZ; z++)
            {
                for (int x = 0; x < gridInformation.elemsInX; x++)
                {
                    data[x, z] = extractComponent(resultsValues[z][x]);
                }
            }

            var heatMapSeries = new HeatMapSeries
            {
                X0 = gridInformation.xStart + gridInformation.dx / 2,
                X1 = gridInformation.xEnd - gridInformation.dx / 2,
                Y0 = gridInformation.zStart + gridInformation.dz / 2,
                Y1 = gridInformation.zEnd - gridInformation.dz / 2,
                RenderMethod = HeatMapRenderMethod.Rectangles,
                LabelFontSize = 0.2, // neccessary to display the label
                Data = data,
            };

            model.Series.Add(heatMapSeries);
            return model;
        }
        public PlotModel buildDiscrepancyGraph(double[] x, Value[] fx, string mode, string discrepancyComponentToShow)
        {
            // create the model and add the lines to it
            var model = new OxyPlot.PlotModel
            {
                Title = $"Discrepancy graph"
            };

            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = $"{mode} coordinate, m" });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Discrepancy, %" });

            var line1 = new OxyPlot.Series.LineSeries()
            {
                Title = $"Discrepancy",
                Color = OxyPlot.OxyColors.Red,
                StrokeThickness = 1,
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
            };


            // Extract result component to show:
            Func<Value, double> extractComponent = CreateExtractComponentFunction(discrepancyComponentToShow);

            for (int i = 0; i < x.Length; i++)
            {
                line1.Points.Add(new OxyPlot.DataPoint(x[i], extractComponent(fx[i])));
            }

            model.Series.Add(line1);
            return model;
        }

        public PlotModel buildMagneticInductionGraph(double[] x, Value[] fx, string mode, string magneticInductionComponentToShow)
        {
            // create the model and add the lines to it
            var model = new OxyPlot.PlotModel
            {
                Title = $"Magnetic Induction graph"
            };

            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = $"{mode} coordinate, m" });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Magnetic Induction, T" });

            var line1 = new OxyPlot.Series.LineSeries()
            {
                Title = $"Magnetic Induction",
                Color = OxyPlot.OxyColors.Red,
                StrokeThickness = 1,
                InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline,
            };

            // Extract result component to show:
            Func<Value, double> extractComponent = CreateExtractComponentFunction(magneticInductionComponentToShow);

            for (int i = 0; i < x.Length; i++)
            {
                line1.Points.Add(new OxyPlot.DataPoint(x[i], extractComponent(fx[i])));
            }

            model.Series.Add(line1);
            return model;
        }
    }
}
