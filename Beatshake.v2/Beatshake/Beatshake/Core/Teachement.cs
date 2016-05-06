using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.DependencyServices;
using Beatshake.ExtensionMethods;
using Xamarin.Forms;

namespace Beatshake.Core
{
    public class Teachement
    {
        public static TeachementSettings Settings = new TeachementSettings();

        public Tuple<double, double, double> XCoefficients;
        public Tuple<double, double, double> YCoefficients;
        public Tuple<double, double, double> ZCoefficients;


        public static Teachement Create(double[] timesteps, double[] xValues, double[] yValues, double[] zValues)
        {
            var teachement = new Teachement();

            // Get point with highest absolute Acceleration
            int index;
            var xMax = DataAnalyzer.GetPeak(timesteps, new IList<double>[]{ xValues , yValues, zValues }, out index);

            if (index == -1)
            {
                throw new InvalidOperationException("No peak value could be detected.");
            }

            int teachStartPoint = Math.Max(index - Settings.SamplePoints, 0);

            teachement.XCoefficients = DataAnalyzer.CalculateCoefficients(timesteps.SubArray(teachStartPoint, index), xValues.SubArray(teachStartPoint, index));
            teachement.YCoefficients = DataAnalyzer.CalculateCoefficients(timesteps.SubArray(teachStartPoint, index), yValues.SubArray(teachStartPoint, index));
            teachement.ZCoefficients = DataAnalyzer.CalculateCoefficients(timesteps.SubArray(teachStartPoint, index), zValues.SubArray(teachStartPoint, index));

            return teachement;
        }

        public static Teachement Create(IEnumerable<double> timesteps, IEnumerable<double> xValues, IEnumerable<double> yValues, IEnumerable<double> zValues)
        {
                return Create(timesteps.ToArray(), xValues.ToArray(), yValues.ToArray(), zValues.ToArray());
        }
    }


    public class TeachementSettings
    {
        public TeachementSettings()
        {
            this.AssignDefaultValueAttributes();
        }

        [DefaultValue(5)]
        public int SamplePoints { get; set; }
    }
}
