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
using Xamarin.Forms;

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
        private double _teachementTolerance;
        private readonly List<double> XHistory = new List<double>(); 
        private readonly List<double> YHistory = new List<double>(); 
        private readonly List<double> ZHistory = new List<double>(); 
        private readonly List<long> Timestamps = new List<long>(); 
        private readonly Stopwatch _timeElapsedStopwatch = new Stopwatch();
        private uint _responseTime;
        private bool _useFunctionAnalysis;
        private double _lastGradient;
        private bool _useRandom;

        public override async void ProcessMotionData(IMotionDataProvider motionDataProvider)
        {
            Timestamps.Add((long) motionDataProvider.Acceleration.Timestamp);
            XHistory.Add(motionDataProvider.Acceleration.Trans[0]);
            YHistory.Add(motionDataProvider.Acceleration.Trans[1]);
            ZHistory.Add(motionDataProvider.Acceleration.Trans[2]);
            var cap = BeatshakeSettings.SamplePoints;
            var tasks = new List<Task>();
            var tooMuch = XHistory.Count - cap;
            if (tooMuch > 0) // todo: always remove 1, becaause we know, that we always add 1 element
            {
                XHistory.RemoveRange(0, tooMuch);
                YHistory.RemoveRange(0, tooMuch);
                ZHistory.RemoveRange(0, tooMuch);
                Timestamps.RemoveRange(0, tooMuch);
            }
            // todo: use Strategy pattern
            if (UseTeachement)
            {
                var normalizedTimestamps = Utility.NormalizeTimeStamps(Timestamps);
                var xCoeff = new PolynomialFunction(normalizedTimestamps, XHistory);
                var yCoeff = new PolynomialFunction(normalizedTimestamps, YHistory);
                var zCoeff = new PolynomialFunction(normalizedTimestamps, ZHistory);

                var teachedOnes = Components.Where(component => component.Teachement != null).ToArray();
                foreach (var instrumentalComponent in teachedOnes)
                {
                    var result = instrumentalComponent.Teachement.FitsDataSet(TeachementTolerance,
                       normalizedTimestamps.Last(), 0, true, xCoeff, yCoeff, zCoeff); // todo: Add Setting for normalizing
                    if (result)
                    {
                        var task = instrumentalComponent.PlaySoundCommand.Execute();
                        tasks.Add(task);
                    }
                }
                Task.WaitAll(tasks.ToArray());
                //Parallel.ForEach(teachedOnes, async instrumentalComponent =>
                //{
                //    var result = instrumentalComponent.Teachement.FitsDataSet(TeachementTolerance,
                //        normalizedTimestamps.Last(), 0, true, xCoeff, yCoeff, zCoeff); // todo: Add Setting for normalizing
                //    if (result)
                //    {
                //        await instrumentalComponent.PlaySoundCommand.Execute();
                //        //tasks.Add(task);
                //    }
                //});
            }
            else if (UseFunctionAnalysis)
            {
                var xFunction = new QuadraticFunction();
                var yFunction = new QuadraticFunction();
                var zFunction = new QuadraticFunction();

                // Get Coefficients
                var normaliedTimestamps = Utility.NormalizeTimeStamps(Timestamps);
                xFunction.Coefficients = DataAnalyzer.CalculateCoefficients(normaliedTimestamps, XHistory);
                yFunction.Coefficients = DataAnalyzer.CalculateCoefficients(normaliedTimestamps, YHistory);
                zFunction.Coefficients = DataAnalyzer.CalculateCoefficients(normaliedTimestamps, ZHistory);

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
                    await Components.Random().PlaySoundCommand.Execute();
                }

                _lastGradient = absGrad;
            }
            else if (UseRandom)
            {
                if (motionDataProvider.Acceleration.Trans.Any(d => d > 1))
                {
                    await Components.Random().PlaySoundCommand.Execute();
                }
            }
           
            
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
