using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Beatshake.Core;
using Beatshake.DependencyServices;
using Beatshake.ExtensionMethods;
using Prism.Navigation;

namespace Beatshake.ViewModels
{


    public class DrumViewModel : InstrumentViewModelBase
    {

        public DrumViewModel(INavigationService navigationService, 
            IMotionDataProvider motionDataProvider, 
            InstrumentalComponentFactory componentFactory) : base(navigationService, motionDataProvider)
        {
            _timeElapsedStopwatch.Start();
            
            Components = new ObservableCollection<InstrumentalComponent>();
            Title = "DrumKit 1";
            Kit = "Kit1";

            componentFactory.Init(this, this, motionDataProvider);

            foreach (var allName in DrumComponentNames.GetAllNames())
            {
                Components.Add(componentFactory.CreateInstrumentalComponent(allName));
            }

            ResponseTime = BeatshakeSettings.SensorRefreshInterval;
            UseRandom = true;
        }

        private string _heading = "Shake your Drums!";

        public string Heading
        {
            get { return _heading; }
            set { SetProperty(ref _heading, value); }
        }

        private ObservableCollection<InstrumentalComponent> _components;
        private string _title;
        private string _kit;
        private bool _useTeachement;
        private double _teachementTolerance = 50;
        private readonly List<double> _xHistory = new List<double>(); 
        private readonly List<double> _yHistory = new List<double>(); 
        private readonly List<double> _zHistory = new List<double>(); 
        private readonly List<long> _timestamps = new List<long>(); 
        private readonly Stopwatch _timeElapsedStopwatch = new Stopwatch();
        private uint _responseTime;
        private bool _useFunctionAnalysis;
        private double _lastGradient;
        private bool _useRandom;
        private double _cycleTime;
        private readonly Stopwatch _cycleStopwatch = new Stopwatch();

        public double CycleTime
        {
            get { return _cycleTime; }
            set { SetProperty(ref _cycleTime, value); }
        }

        public bool Normalize { get; set; }

        public override async void ProcessMotionData(IMotionDataProvider motionDataProvider)
        {
            _cycleStopwatch.Start();
            _timestamps.Add((long) motionDataProvider.Acceleration.Timestamp);
            _xHistory.Add(motionDataProvider.Acceleration.Trans[0]);
            _yHistory.Add(motionDataProvider.Acceleration.Trans[1]);
            _zHistory.Add(motionDataProvider.Acceleration.Trans[2]);
            var cap = BeatshakeSettings.SamplePoints;
            var tooMuch = _xHistory.Count - cap;
            if (tooMuch > 0) // todo: always remove 1, becaause we know, that we always add 1 element
            {
                _xHistory.RemoveRange(0, tooMuch);
                _yHistory.RemoveRange(0, tooMuch);
                _zHistory.RemoveRange(0, tooMuch);
                _timestamps.RemoveRange(0, tooMuch);
            }

            var activatedComponents = Components.Where(component => component.IsActivated).ToArray();
            if (activatedComponents.Length == 0)
            {
                return;
            }

            // todo: use Strategy pattern
            if (UseTeachement)
            {
                var normalizedTimestamps = Utility.NormalizeTimeStamps(_timestamps);
                var xCoeff = new PolynomialFunction(normalizedTimestamps, _xHistory);
                var yCoeff = new PolynomialFunction(normalizedTimestamps, _yHistory);
                var zCoeff = new PolynomialFunction(normalizedTimestamps, _zHistory);

                var teachedOnes = activatedComponents.Where(component => component.Teachement != null);
                var tasks = new List<Task>();
                foreach (var instrumentalComponent in teachedOnes)
                {
                    var result = instrumentalComponent.Teachement.FitsDataSet(TeachementTolerance / 10,
                       normalizedTimestamps.Last(), 0, Normalize, xCoeff, yCoeff, zCoeff); // todo: Add Setting for normalizing
                    if (result)
                    {
                        var task = instrumentalComponent.PlaySoundCommand.Execute();
                        //var awaiter = task.ConfigureAwait(false);
                        //awaitables.Add(awaiter);
                        tasks.Add(task);
                    }
                }
                await Task.WhenAll(tasks);

            }
            else if (UseFunctionAnalysis)
            {
                var xFunction = new QuadraticFunction();
                var yFunction = new QuadraticFunction();
                var zFunction = new QuadraticFunction();

                // Get Coefficients
                var normaliedTimestamps = Utility.NormalizeTimeStamps(_timestamps);
                xFunction.Coefficients = DataAnalyzer.CalculateCoefficients(normaliedTimestamps, _xHistory);
                yFunction.Coefficients = DataAnalyzer.CalculateCoefficients(normaliedTimestamps, _yHistory);
                zFunction.Coefficients = DataAnalyzer.CalculateCoefficients(normaliedTimestamps, _zHistory);

                // Set start and Endpoints
                var start = normaliedTimestamps[0];
                var end = normaliedTimestamps.Last();
                xFunction.Start = start;
                xFunction.End = end;
                yFunction.Start = start;
                yFunction.End = end;
                zFunction.Start = start;
                zFunction.End = end;

                var gradX = 2 * xFunction.A * end + xFunction.B;
                var gradY = 2 * yFunction.A * end + yFunction.B;
                var gradZ = 2 * zFunction.A * end + zFunction.B;

                var absGrad = Math.Abs(gradX) + Math.Abs(gradY) + Math.Abs(gradZ);

                if (absGrad < _lastGradient && absGrad > TeachementTolerance / 200)
                {
                    await activatedComponents.Random().PlaySoundCommand.Execute();
                }

                _lastGradient = absGrad;
            }
            else if (UseRandom)
            {
                if (motionDataProvider.Acceleration.Trans.Any(d => d > TeachementTolerance / 100))
                {
                    await activatedComponents.Random().PlaySoundCommand.Execute();
                }
            }
            CycleTime = _cycleStopwatch.ElapsedMilliseconds;
            _cycleStopwatch.Reset();
        }

        public override string Kit
        {
            get { return _kit; }
            set
            {
                SetProperty(ref _kit, value);
                foreach (var instrumentalComponent in Components)
                {
                    instrumentalComponent.PreLoadAudio(); // the whole Kit has changed
                }
            }
        }


        public double TeachementTolerance
        {
            get { return _teachementTolerance; }
            set { SetProperty(ref _teachementTolerance, value); }
        }

        public ObservableCollection<InstrumentalComponent> Components
        {
            get { return _components; }
            set { SetProperty(ref _components, value); }
        }

        public bool UseTeachement
        {
            get { return _useTeachement; }
            set
            {
                if (value)
                {
                    ConcurrentOptionSet();
                }
                SetProperty(ref _useTeachement, value);
            }
        }

        public bool UseRandom
        {
            get { return _useRandom; }
            set
            {
                if (value)
                {
                    ConcurrentOptionSet();
                }
                SetProperty(ref _useRandom, value);
            }
        }

        public bool UseFunctionAnalysis 
        {
            get { return _useFunctionAnalysis; }
            set
            {
                if (value)
                {
                    ConcurrentOptionSet();
                }
                SetProperty(ref _useFunctionAnalysis, value);
            }
        }

        private void ConcurrentOptionSet()
        {
            UseTeachement = false;
            UseFunctionAnalysis = false;
            UseRandom = false;
        }

        public uint ResponseTime
        {
            get { return _responseTime; }
            set
            {
                SetProperty(ref _responseTime, value);
                if (MotionDataProvider != null) MotionDataProvider.RefreshRate = value;
            }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

    }
}
