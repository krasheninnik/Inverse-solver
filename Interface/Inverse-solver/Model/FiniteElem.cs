using System.Runtime.InteropServices;

namespace Inverse_solver.Model
{
	[StructLayout(LayoutKind.Sequential)]
	public struct FiniteElem {
        public FiniteElem( int n0, int n1,  int n2, int n3, int n4, int n5, int n6, int n7, Value p)
        {
			N0 = n0;
			N1 = n1;
			N2 = n2;
			N3 = n3;
			N4 = n4;
			N5 = n5;
			N6 = n6;
			N7 = n7;
			P = p;
		}

		// Map local point into global
		public int N0 { get; set; }
		public int N1 { get; set; }
		public int N2 { get; set; }
		public int N3 { get; set; }
		public int N4 { get; set; }
		public int N5 { get; set; }
		public int N6 { get; set; }
		public int N7 { get; set; }

		// Magnetization intensity
		public Value P { get; set; }
	}
}
