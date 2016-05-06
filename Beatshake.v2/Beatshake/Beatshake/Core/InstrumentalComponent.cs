using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
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
        private object _audionInstance;
        private Teachement _teachement;
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
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

        public Teachement Teachement
        {
            get { return _teachement; }
            set
            {
                SetProperty(ref _teachement, value);
            }
        }

        protected async void Teach()
        {
            // unregister current processing
            MotionDataProcessor.MotionDataProvider.MotionDataRefreshed -= MotionDataProcessor.ProcessMotionData;

            Teachement = await TeachMovement();


            // reenable motion processing 
            MotionDataProcessor.MotionDataProvider.MotionDataRefreshed += MotionDataProcessor.ProcessMotionData;
        }

        protected async Task<Teachement> TeachMovement()
        {
            MotionDataProvider.MotionDataRefreshed -= MotionDataProcessor.ProcessMotionData;

            // record movement

            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();
            List<double> zValues = new List<double>();
            List<double> timesteps = new List<double>();
            var userConfirmation = Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Click OK when you finished teaching");

            var measureTask = Task.Factory.StartNew(() =>
            {
                while (!userConfirmation.IsCompleted)
                {
                    timesteps.Add(MotionDataProvider.Acceleration.Timestamp);
                    xValues.Add(MotionDataProvider.Acceleration.Trans[0]);
                    yValues.Add(MotionDataProvider.Acceleration.Trans[1]);
                    zValues.Add(MotionDataProvider.Acceleration.Trans[2]);
                    var task = Task.Delay((int) MotionDataProvider.RefreshRate);
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

            Teachement teachement = null;
            try
            {
                var normalizedTimestamps = Utility.NormalizeTimeStamps(timesteps);
                teachement = Teachement.Create(normalizedTimestamps, xValues, yValues, zValues);
            }
            catch (InvalidOperationException)
            {
                Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Click OK when you finished teaching");
            }

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