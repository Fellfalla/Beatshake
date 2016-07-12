using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Beatshake.DependencyServices;
using Beatshake.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using DependencyService = Prism.Services.DependencyService;

namespace Beatshake.Core
{
    public class InstrumentalComponent : BindableBase, IInstrumentalComponentIdentification, IDisposable
    {
        protected IInstrumentPlayer Player
        {
            get { return _player; }
            set
            {
                _player = value;
                _player.Component = this;
            }
        }

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

        public bool IsActivated
        {
            get { return _isActivated; }
            set { SetProperty(ref _isActivated, value); }
        }

        [DefaultValue(1)]
        public int Number
        {
            get { return _number; }
            set { SetProperty(ref _number, value); }
        }

        public InstrumentalComponent(IInstrumentalIdentification containingInstrument, 
            IMotionDataProcessor dataProcessor, IMotionDataProvider dataProvider, IInstrumentPlayer player, string name)
        {
            //_player = Xamarin.Forms.DependencyService.Get<IInstrumentPlayer>(DependencyFetchTarget.NewInstance);
            Player = player;
            MotionDataProcessor = dataProcessor;
            MotionDataProvider = dataProvider;

            Number = 1;
            Name = name;
            ContainingInstrument = containingInstrument;

            Player.PreLoadAudio();

            PropertyChanged += OnPropertyChanged;

            PlaySoundCommand = DelegateCommand.FromAsyncHandler(PlaySound);
            TeachCommand = DelegateCommand.FromAsyncHandler(Teach);

            //Task.WaitAll(audioLoader);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(Name) || args.PropertyName == nameof(Number))
            {
                PreLoadAudio().ConfigureAwait(false);
            }
        }


        public async Task PreLoadAudio()
        {
            await Player.PreLoadAudioAsync().ConfigureAwait(false);
            //_audionInstance = await _player.PreLoadAudio(this).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a template methode for playing sounds.
        /// Theres no common Sound API in Xamarin, so the play-logic has to be implemented specific by each platform implementation seperately
        /// </summary>
        /// <returns></returns>
        public async Task PlaySound()
        {
            cooldown.TryAddAsyncRequest(async () =>
            {
                await Player.PlayAsync().ConfigureAwait(false);
            });

            if (cooldown.IsCoolingDown)
            {
                return;
            }

            try
            {
                await cooldown.Activate().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                var dependencyService = new DependencyService();
                await dependencyService.Get<IUserTextNotifier>().Notify(e).ConfigureAwait(false);
            }
        }

        private DelegateCommand _playSoundCommand;
        private string _name;
        private IInstrumentalIdentification _containingInstrument;
        private bool _isActivated = true;
        private int _number;
        private IInstrumentPlayer _player;

        public DelegateCommand PlaySoundCommand
        {
            get { return _playSoundCommand; }
            set { SetProperty(ref _playSoundCommand, value); }
        }

        public Teachement Teachement
        {
            get { return _teachement; }
            set { SetProperty(ref _teachement, value); }
        }

        protected async Task Teach()
        {
            // unregister current processing
            MotionDataProcessor.MotionDataProvider.MotionDataRefreshed -= MotionDataProcessor.ProcessMotionData;
            MotionDataProvider.MotionDataRefreshed -= MotionDataProcessor.ProcessMotionData;

            Teachement = await Teachement.TeachMovement(MotionDataProvider);

            // reenable motion processing 
            MotionDataProcessor.MotionDataProvider.MotionDataRefreshed += MotionDataProcessor.ProcessMotionData;
            MotionDataProvider.MotionDataRefreshed += MotionDataProcessor.ProcessMotionData;
        }

        public void Dispose()
        {
            _player.Dispose();
        }
    }
}