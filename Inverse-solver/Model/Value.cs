using System.Runtime.InteropServices;

namespace Inverse_solver.Model
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Value
    {
        public Value(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }

}
