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

        public DrumViewModel(INavigationService navigationService, IMotionDataProvider motionDataProvider, 
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
        }

        private string _heading = "Shake your Drums!";

        public DrumMode CurrentDrumMode
        {
            get { return _currentDrumMode; }
            set
            {
                if (value.HasFlag(DrumMode.None) && value.HasFlag(_currentDrumMode))
                {
                    // Having a flag, which is already set plus None means: Resetting to None
                    SetProperty(ref _currentDrumMode, DrumMode.None);
                }
                else if (value.HasFlag(DrumMode.None)) // meanwhile another value has been switched on
                {
                    return;
                }
                else
                {
                    SetProperty(ref _currentDrumMode, value);
                }
                ConfigureMotionDataProviderForMode(_currentDrumMode, MotionDataProvider);
            }
        }

        private static void ConfigureMotionDataProviderForMode(DrumMode mode, IMotionDataProvider provider)
        {
            provider.MotionDataNeeds = ModeNeeds[mode];
            return;

            switch (mode)
            {
                case DrumMode.FunctionAnalysis:
                case DrumMode.Random:
                    provider.MotionDataNeeds |= MotionData.RelAccelerationTrans;
                    break;
                case DrumMode.Jolt:
                    provider.MotionDataNeeds |= MotionData.JoltTrans;
                    break;
                case DrumMode.Position:
                    provider.MotionDataNeeds |= MotionData.PoseTrans;
                    break;
                case DrumMode.None:
                default:
                    provider.MotionDataNeeds = MotionData.None;
                    break;
            }
        }

        public string Heading
        {
            get { return _heading; }
            set { SetProperty(ref _heading, value); }
        }

        private ObservableCollection<InstrumentalComponent> _components;
        private string _title;
        private string _kit;
        private double _teachementTolerance = 50;
        private readonly List<double> _xHistory = new List<double>(); 
        private readonly List<double> _yHistory = new List<double>(); 
        private readonly List<double> _zHistory = new List<double>(); 
        private readonly List<long> _timestamps = new List<long>(); 
        private readonly Stopwatch _timeElapsedStopwatch = new Stopwatch();
        private uint _responseTime;
        private double _lastGradient;
        private double _cycleTime;
        private readonly Stopwatch _cycleStopwatch = new Stopwatch();
        private DrumMode _currentDrumMode;

        public double CycleTime
        {
            get { return _cycleTime; }
            set { SetProperty(ref _cycleTime, value); }
        }

        public bool Normalize { get; set; }

        public ObservableCollection<DrumMode> AvailableDrumModes
        {
            get
            {
                var collection = new ObservableCollection<DrumMode>();
                foreach (var mode in Enum.GetValues(typeof(DrumMode)))
                {
                    collection.Add((DrumMode)mode);
                }
                return collection;
            }
            set
            {
                
            }
        }

        public int MaxTeachementTolerance { get { return 150; } }

        public override async void ProcessMotionData(IMotionDataProvider motionDataProvider)
        {
            _cycleStopwatch.Start();
            _timestamps.Add((long) motionDataProvider.RelAcceleration.Timestamp);
            _xHistory.Add(motionDataProvider.RelAcceleration.Trans[0]);
            _yHistory.Add(motionDataProvider.RelAcceleration.Trans[1]);
            _zHistory.Add(motionDataProvider.RelAcceleration.Trans[2]);
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
                await TriggerOnTeachementMatch(activatedComponents);
            }
            else if (UseNeuralNetwork)
            {
                await TriggerOnNeuralNetworkMatch(activatedComponents);
            }
            else if (UsePosition)
            {
                await TriggerOnPositionMatch(activatedComponents);
            }
            else if (UseFunctionAnalysis)
            {
                await TriggerOnHighFunctionDerivation(activatedComponents);
            }

            else if (UseRandom)
            {
                if (motionDataProvider.RelAcceleration.Trans.Any(d => d > TeachementTolerance / 100))
                {
                    await activatedComponents.Random().PlaySoundCommand.Execute();
                }
            }
            CycleTime = _cycleStopwatch.ElapsedMilliseconds;
            _cycleStopwatch.Reset();
        }

        private async Task TriggerOnPositionMatch(InstrumentalComponent[] activatedComponents)
        {

            var teachedOnes = activatedComponents.Where(component => component.Teachement != null);
            var tasks = new List<Task>();
            foreach (var instrumentalComponent in teachedOnes)
            {
                // Full Teachement tolerance shall be 1 Meter -> Multiply by maxTeachement^-1 9,81^-1 10^6
                var transformedTolerance = TeachementTolerance*(10E5/(MaxTeachementTolerance*BeatshakeGlobals.G));
                if (instrumentalComponent.Teachement.FitsPositionData(transformedTolerance, MotionDataProvider))
                {
                    tasks.Add(instrumentalComponent.PlaySound());
                }
            }
            await Task.WhenAll(tasks);
        }

        private async Task TriggerOnTeachementMatch(InstrumentalComponent[] activatedComponents)
        {
            var normalizedTimestamps = Utility.NormalizeTimeStamps(_timestamps);
            var xCoeff = new PolynomialFunction(normalizedTimestamps, _xHistory);
            var yCoeff = new PolynomialFunction(normalizedTimestamps, _yHistory);
            var zCoeff = new PolynomialFunction(normalizedTimestamps, _zHistory);
            var group = new FunctionGroup(xCoeff, yCoeff, zCoeff);

            var teachedOnes = activatedComponents.Where(component => component.Teachement != null);
            var tasks = new List<Task>();
            foreach (var instrumentalComponent in teachedOnes)
            {
                var result = instrumentalComponent.Teachement.FitsDataSet(TeachementTolerance/10,
                    normalizedTimestamps.Last(), 0, ComparisonStrategy.PeakNormalized, @group);
                if (result)
                {
                    var task = instrumentalComponent.PlaySoundCommand.Execute();
                    tasks.Add(task);
                }
            }
            await Task.WhenAll(tasks);
        }
        private async Task TriggerOnNeuralNetworkMatch(InstrumentalComponent[] activatedComponents)
        {

            var teachedOnes = activatedComponents.Where(component => component.NeuralTeachement != null);
            var tasks = new List<Task>();
            foreach (var instrumentalComponent in teachedOnes)
            {
                var data = NeuralTeachement.TransformFunctionsToNetworkInputs(_xHistory, _yHistory, _zHistory);
                var result = instrumentalComponent.NeuralTeachement.FitsDataSet(TeachementTolerance/10, data);
                if (result)
                {
                    var task = instrumentalComponent.PlaySoundCommand.Execute();
                    tasks.Add(task);
                }
            }
            await Task.WhenAll(tasks);
        }

        private async Task TriggerOnHighFunctionDerivation(InstrumentalComponent[] activatedComponents)
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

            var gradX = 2*xFunction.A*end + xFunction.B;
            var gradY = 2*yFunction.A*end + yFunction.B;
            var gradZ = 2*zFunction.A*end + zFunction.B;

            var absGrad = Math.Abs(gradX) + Math.Abs(gradY) + Math.Abs(gradZ);

            if (absGrad < _lastGradient && absGrad > TeachementTolerance/200)
            {
                await activatedComponents.Random().PlaySoundCommand.Execute();
            }

            _lastGradient = absGrad;
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
            get { return CurrentDrumMode == DrumMode.Jolt; }
            set
            {
                if (value)
                {
                    CurrentDrumMode = DrumMode.Jolt;
                }
                else
                {
                    CurrentDrumMode = DrumMode.None;
                }
            }
        }

        public bool UseRandom
        {
            get { return CurrentDrumMode == DrumMode.Random; }
            set
            {
                if (value)
                {
                    CurrentDrumMode = DrumMode.Random;
                }
                else
                {
                    CurrentDrumMode = DrumMode.None;
                }
            }
        }

        public bool UsePosition
        {
            get { return CurrentDrumMode == DrumMode.Position; }
            set
            {
                if (value)
                {
                    //MotionDataProvider.MotionDataNeeds |= MotionData.PoseTrans;
                    CurrentDrumMode = DrumMode.Position;
                }
                else
                {
                    //MotionDataProvider.MotionDataNeeds ^= MotionData.PoseTrans;
                    CurrentDrumMode = DrumMode.None;
                }
            }
        }

        public bool UseNeuralNetwork
        {
            get { return CurrentDrumMode == DrumMode.NeuralNetwork; }
            set
            {
                if (value)
                {
                    CurrentDrumMode = DrumMode.NeuralNetwork;
                }
                else
                {
                    CurrentDrumMode = DrumMode.None;
                }
            }
        }

        public bool UseFunctionAnalysis 
        {
            get { return CurrentDrumMode == DrumMode.FunctionAnalysis; }
            set
            {
                if (value)
                {
                    CurrentDrumMode = DrumMode.FunctionAnalysis;
                }
                else
                {
                    CurrentDrumMode = DrumMode.None;
                }
            }
        }

        //private void ConcurrentOptionSet()
        //{
        //    UseTeachement = false;
        //    UseFunctionAnalysis = false;
        //    UseRandom = false;
        //    UsePosition = false;
        //}

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

        private static readonly Dictionary<DrumMode, MotionData> ModeNeeds = 
            new Dictionary<DrumMode, MotionData>()
            {
                { DrumMode.NeuralNetwork, MotionData.All },
                { DrumMode.None, MotionData.None },
                { DrumMode.Jolt, MotionData.JoltTrans },
                { DrumMode.Position, MotionData.PoseTrans },
                { DrumMode.FunctionAnalysis, MotionData.RelAccelerationTrans },
                { DrumMode.Random, MotionData.RelAccelerationTrans },
            };
    }

    

    [Flags]
    public enum DrumMode // todo: add Flags and allow additive combination of some modes
    {
        None = 1,
        Position = 2,
        Random = 4,
        FunctionAnalysis = 8,
        Jolt = 16,
        NeuralNetwork = 32,
    }

}
