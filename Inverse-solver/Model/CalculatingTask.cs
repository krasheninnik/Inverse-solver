using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inverse_solver.Model
{
    public class CalculatingTask
    {
        // IntPtr
        [DllImport("C:\\Users\\Krash\\source\\repos\\inverseSolverDLL\\Debug\\inverseSolverDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public void initInverseTask(int hx, int hy, double x0, double y0,
                            double z0, Value[] measuredValues, int measuredValuesSize,
                            double xStart, double xEnd, int xStepsAmount,
                            double yStart, double yEnd, int yStepsAmount,
                            double zStart, double zEnd, int zStepsAmount);

        public static int GetSumOf(int a, int b) => a + b;
        public void InitInverseTask(int hx, int hy, double x0, double y0,
                            double z0, Value[] measuredValues, int measuredValuesSize,
                            double xStart, double xEnd, int xStepsAmount,
                            double yStart, double yEnd, int yStepsAmount,
                            double zStart, double zEnd, int zStepsAmount)
        {
          initInverseTask( hx, hy, x0, y0,
                      z0, measuredValues, measuredValuesSize,
                      xStart, xEnd, xStepsAmount,
                      yStart, yEnd, yStepsAmount,
                      zStart, zEnd, zStepsAmount);
        }
    }
}
