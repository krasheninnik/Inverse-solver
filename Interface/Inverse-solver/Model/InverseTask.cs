using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inverse_solver.Model
{
    public class InverseTask
    {
        // ptr on the Task class, allocated in C++ code
        private readonly IntPtr task;
        public InverseTask()
        {
            task = createTask();
        }

        ~InverseTask()
        {
            deleteTask(task);
        }



        //  [DllImport("inverseSolverDLL\\Debug\\inverseSolverDLL.dll", CallingConvention = CallingConvention.Cdecl)]

        [DllImport("C:\\Users\\Krash\\source\\repos\\Inverse-solver\\Solver\\mct_direct\\x64\\Debug\\mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr createTask();

         [DllImport("C:\\Users\\Krash\\source\\repos\\Inverse-solver\\Solver\\mct_direct\\x64\\Debug\\mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public void deleteTask(IntPtr task);        // IntPtr

         [DllImport("C:\\Users\\Krash\\source\\repos\\Inverse-solver\\Solver\\mct_direct\\x64\\Debug\\mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public void initInverseTask(IntPtr task, int hx, int nx, int hy, int ny,
                            Value v0, Value[] measuredValues, int measuredValuesSize,
                            double xStart, double xEnd, int xStepsAmount,
                            double yStart, double yEnd, int yStepsAmount,
                            double zStart, double zEnd, int zStepsAmount,
                            double alpha);

         [DllImport("C:\\Users\\Krash\\source\\repos\\Inverse-solver\\Solver\\mct_direct\\x64\\Debug\\mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public void getGridInforamtion(IntPtr task, out GridInformation gridInformation);

         [DllImport("C:\\Users\\Krash\\source\\repos\\Inverse-solver\\Solver\\mct_direct\\x64\\Debug\\mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public void getResultGrids(IntPtr task, [Out] Value[] nodes, [Out] double[] yLayers);

         [DllImport("C:\\Users\\Krash\\source\\repos\\Inverse-solver\\Solver\\mct_direct\\x64\\Debug\\mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public void getMeasureGrids(IntPtr task, [Out] double[] xGrid, [Out] double[] yGrid);


         [DllImport("C:\\Users\\Krash\\source\\repos\\Inverse-solver\\Solver\\mct_direct\\x64\\Debug\\mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr calculateTask(IntPtr task, [Out] FiniteElem[] elems);


         [DllImport("C:\\Users\\Krash\\source\\repos\\Inverse-solver\\Solver\\mct_direct\\x64\\Debug\\mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr getDiscrepancy(IntPtr task, int yLayer, [Out] double[] values);

        public void Init(int hx, int nx, int hy, int ny, Value p0Measure,
                            Value[] measuredValues, int measuredValuesSize,
                            double xStart, double xEnd, int xStepsAmount,
                            double yStart, double yEnd, int yStepsAmount,
                            double zStart, double zEnd, int zStepsAmount,
                            double alpha)
        {
          initInverseTask(task, hx, nx, hy, ny,
                      p0Measure, measuredValues, measuredValuesSize,
                      xStart, xEnd, xStepsAmount,
                      yStart, yEnd, yStepsAmount,
                      zStart, zEnd, zStepsAmount,
                      alpha);
        }

        public void GetGridInforamtion(out GridInformation gridInformation)
        {
            getGridInforamtion(task, out gridInformation);
        }

        public void GetResultGrids([Out] Value[] nodes, [Out] double[] yLayers)
        {
            getResultGrids(task, nodes, yLayers);
        }

        public void GetMeasureGrids([Out] double[] xGrid, [Out] double[] yGrid)
        {
            getMeasureGrids(task, xGrid, yGrid);
        }

        public void CalculateTask([Out] FiniteElem[] elems)
        {
            calculateTask(task, elems);
        }

        public void GetDiscrepancy(int yLayer, [Out] double[] values)
        {
            getDiscrepancy(task, yLayer, values);
        }

    }
}
