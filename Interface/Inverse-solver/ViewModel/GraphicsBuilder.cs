using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;

namespace Inverse_solver.ViewModel
{
    public class GraphicsBuilder
    {
        public PlotModel buildHeatmap(List<double> xGrid, List<double> zGrid, List<List<double>> resultsValues)
        {
            var model = new PlotModel { Title = "Results" };

            // Color axis
            model.Axes.Add(new LinearColorAxis
            {
                Palette = OxyPalettes.Hot(200)
            });

            // Display values source:
            var data = new double[xGrid.Count - 1, zGrid.Count - 1];
            for (int z = 0; z < zGrid.Count - 1; z++)
            {
                for (int x = 0; x < xGrid.Count - 1; x++)
                {
                    data[x, z] = resultsValues[z][x];
                }
            }

            double dx = xGrid[1] - xGrid[0];
            double dz = zGrid[1] - zGrid[0];

            var heatMapSeries = new HeatMapSeries
            {
                X0 = xGrid[0] + dx,
                X1 = xGrid[xGrid.Count - 1] - dx,
                Y0 = zGrid[0] + dz,
                Y1 = zGrid[zGrid.Count - 1] - dz,
                RenderMethod = HeatMapRenderMethod.Rectangles,
                LabelFontSize = 0.2, // neccessary to display the label
                Data = data,
            };

            model.Series.Add(heatMapSeries);
            return model;
        }

        public PlotModel buildDiscrepancyGraph(List<double> x, List<double> fx)
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

            for (int i = 0; i < x.Count; i++)
            {
                line1.Points.Add(new OxyPlot.DataPoint(x[i], fx[i]));
            }

            model.Series.Add(line1);
            return model;
        }
    }
}
