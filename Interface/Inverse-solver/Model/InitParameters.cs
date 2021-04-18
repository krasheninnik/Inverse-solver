using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inverse_solver.Model
{
    // Class for read JSON file and init task from it
    public class InitParameters
    {
        // Task settings props:
        // For measures grid:
        public int Hx { get; set; }
        public int Hy { get; set; }
        public int Nx { get; set; }
        public int Ny { get; set; }
        public double Alpha { get; set; }

        public double X0 { get; set; }
        public double Y0 { get; set; }
        public double Z0 { get; set; }

        // For Measures:
        public List<Value> MeasuredValues { get; set; }

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
