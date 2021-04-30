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
                            double alpha, double Pmin, double Pmax,
                            double firstAlpha, double alphaStep, double fittingProcentThreshold);

        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public void getGridInformation(IntPtr task, out GridInformation gridInformation);

        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public void getResultGrids(IntPtr task, [Out] Value[] nodes, [Out] double[] yLayers);

        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public void getMeasureGrids(IntPtr task, [Out] double[] xGrid, [Out] double[] yGrid);

        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr getDiscrepancyByY(IntPtr task, int yLayer, [Out] Value[] values);
        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr getDiscrepancyByX(IntPtr task, int xLayer, [Out] Value[] values);
        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr getMagneticInductionByY(IntPtr task, int yLayer, [Out] Value[] values);
        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr getMagneticInductionByX(IntPtr task, int xLayer, [Out] Value[] values);
        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr changeAlphaThings(IntPtr task, double alpha, double pmin, double pmax,
                double firstAlpha, double alphaStep, double fittingProcentThreshold);

        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr buildMatrix(IntPtr task);

        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr solveWithAlphaSetted(IntPtr task, [Out] FiniteElem[] elems);

        [DllImport("mct_direct.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public IntPtr solveWithAlphaFitting(IntPtr task, [Out] FiniteElem[] elems, out double alpha);

        #endregion

        #region PublicFunctionsForVM
        public void Init(InitParameters ip)
        {
            initInverseTask(task, ip.Hx, ip.Nx, ip.Hy, ip.Ny, new Value(ip.X0, ip.Y0, ip.Z0),
                      ip.MeasuredValues.ToArray(), ip.MeasuredValues.Count,
                      ip.Xstart, ip.Xend, ip.XstepsAmount,
                      ip.Ystart, ip.Yend, ip.YstepsAmount,
                      ip.Zstart, ip.Zend, ip.ZstepsAmount,
                      ip.Alpha, ip.Pmin, ip.Pmax,
                      ip.FirstAlpha, ip.AlphaStep, ip.FittingProcentThreshold);

            getGridInformation(task, out gridInfo);

            // initialize grids:
            Nodes = new Value[GridInfo.pointsSize];
            YResultGridLayers = new double[GridInfo.yResultsLayersSize];
            getResultGrids(task, Nodes, YResultGridLayers);

            XMeasureGrid = new double[GridInfo.xMeasureLayersSize];
            DiscrepancyValuesByX = new Value[GridInfo.yMeasureLayersSize];
            DiscrepancyValuesByY = new Value[GridInfo.xMeasureLayersSize];
            MagnIndValuesByX = new Value[GridInfo.yMeasureLayersSize];
            MagnIndValuesByY = new Value[GridInfo.xMeasureLayersSize];
            YMeasureGrid = new double[GridInfo.yMeasureLayersSize];
            getMeasureGrids(task, XMeasureGrid, YMeasureGrid);

            // allocate memory for FE
            FiniteElems = new FiniteElem[GridInfo.elemsSize];
        }

        public void ChangeAlpha(InitParameters ip)
        {
            changeAlphaThings(task, ip.Alpha, ip.Pmin, ip.Pmax,
                      ip.FirstAlpha, ip.AlphaStep, ip.FittingProcentThreshold);
        }

        public void BuildMatrix()
        {
            buildMatrix(task);
        }

        public void SolveWithAlphaSetted()
        {
            solveWithAlphaSetted(task, FiniteElems);
        }

        public void SolveWithAlphaFitting()
        {

            solveWithAlphaFitting(task, FiniteElems, out double fittedAlpha);
            FittedAlpha = fittedAlpha;
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
        public List<List<Value>> ResultsValues
        {
            get
            {
                int elemsInXY = GridInfo.elemsInX * GridInfo.elemsInY;
                List<List<Value>> resultsValues = new List<List<Value>>();
                for (int zi = 0; zi < GridInfo.elemsInZ; zi++)
                {
                    var values = new List<Value>();
                    for (int xi = 0; xi < GridInfo.elemsInX; xi++)
                    {
                        values.Add(FiniteElems[zi * elemsInXY + YResultLayerIndex * GridInfo.elemsInX + xi].P);
                    }

                    resultsValues.Add(values);
                };
                return resultsValues;
            }
        }

        private GridInformation gridInfo;
        public GridInformation GridInfo { get => gridInfo; }

        public double FittedAlpha { get; set; }

        public double[] YResultGridLayers { get; set; }

        public Value[] DiscrepancyValuesByX { get; private set; }
        public Value[] DiscrepancyValuesByY { get; private set; }

        public Value[] MagnIndValuesByY { get; private set; }
        public Value[] MagnIndValuesByX { get; private set; }

        public double[] YMeasureGrid { get; private set; }
        public double[] XMeasureGrid { get; private set; }

        // Finite elems and coordinate points:
        public FiniteElem[] FiniteElems { get; set; }
        public Value[] Nodes { get; set; }
    }
}
