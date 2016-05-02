using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.Core;
using Beatshake.DependencyServices;
using Prism.Navigation;

namespace Beatshake.ViewModels
{
    public abstract class MeasureViewModelBase : BaseViewModel
    {
        protected IMotionDataProvider MotionDataProvider { get; set; }

        public MeasureViewModelBase(INavigationService navigationService, IMotionDataProvider motionDataProvider) : base(navigationService)
        {
            MotionDataProvider = motionDataProvider;
            motionDataProvider.MotionDataRefreshed += ProcessMotionData;
        }

        protected abstract void ProcessMotionData(IMotionDataProvider sender);

        protected Teachement TeachMovement()
        {
            MotionDataProvider.MotionDataRefreshed -= ProcessMotionData;

            // record movement
            Stopwatch stopwatch = new Stopwatch();

            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();
            List<double> zValues = new List<double>();
            List<double> timesteps = new List<double>();

            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < 2000)
            {
                timesteps.Add(stopwatch.ElapsedMilliseconds);
                xValues.Add(MotionDataProvider.Acceleration.Trans[0]);
                yValues.Add(MotionDataProvider.Acceleration.Trans[1]);
                zValues.Add(MotionDataProvider.Acceleration.Trans[2]);
                var task = Task.Delay((int) MotionDataProvider.RefreshRate);
                task.Wait();
            }
            stopwatch.Stop();
            var dataPoints = timesteps.ToArray();
            var teachement = new Teachement();

            teachement.XCoefficients = DataAnalyzer.CalculateCoefficients(dataPoints, xValues.ToArray());
            teachement.YCoefficients = DataAnalyzer.CalculateCoefficients(dataPoints, yValues.ToArray());
            teachement.ZCoefficients = DataAnalyzer.CalculateCoefficients(dataPoints, zValues.ToArray());

            MotionDataProvider.MotionDataRefreshed += ProcessMotionData;

            return teachement;
        }
    }
}
