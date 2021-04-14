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
        static extern public int test1();

        [DllImport("C:\\Users\\Krash\\source\\repos\\inverseSolverDLL\\Debug\\inverseSolverDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public int test2(int a, int b);

        [DllImport("C:\\Users\\Krash\\source\\repos\\inverseSolverDLL\\Debug\\inverseSolverDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern public int test3(InitParameters p);

        public static int GetSumOf(int a, int b) => test2(a,b);
        public void Test3(InitParameters p) => test3(p);
    }
}
