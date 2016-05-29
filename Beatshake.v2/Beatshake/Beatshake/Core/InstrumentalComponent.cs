using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Beatshake.DependencyServices;
using Beatshake.ViewModels;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Mvvm;
using Xamarin.Forms;
using DependencyService = Prism.Services.DependencyService;

namespace Beatshake.Core
{
    public class InstrumentalComponentFactory
    {
        private IInstrumentalIdentification _containingInstrument;
        private IMotionDataProcessor _dataProcessor;
        private IMotionDataProvider _dataProvider;
        private IUnityContainer _container;
        private bool _isInitialized;

        public InstrumentalComponentFactory(IUnityContainer container)
        {
         
            _container = container;
        }

        public void Init(IInstrumentalIdentification containingInstrument,
            IMotionDataProcessor dataProcessor, IMotionDataProvider dataProvider)
        {
            _containingInstrument = containingInstrument;
            _dataProcessor = dataProcessor;
            _dataProvider = dataProvider;
            _isInitialized = true;
        }

        public InstrumentalComponent CreateInstrumentalComponent(string name)
        {
            if (!_isInitialized)
            {
                throw new IncompleteInitializationException("Call " + nameof(Init));
            }

            var newPlayerInstance = _container.Resolve<IInstrumentPlayer>();
            return new InstrumentalComponent(_containingInstrument,_dataProcessor, _dataProvider, newPlayerInstance, name);
        }
    }

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
            IMotionDataProcessor dataProcessor, IMotionDataProvider dataProvider, IInstrumentPlayer player, string name)
        {
            //_player = Xamarin.Forms.DependencyService.Get<IInstrumentPlayer>(DependencyFetchTarget.NewInstance);
            _player = player;
            MotionDataProcessor = dataProcessor;
            MotionDataProvider = dataProvider;

            Number = 1;
            Name = name;
            ContainingInstrument = containingInstrument;

            PreLoadAudio();
            PropertyChanged += OnPropertyChanged;

            PlaySoundCommand = DelegateCommand.FromAsyncHandler(PlaySound);
            TeachCommand = DelegateCommand.FromAsyncHandler(Teach);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(Name) ||
                args.PropertyName == nameof(Number)
                )
            {
                PreLoadAudio();
            }
        }


        public async void PreLoadAudio()
        {
            _audionInstance = await _player.PreLoadAudio(this); //.ConfigureAwait(false);
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

        protected async Task Teach()
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
            Teachement teachement = null;
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

            try
            {
                var normalizedTimestamps = Utility.NormalizeTimeStamps(timesteps);
                teachement = Teachement.Create(normalizedTimestamps, xValues, yValues, zValues);
            }
            catch (InsufficientDataException) // thrown if the peak is to near at beginning data
            {
                await Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Teachement failed. Please try again. (longer)");
            }
            catch (InvalidOperationException)
            {
                await Xamarin.Forms.DependencyService.Get<IUserTextNotifier>().Notify("Click OK when you finished teaching");
            }

            MotionDataProvider.MotionDataRefreshed += MotionDataProcessor.ProcessMotionData;

            return teachement;
        }



    }
}