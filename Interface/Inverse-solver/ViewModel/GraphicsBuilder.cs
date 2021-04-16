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

            // Color axis
            model.Axes.Add(new LinearColorAxis
            {
                Palette = OxyPalettes.Hot(200)
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
                X0 = gridInformation.xStart + gridInformation.dx,
                X1 = gridInformation.xEnd - gridInformation.dx,
                Y0 = gridInformation.zStart + gridInformation.dz,
                Y1 = gridInformation.zEnd - gridInformation.dz,
                RenderMethod = HeatMapRenderMethod.Rectangles,
                LabelFontSize = 0.2, // neccessary to display the label
                Data = data,
            };

            model.Series.Add(heatMapSeries);
            return model;
        }

        public PlotModel buildDiscrepancyGraph(double[] x, double[]  fx)
        {
            // create the model and add the lines to it
            var model = new OxyPlot.PlotModel
            {
                Title = $"Discrepancy graph"

            };
            var line1 = new OxyPlot.Series.LineSeries()
            {
                Title = $"Discrepancy",
                Color = OxyPlot.OxyColors.Red,
                StrokeThickness = 1,
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
