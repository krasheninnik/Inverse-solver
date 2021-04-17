using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inverse_solver.Model
{
    public class InverseTask
    {
        // ptr on the Task class, allocated in C++ code
        private readonly IntPtr task;
        public InverseTask() { task = createTask();}

        ~InverseTask() { deleteTask(task); }

        //  [DllImport("inverseSolverDLL\\Debug\\inverseSolverDLL.dll", CallingConvention = CallingConvention.Cdecl)]

        #region DLLImports
        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr createTask();

        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public void deleteTask(IntPtr task);        // IntPtr

        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public void initInverseTask(IntPtr task, double hx, int nx, double hy, int ny,
                            Value v0, Value[] measuredValues, int measuredValuesSize,
                            double xStart, double xEnd, int xStepsAmount,
                            double yStart, double yEnd, int yStepsAmount,
                            double zStart, double zEnd, int zStepsAmount,
                            double alpha);

        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public void getGridInformation(IntPtr task, out GridInformation gridInformation);

        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public void getResultGrids(IntPtr task, [Out] Value[] nodes, [Out] double[] yLayers);

        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public void getMeasureGrids(IntPtr task, [Out] double[] xGrid, [Out] double[] yGrid);


        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr solveTask(IntPtr task, [Out] FiniteElem[] elems);


        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr getDiscrepancy(IntPtr task, int yLayer, [Out] double[] values);
        #endregion

        #region PublicFunctionsForVM
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

            getGridInformation(task, out gridInfo);

            // initialize grids:
            Nodes = new Value[GridInfo.pointsSize];
            YResultGridLayers = new double[GridInfo.yResultsLayersSize];
            getResultGrids(task, Nodes, YResultGridLayers);

            XMeasureGrid = new double[GridInfo.xMeasureLayersSize];
            DiscrepancyValues = new double[GridInfo.xMeasureLayersSize];
            YMeasureGridLayers = new double[GridInfo.yMeasureLayersSize];
            getMeasureGrids(task, XMeasureGrid, YMeasureGridLayers);

            // allocate memory for FE
            FiniteElems = new FiniteElem[GridInfo.elemsSize];
        }

        public void CalculateTask()
        {
            solveTask(task, FiniteElems);

            // for sake of debug:
            //for (int i = 0; i < FiniteElems.Length; i++) FiniteElems[i].P = new Value(i, i, i);
        }

        public void GetDiscrepancy(int yLayer)
        {
            getDiscrepancy(task, yLayer, DiscrepancyValues);
        }
        #endregion

        public int YResultLayerIndex { get; set; }
        public List<List<double>> ResultsValues
        {
            get
            {
                int elemsInXY = GridInfo.elemsInX * GridInfo.elemsInY;
                List<List<double>>  resultsValues = new List<List<double>>();
                for (int zi = 0; zi < GridInfo.elemsInZ; zi++)
                {
                    var values = new List<double>();
                    for (int xi = 0; xi < GridInfo.elemsInX; xi++)
                    {
                        values.Add(FiniteElems[zi * elemsInXY + YResultLayerIndex * GridInfo.elemsInX + xi].P.Z);
                    }

                    resultsValues.Add(values);
                };
                return resultsValues;
            }
        }

        private GridInformation gridInfo;
        public GridInformation GridInfo { get => gridInfo; }

        public double[] YResultGridLayers { get; set; }

        public double[] XMeasureGrid { get; private set; }

        public double[] DiscrepancyValues { get; private set; }

        public double[] YMeasureGridLayers { get; private set; }  

        // Finite elems and coordinate points:
        public FiniteElem[] FiniteElems { get; set; }
        public Value[] Nodes { get; set; }
    }
}
