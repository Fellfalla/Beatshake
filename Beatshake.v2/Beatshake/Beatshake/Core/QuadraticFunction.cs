using System;

namespace Beatshake.Core
{
    public class QuadraticFunction : PolynomialFunction
    {
        public QuadraticFunction()
        {
            Degree = 2;
        }

        /// <summary>
        /// First item stands in front of x^2 second before x and last is const.
        /// </summary>
        public new Tuple<double, double, double> Coefficients

        {
            get { return Tuple.Create(A,B,C); }
            set
            {
                if (value != null)
                {
                    A = value.Item1;
                    B = value.Item2;
                    C = value.Item3;
                }
                else
                {
                    A = double.NaN;
                    B = double.NaN;
                    C = double.NaN;
                }
            }
        }

        public double A
        {
            get { return base.Coefficients[2]; }
            set { base.Coefficients[2] = value; }
        }

        public double B
        {
            get { return base.Coefficients[1]; }
            set { base.Coefficients[1] = value; }
        }

        public double C
        {
            get { return base.Coefficients[0]; }
            set { base.Coefficients[0] = value; }
        }

        public double Start { get; set; }

        public double End { get; set; }

    }
}
