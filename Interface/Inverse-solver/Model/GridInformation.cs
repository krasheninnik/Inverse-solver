using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inverse_solver.Model
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GridInformation
    {
        public int elemsInX;
        public int elemsInY;
        public int elemsInZ;
        public int elemsSize;
        public int pointsSize;
        public int yResultsLayersSize;
        public int yMeasureLayersSize;
        public int xMeasureLayersSize;
        public double dx;
        public double xStart;
        public double xEnd;
        public double dz;
        public double zStart;
        public double zEnd;
    }
}
