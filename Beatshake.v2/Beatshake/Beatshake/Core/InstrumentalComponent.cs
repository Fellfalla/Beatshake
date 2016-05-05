using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Beatshake.DependencyServices;
using Beatshake.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using Xamarin.Forms;
using DependencyService = Prism.Services.DependencyService;

namespace Beatshake.Core
{
    public class InstrumentalComponent : BindableBase, IInstrumentalComponentIdentification
    {
        private readonly IInstrumentPlayer _player;
        private double _xCoefficients;
        private double _yCoefficients;
        private double _zCoefficients;
        private object _audionInstance;
        private Teachement _teachement;

        private Cooldown cooldown = new Cooldown();

        public IInstrumentalIdentification ContainingInstrument
        {
            get { return _containingInstrument; }
            set
            {
                SetProperty(ref _containingInstrument, value);
            }
        }

        private IMotionDataProcessor MotionDataProcessor { get; set; }
        private IMotionDataProvider MotionDataProvider { get; set; }

        private DelegateCommand _teachCommand;
        public DelegateCommand TeachCommand
        {
            get { return _teachCommand; }
            set { SetProperty(ref _teachCommand, value); }
        }


        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
            }
        }

        [DefaultValue(1)]
        public int Number { get; set; }

        public InstrumentalComponent(IInstrumentalIdentification containingInstrument, 
            IMotionDataProcessor dataProcessor, IMotionDataProvider dataProvider, string name)
        {
            _player = Xamarin.Forms.DependencyService.Get<IInstrumentPlayer>(DependencyFetchTarget.NewInstance);
            MotionDataProcessor = dataProcessor;
            MotionDataProvider = dataProvider;

            Number = 1;
            Name = name;
            ContainingInstrument = containingInstrument;

            PreLoadAudio();
            PropertyChanged += (sender, args) => PreLoadAudio();

            PlaySoundCommand = DelegateCommand.FromAsyncHandler(PlaySound);
            TeachCommand = new DelegateCommand(Teach);
        }

        public async void PreLoadAudio()
        {
                _audionInstance = await _player.PreLoadAudio(this).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a template methode for playing sounds.
        /// Theres no common Sound API in Xamarin, so the play-logic has to be implemented specific by each platform implementation seperately
        /// </summary>
        /// <returns></returns>
        public async Task PlaySound()
        {
            cooldown.TryAddAsyncRequest(async () => await _player.Play(_audionInstance));

            if (cooldown.IsCoolingDown)
            {
                return;
            }

            try
            {
                await cooldown.Activate();
            }
            catch (Exception e)
            {
                var dependencyService = new DependencyService();
                dependencyService.Get<IUserTextNotifier>().Notify(e);
            }
        }

        private DelegateCommand _playSoundCommand;
        private string _name;
        private IInstrumentalIdentification _containingInstrument;

        public DelegateCommand PlaySoundCommand
        {
            get { return _playSoundCommand; }
            set { SetProperty(ref _playSoundCommand, value); }
        }


        public double XCoefficients
        {
            get { return _xCoefficients; }
            set { SetProperty(ref _xCoefficients, value); }
        }

        public double YCoefficients
        {
            get { return _yCoefficients; }
            set { SetProperty(ref _yCoefficients, value); }
        }

        public double ZCoefficients
        {
            get { return _zCoefficients; }
            set { SetProperty(ref _zCoefficients, value); }
        }


        public Teachement Teachement
        {
            get { return _teachement; }
            set
            {
                SetProperty(ref _teachement, value);
                OnPropertyChanged(nameof(XCoefficients));
                OnPropertyChanged(nameof(YCoefficients));
                OnPropertyChanged(nameof(ZCoefficients));
            }
        }

        protected void Teach()
        {
            // unregister current processing
            MotionDataProcessor.MotionDataProvider.MotionDataRefreshed -= MotionDataProcessor.ProcessMotionData;

            Xamarin.Forms.DependencyService.Get<IUserSoudNotifier>().Notify();

            Teachement = TeachMovement();

            // reenable motion processing 
            MotionDataProcessor.MotionDataProvider.MotionDataRefreshed += MotionDataProcessor.ProcessMotionData;
        }

        protected Teachement TeachMovement()
        {
            MotionDataProvider.MotionDataRefreshed -= MotionDataProcessor.ProcessMotionData;

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
                var task = Task.Delay((int)MotionDataProvider.RefreshRate);
                task.Wait();
            }
            stopwatch.Stop();

            var teachement = Teachement.Create(timesteps, xValues, yValues, zValues);

            MotionDataProvider.MotionDataRefreshed += MotionDataProcessor.ProcessMotionData;

            return teachement;
        }
    }

    public class Cooldown
    {
        private bool _isCoolingDown;
        private Func<Task> _request;
        private Stopwatch _stopwatch = new Stopwatch();

        private int minElapsedSecondsForRequest = BeatshakeSettings.InstrumentalCooldown -
                                                  BeatshakeSettings.MaxInstrumentalRequestDelay;

        public bool IsCoolingDown;

        public async Task Activate()
        {
            if (IsCoolingDown) // a new activation ist not supported, while old one is running
            {
                throw new InvalidOperationException("Cooldown is in progress");
            }

            while (_request != null)
            {
                IsCoolingDown = true;
                await HandleRequest();
            }

            IsCoolingDown = false;
        }

        private async Task HandleRequest()
        {
            // Start request as soon as possible
            var delay = Task.Delay(BeatshakeSettings.InstrumentalCooldown);
            var task = _request.Invoke();

            // Set timer and cooldown Data
            _stopwatch.Start();

            // Wait for task finishing
            //Task.WaitAll(Task.Delay(BeatshakeSettings.InstrumentalCooldown), task);
            _request = null; // delete handled request
            await delay;
            _stopwatch.Stop();
            _stopwatch.Reset();
        }

        /// <summary>
        /// Returns true, if the request has been added
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public bool TryAddAsyncRequest(Func<Task> func)
        {
            if (_request == null && (!_stopwatch.IsRunning || _stopwatch.ElapsedMilliseconds > minElapsedSecondsForRequest))
            {
                _request = func;
                return true;
            }
            return false;
        }



    }
}