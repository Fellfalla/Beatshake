using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Beatshake.Core
{
    public class Teachement
    {
        public Tuple<double, double, double> XCoefficients;
        public Tuple<double, double, double> YCoefficients;
        public Tuple<double, double, double> ZCoefficients;


        public static Teachement Create(double[] timesteps, double[] xValues, double[] yValues, double[] zValues)
        {
            var teachement = new Teachement();
            teachement.XCoefficients = DataAnalyzer.CalculateCoefficients(timesteps, xValues);
            teachement.YCoefficients = DataAnalyzer.CalculateCoefficients(timesteps, yValues);
            teachement.ZCoefficients = DataAnalyzer.CalculateCoefficients(timesteps, zValues);

            return teachement;
        }

        public Teachement Create(IEnumerable<double> timesteps, IEnumerable<double> xValues, IEnumerable<double> yValues, IEnumerable<double> zValues)
        {
            return Create(timesteps.ToArray(), xValues.ToArray(), yValues.ToArray(), zValues.ToArray());
        }
    }
}
