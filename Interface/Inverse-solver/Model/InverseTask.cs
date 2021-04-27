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
        public InverseTask() { task = createTask(); }

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
        static extern public IntPtr getDiscrepancyByY(IntPtr task, int yLayer, [Out] double[] values);
        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr getDiscrepancyByX(IntPtr task, int xLayer, [Out] double[] values);
        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr getMagneticInductionByY(IntPtr task, int yLayer, [Out] double[] values);
        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr getMagneticInductionByX(IntPtr task, int xLayer, [Out] double[] values);
        #endregion

        #region PublicFunctionsForVM
        public void Init(InitParameters ip)
        {
            initInverseTask(task, ip.Hx, ip.Nx, ip.Hy, ip.Ny, new Value(ip.X0, ip.Y0, ip.Z0),
                      ip.MeasuredValues.ToArray(), ip.MeasuredValues.Count,
                      ip.Xstart, ip.Xend, ip.XstepsAmount,
                      ip.Ystart, ip.Yend, ip.YstepsAmount,
                      ip.Zstart, ip.Zend, ip.ZstepsAmount,
                      ip.Alpha);

            getGridInformation(task, out gridInfo);

            // initialize grids:
            Nodes = new Value[GridInfo.pointsSize];
            YResultGridLayers = new double[GridInfo.yResultsLayersSize];
            getResultGrids(task, Nodes, YResultGridLayers);

            XMeasureGrid = new double[GridInfo.xMeasureLayersSize];
            DiscrepancyValuesByX = new double[GridInfo.yMeasureLayersSize];
            DiscrepancyValuesByY = new double[GridInfo.xMeasureLayersSize];
            MagnIndValuesByX = new double[GridInfo.yMeasureLayersSize];
            MagnIndValuesByY = new double[GridInfo.xMeasureLayersSize];
            YMeasureGrid = new double[GridInfo.yMeasureLayersSize];
            getMeasureGrids(task, XMeasureGrid, YMeasureGrid);

            // allocate memory for FE
            FiniteElems = new FiniteElem[GridInfo.elemsSize];
        }

        public void CalculateTask()
        {
            solveTask(task, FiniteElems);

            // for sake of debug:
            //for (int i = 0; i < FiniteElems.Length; i++) FiniteElems[i].P = new Value(i, i, i);
        }

        public void GetDiscrepancyByY(int yLayer)
        {
            getDiscrepancyByY(task, yLayer, DiscrepancyValuesByY);
        }

        public void GetDiscrepancyByX(int xLayer)
        {
            getDiscrepancyByX(task, xLayer, DiscrepancyValuesByX);
        }

        public void GetMagneticInductionByY(int yLayer)
        {
            getMagneticInductionByY(task, yLayer, MagnIndValuesByY);
        }

        public void GetMagneticInductionByX(int xLayer)
        {
            getMagneticInductionByX(task, xLayer, MagnIndValuesByX);
        }
        #endregion

        public int YResultLayerIndex { get; set; }
        public List<List<double>> ResultsValues
        {
            get
            {
                int elemsInXY = GridInfo.elemsInX * GridInfo.elemsInY;
                List<List<double>> resultsValues = new List<List<double>>();
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

        public double[] DiscrepancyValuesByX { get; private set; }
        public double[] DiscrepancyValuesByY { get; private set; }

        public double[] MagnIndValuesByY { get; private set; }
        public double[] MagnIndValuesByX { get; private set; }

        public double[] YMeasureGrid { get; private set; }
        public double[] XMeasureGrid { get; private set; }

        // Finite elems and coordinate points:
        public FiniteElem[] FiniteElems { get; set; }
        public Value[] Nodes { get; set; }
    }
}
