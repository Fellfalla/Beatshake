using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beatshake.Core
{
    public class QuadraticFunction
    {

        public Tuple<double, double, double> Coefficients

        {
            get { return Tuple.Create(A,B,C); }
            set
            {
                if (value != null)
                {
                    A = value.Item1;
                    B = value.Item1;
                    C = value.Item1;
                }
                else
                {
                    A = double.NaN;
                    B = double.NaN;
                    C = double.NaN;
                }
            }
        }

        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public double Start { get; set; }

        public double End { get; set; }

    }
}
