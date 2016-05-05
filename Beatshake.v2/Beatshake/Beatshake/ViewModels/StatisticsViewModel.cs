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
    class StatisticsViewModel : MeasureViewModelBase
    {
        private double _xAccel;
        private double _yAccel;
        private double _zAccel;
        private double _measureTimeJitter;
        private Stopwatch _stopwatch = new Stopwatch();
        private long _measurePoints = 0;
        private List<long> _timeMeasurements = new List<long>();
        private double _averageMeasureTime;


        public double XAccel
        {
            get { return _xAccel; }
            set { SetProperty(ref _xAccel, value); }
        }

        public double YAccel
        {
            get { return _yAccel; }
            set { SetProperty(ref _yAccel, value); }
        }

        public double ZAccel
        {
            get { return _zAccel; }
            set { SetProperty(ref _zAccel, value); }
        }

        public double MeasureTimeJitter
        {
            get { return _measureTimeJitter; }
            set { SetProperty(ref _measureTimeJitter, value); }
        }

        public double AverageMeasureTime
        {
            get { return _averageMeasureTime; }
            set { SetProperty(ref _averageMeasureTime, value); }
        }


        public StatisticsViewModel(INavigationService navigationService, IMotionDataProvider motionDataProvider) : base(navigationService, motionDataProvider)
        {
        }

        public override void ProcessMotionData(IMotionDataProvider motionDataProvider)
        {
            if (_stopwatch.IsRunning)
            {
                _measurePoints++;
                _stopwatch.Stop();
                _timeMeasurements.Add(_stopwatch.ElapsedMilliseconds);
                AverageMeasureTime = _timeMeasurements.Average();
                MeasureTimeJitter = _timeMeasurements.Max() - _timeMeasurements.Min();
                XAccel = MotionDataProvider.Acceleration.Trans[0];
                YAccel = MotionDataProvider.Acceleration.Trans[1];
                ZAccel = MotionDataProvider.Acceleration.Trans[2];
            }
            _stopwatch.Restart();

        }
    }
}
