using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inverse_solver.Model
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InitParameters
    {
        public InitParameters(int hx, int hy, double x0, double y0,
            double z0, List<Value> measuredValues,
            double xStart, double xEnd, int xStepsAmount,
            double yStart, double yEnd, int yStepsAmount,
            double zStart, double zEnd, int zStepsAmount)
        {
            Hx = hx; 
            Hy = hy; 
            X0 = x0; 
            Y0 = y0; 
            Z0 = z0; 
            // For Measures:
            //MeasuredValues = measuredValues.ToArray();
            //MeasuredValuesCount = measuredValues.Count();
            
            // For space grid:
            Xstart = xStart; 
            Xend = xEnd; 
            XstepsAmount = xStepsAmount; 
            Ystart = yStart; 
            Yend = yEnd; 
            YstepsAmount = yStepsAmount; 
            Zstart = zStart; 
            Zend = zEnd; 
            ZstepsAmount = zStepsAmount; 
        }

        public int Hx { get; set; }
        public int Hy { get; set; }
        public double X0 { get; set; }
        public double Y0 { get; set; }
        public double Z0 { get; set; }
        // For Measures:
        //public Value[] MeasuredValues { get; set; }
        //public int MeasuredValuesCount { get; set; }
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
