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
        private bool _testTeachement;
        private double _teachementTolerance;
        private readonly List<double> XHistory = new List<double>(); 
        private readonly List<double> YHistory = new List<double>(); 
        private readonly List<double> ZHistory = new List<double>(); 
        private readonly List<long> Timestamps = new List<long>(); 
        private readonly Stopwatch _timeElapsedStopwatch = new Stopwatch();
        private uint _responseTime;


        public override async void ProcessMotionData(IMotionDataProvider motionDataProvider)
        {
            Timestamps.Add(_timeElapsedStopwatch.ElapsedMilliseconds);
            XHistory.Add(motionDataProvider.Acceleration.Trans[0]);
            YHistory.Add(motionDataProvider.Acceleration.Trans[1]);
            ZHistory.Add(motionDataProvider.Acceleration.Trans[2]);

            if (TestTeachement)
            {
                var choosenOne = Components.Where(component => component.Teachement != null).Random();
                if (choosenOne == null) // Noone has been teched
                {
                    await Components.Random().PlaySoundCommand.Execute();
                    return;
                }
                var cap = (int) (2000/BeatshakeSettings.SensorRefreshInterval);

                var tooMuch = XHistory.Count - cap;
                if (tooMuch > 0) // todo: always remove 1, becaause we know, that we always add 1 element
                {
                    XHistory.RemoveRange(0, tooMuch);
                    YHistory.RemoveRange(0, tooMuch);
                    ZHistory.RemoveRange(0, tooMuch);
                    Timestamps.RemoveRange(0, tooMuch);
                }


                var normaliedTimestamps = Timestamps.Select(l =>(double) l - Timestamps.First()).ToList();
                var xCoeff = DataAnalyzer.CalculateCoefficients(normaliedTimestamps,XHistory );
                var yCoeff = DataAnalyzer.CalculateCoefficients(normaliedTimestamps,YHistory );
                var zCoeff = DataAnalyzer.CalculateCoefficients(normaliedTimestamps,ZHistory );
                var start = normaliedTimestamps.First();
                var end = normaliedTimestamps.Last();
                var integral =  GetIntegralDifference(choosenOne.Teachement.XCoefficients, xCoeff, start, end);
                integral +=     GetIntegralDifference(choosenOne.Teachement.YCoefficients, yCoeff, start, end);
                integral +=     GetIntegralDifference(choosenOne.Teachement.ZCoefficients, zCoeff, start, end);
                if (AreFunctionsAlmostEqual(integral))
                {
                    await choosenOne.PlaySoundCommand.Execute();
                }
            }

            else if (motionDataProvider.Acceleration.Trans.Any(d => d > 1))
            {
                await Components.Random().PlaySoundCommand.Execute();

            }
            
        }

        private bool AreFunctionsAlmostEqual(double integral)
        {
            return integral < TeachementTolerance;
        }

        private double GetIntegralDifference(Tuple<double, double, double> func1, Tuple<double, double, double> func2, double start, double end)
        {
            double integral = 0; // this integral means the speed difference
            int precision = 100;
            double delta = (end - start) / precision;
            for (double x = start; x < end; x += precision)
            {
                var diff = (func1.Item1 * x * x + func1.Item2 * x + func1.Item3) -
                           (func2.Item1 * x * x + func2.Item2 * x + func2.Item3);
                integral += Math.Abs(diff) * delta;
            }

            return integral;
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

        public bool TestTeachement
        {
            get { return _testTeachement; }
            set { SetProperty(ref _testTeachement, value); }
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
