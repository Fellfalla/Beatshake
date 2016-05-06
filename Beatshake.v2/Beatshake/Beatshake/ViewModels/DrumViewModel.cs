using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        public DrumViewModel(INavigationService navigationService, IMotionDataProvider motionDataProvider) : base(navigationService, motionDataProvider)
        {
            _timeElapsedStopwatch.Start();
            
            Components = new ObservableCollection<InstrumentalComponent>();
            Title = "DrumKit 1";
            Kit = "Kit1";
            foreach (var allName in DrumComponentNames.GetAllNames())
            {
                Components.Add(new InstrumentalComponent(this, this, motionDataProvider, allName));
            }
            ResponseTime = BeatshakeSettings.SensorRefreshInterval;
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

        public override async void ProcessMotionData(IMotionDataProvider motionDataProvider)
        {
            Timestamps.Add((long) motionDataProvider.Acceleration.Timestamp);
            XHistory.Add(motionDataProvider.Acceleration.Trans[0]);
            YHistory.Add(motionDataProvider.Acceleration.Trans[1]);
            ZHistory.Add(motionDataProvider.Acceleration.Trans[2]);
            var cap = Teachement.Settings.SamplePoints;

            var tooMuch = XHistory.Count - cap;
            if (tooMuch > 0) // todo: always remove 1, becaause we know, that we always add 1 element
            {
                XHistory.RemoveRange(0, tooMuch);
                YHistory.RemoveRange(0, tooMuch);
                ZHistory.RemoveRange(0, tooMuch);
                Timestamps.RemoveRange(0, tooMuch);
            }

            if (UseTeachement)
            {
                var choosenOne = Components.Where(component => component.Teachement != null).Random();
                if (choosenOne == null) // None has been teached yet
                {
                    //await Components.Random().PlaySoundCommand.Execute();
                    return;
                }
                //var cap = (int) (2000/BeatshakeSettings.SensorRefreshInterval);

                var normaliedTimestamps = Utility.NormalizeTimeStamps(Timestamps);
                var xCoeff = DataAnalyzer.CalculateCoefficients(normaliedTimestamps, XHistory );
                var yCoeff = DataAnalyzer.CalculateCoefficients(normaliedTimestamps, YHistory );
                var zCoeff = DataAnalyzer.CalculateCoefficients(normaliedTimestamps, ZHistory );
                var start = normaliedTimestamps.First();
                var end = normaliedTimestamps.Last();
                var integral = DataAnalyzer.GetIntegralDifference(choosenOne.Teachement.XCoefficients, xCoeff, start, end);
                integral += DataAnalyzer.GetIntegralDifference(choosenOne.Teachement.YCoefficients, yCoeff, start, end);
                integral += DataAnalyzer.GetIntegralDifference(choosenOne.Teachement.ZCoefficients, zCoeff, start, end);
                if (DataAnalyzer.AreFunctionsAlmostEqual(integral, TeachementTolerance, end - start))
                {
                    await choosenOne.PlaySoundCommand.Execute();
                }
            }
            if (UseFunctionAnalysis)
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

            else if (motionDataProvider.Acceleration.Trans.Any(d => d > 1))
            {
                await Components.Random().PlaySoundCommand.Execute();

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
            set { SetProperty(ref _useTeachement, value); }
        }

        public bool UseFunctionAnalysis 
        {
            get { return _useFunctionAnalysis; }
            set { SetProperty(ref _useFunctionAnalysis, value); }
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
