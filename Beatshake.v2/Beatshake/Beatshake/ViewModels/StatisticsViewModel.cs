using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.Core;
using Beatshake.DependencyServices;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Xamarin.Forms;
using Prism.Navigation;
using Xamarin.Forms;

namespace Beatshake.ViewModels
{
    public class StatisticsViewModel : MeasureViewModelBase
    {
        private double _xAccel;
        private double _yAccel;
        private double _zAccel;
        private double _measureTimeJitter;
        private Stopwatch _stopwatch = new Stopwatch();
        private long _measurePoints = 0;
        private List<long> _timeMeasurements = new List<long>();
        private double _averageMeasureTime;
        private PlotModel _measureData;

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

        public PlotModel MeasureData
        {
            get { return _measureData; }
            set { SetProperty(ref _measureData, value); }
        }

        public PlotModel TeachedData
        {
            get { return _measureData; }
            set { SetProperty(ref _measureData, value); }
        }



        public StatisticsViewModel(INavigationService navigationService, IMotionDataProvider motionDataProvider) : base(navigationService, motionDataProvider)
        {
            var model = new PlotModel() {Title = "Continous Measure Data"};
            model.Axes.Add(new LinearAxis() {Position = AxisPosition.Bottom});
            model.Axes.Add(new LinearAxis() {Position = AxisPosition.Left});
            var lineSeries = new LineSeries { Title = "LineSeries", MarkerType = MarkerType.Circle };
            lineSeries.Points.Add(new DataPoint(0, 0));
            lineSeries.Points.Add(new DataPoint(10, 18));
            lineSeries.Points.Add(new DataPoint(20, 12));
            lineSeries.Points.Add(new DataPoint(30, 8));
            lineSeries.Points.Add(new DataPoint(40, 15));

            model.Series.Add(lineSeries);
            MeasureData = model;
        }

        public override void ProcessMotionData(IMotionDataProvider motionDataProvider)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {



                if (_stopwatch.IsRunning)
                {
                    _measurePoints++;
                    _stopwatch.Stop();
                    _timeMeasurements.Add(_stopwatch.ElapsedMilliseconds);
                    AverageMeasureTime = _timeMeasurements.Average();
                    MeasureTimeJitter = _timeMeasurements.Max() - _timeMeasurements.Min();
                    XAccel = MotionDataProvider.RelAcceleration.XTrans[0];
                    YAccel = MotionDataProvider.RelAcceleration.YTrans[0];
                    ZAccel = MotionDataProvider.RelAcceleration.ZTrans[0];
                }
                _stopwatch.Restart();
            });
        }
    }
}
