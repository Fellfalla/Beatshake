using System;

namespace Beatshake.Core
{
    public class QuadraticFunction : PolynomialFunction
    {
        public QuadraticFunction()
        {
            Degree = 2;
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


    }
}
