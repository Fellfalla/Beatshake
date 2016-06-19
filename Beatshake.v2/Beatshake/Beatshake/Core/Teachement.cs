using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Beatshake.DependencyServices;
using Beatshake.ExtensionMethods;

namespace Beatshake.Core
{
    public class Teachement : FunctionGroup
    {
        public PolynomialFunction XCurve
        {
            get { return Functions[0]; }
            set { Functions[0] = value; }
        }
        public PolynomialFunction YCurve
        {
            get { return Functions[1]; }
            set { Functions[1] = value; }
        }
        public PolynomialFunction ZCurve
        {
            get { return Functions[2]; }
            set { Functions[2] = value; }
        }

        public double XPosition { get; private set; }

        public double YPosition { get; private set; }

        public double ZPosition { get; private set; }

        /// <summary>
        /// <exception cref="InsufficientDataException">Thrown if a peak is to close to the start of the data</exception>
        /// </summary>
        /// <param name="timesteps"></param>
        /// <param name="throwOnThinData"></param>
        /// <param name="valueArrays"></param>
        /// <returns></returns>
        public static Teachement Create(double[] timesteps, bool throwOnThinData = false, params IList<double>[] valueArrays)
        {
            var teachement = new Teachement();

            IList<double> xValues = valueArrays[0];
            IList<double> yValues = valueArrays[1];
            IList<double> zValues = valueArrays[2];
            // Get point with highest absolute Acceleration
            var endIndex = DataAnalyzer.GetPeak(xValues , yValues, zValues);
            int startIndex = endIndex - BeatshakeSettings.SamplePoints;

            if (endIndex == -1)
            {
                throw new InvalidOperationException("No peak value could be detected.");
            }
            if (startIndex < 0)
            {
                if (throwOnThinData)
                {
                    throw new InsufficientDataException();
                }
                else
                {
                    startIndex = 0;
                }
            }
            teachement.Functions.Clear();
            teachement.Functions.Add(new PolynomialFunction(timesteps.SubArray(startIndex, endIndex),
                xValues.SubList(startIndex, endIndex)));
            teachement.Functions.Add(new PolynomialFunction(timesteps.SubArray(startIndex, endIndex),
                yValues.SubList(startIndex, endIndex)));
            teachement.Functions.Add(new PolynomialFunction(timesteps.SubArray(startIndex, endIndex),
                zValues.SubList(startIndex, endIndex)));


            return teachement;
        }


        public static async Task<Teachement> TeachMovement(IMotionDataProvider motionDataProvider)
        {

            // record movement
            Teachement teachement = null;

            List<double> xAcc = new List<double>();
            List<double> yAcc = new List<double>();
            List<double> zAcc = new List<double>();

            List<double> xPos = new List<double>();
            List<double> yPos = new List<double>();
            List<double> zPos = new List<double>();

            List<double> timesteps = new List<double>();
            var userConfirmation = Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Click OK when you finished teaching");

            var measureTask = Task.Factory.StartNew(() =>
            {
                while (!userConfirmation.IsCompleted)
                {
                    timesteps.Add(motionDataProvider.RelAcceleration.Timestamp);

                    // Set Accelerations
                    xAcc.Add(motionDataProvider.RelAcceleration.Trans[0]);
                    yAcc.Add(motionDataProvider.RelAcceleration.Trans[1]);
                    zAcc.Add(motionDataProvider.RelAcceleration.Trans[2]);

                    // Set Positions
                    xPos.Add(motionDataProvider.Pose.Trans[0]);
                    yPos.Add(motionDataProvider.Pose.Trans[1]);
                    zPos.Add(motionDataProvider.Pose.Trans[2]);

                    var task = Task.Delay((int)motionDataProvider.RefreshRate);
                    task.Wait();
                }
            });

            //userConfirmation.Wait(30000);
            await userConfirmation.ConfigureAwait(true);
            try
            {
                //Task.WaitAll(userConfirmation, measureTask);

                await measureTask;
            }
            catch (TaskCanceledException)
            {
            }

            try
            {
                var normalizedTimestamps = Utility.NormalizeTimeStamps(timesteps);
                teachement = Teachement.Create(normalizedTimestamps.ToArray(), false, xAcc, yAcc, zAcc);

                int peakPos = DataAnalyzer.GetPeak(xAcc, yAcc, zAcc);

                teachement.XPosition = xPos[peakPos];
                teachement.YPosition = yPos[peakPos];
                teachement.ZPosition = zPos[peakPos];
            }
            catch (InsufficientDataException) // thrown if the peak is to near at beginning data
            {
                await Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Teachement failed. Please try again. (longer)");
            }
            catch (InvalidOperationException)
            {
                await Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Click OK when you finished teaching");
            }


            return teachement;
        }

        ///// <summary>
        ///// <seealso cref="Create(double[],double[],double[],double[])"/>
        ///// </summary>
        ///// <param name="timesteps"></param>
        ///// <param name="xValues"></param>
        ///// <param name="yValues"></param>
        ///// <param name="zValues"></param>
        ///// <returns></returns>
        //public static Teachement Create(IEnumerable<double> timesteps, bool throwOnThinData = false, params IEnumerable<double>[] valueArrays)
        //{
        //        return Create(timesteps.ToArray(), throwOnThinData, valueArrays.Select(doubles => doubles.ToArray()).ToArray());
        //}


        public bool FitsDataSet(double tolerance, double start, double end, ComparisonStrategy strategy, FunctionGroup functions)
        {
            switch (strategy)
            {
                case ComparisonStrategy.Absolute: throw new NotImplementedException();
                case ComparisonStrategy.CoefficientNormalized: throw new NotImplementedException();
                case ComparisonStrategy.Position:

                case ComparisonStrategy.PeakNormalized:
                    functions.PeakNormalizeDownTo(this);
                    var difference = GetDifferenceIntegral(end, start, functions);
                    if (DataAnalyzer.AreFunctionsAlmostEqual(difference, tolerance, end - start))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
            return false;
        }

        public bool FitsPositionData(double tolerance, IMotionDataProvider motionData)
        {
            return motionData.Pose.Trans[0].IsAlmostEqual(XPosition, tolerance) &&
                   motionData.Pose.Trans[1].IsAlmostEqual(YPosition, tolerance) &&
                   motionData.Pose.Trans[2].IsAlmostEqual(ZPosition, tolerance);
        }
    }
}
