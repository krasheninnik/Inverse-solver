﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inverse_solver.Model
{
    public class Value
    {
        public Value()
        {

        }
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
