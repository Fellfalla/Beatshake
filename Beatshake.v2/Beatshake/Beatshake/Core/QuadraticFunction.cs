using System;

namespace Beatshake.Core
{
    [Obsolete("Use  PolynomialFunction instead")]
    public class QuadraticFunction : PolynomialFunction
    {
        public QuadraticFunction()
        {
            Coefficients = new double[3];
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
