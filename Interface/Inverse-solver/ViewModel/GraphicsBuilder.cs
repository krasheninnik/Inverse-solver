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
        public PlotModel buildHeatmap(GridInformation gridInformation, List<List<double>> resultsValues)
        {
            var model = new PlotModel { Title = "Results" };

            var colorAxis = new LinearColorAxis
            {
                Position = AxisPosition.Right,
                Palette = OxyPalettes.Hot(200)
            };
            model.Axes.Add(colorAxis);

            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X coordinate, m" });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Z coordinate, m" });

            // Color axis
            model.Axes.Add(new LinearColorAxis
            {
                Palette = OxyPalettes.Hot(200)
                //Palette = OxyPalettes.Gray(200)          
            });

            // Display values source:
            var data = new double[gridInformation.elemsInX, gridInformation.elemsInZ];
            for (int z = 0; z < gridInformation.elemsInZ; z++)
            {
                for (int x = 0; x < gridInformation.elemsInX; x++)
                {
                    data[x, z] = resultsValues[z][x];
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

        public PlotModel buildDiscrepancyGraph(double[] x, double[] fx, string mode)
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

            for (int i = 0; i < x.Length; i++)
            {
                line1.Points.Add(new OxyPlot.DataPoint(x[i], fx[i]));
            }

            model.Series.Add(line1);
            return model;
        }

        public PlotModel buildMagneticInductionGraph(double[] x, double[] fx, string mode)
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

            for (int i = 0; i < x.Length; i++)
            {
                line1.Points.Add(new OxyPlot.DataPoint(x[i], fx[i]));
            }

            model.Series.Add(line1);
            return model;
        }
    }
}
