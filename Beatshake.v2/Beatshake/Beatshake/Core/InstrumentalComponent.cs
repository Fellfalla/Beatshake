using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Beatshake.DependencyServices;
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

        private Cooldown cooldown = new Cooldown();

        public IInstrumentalIdentification ContainingInstrument
        {
            get { return _containingInstrument; }
            set
            {
                SetProperty(ref _containingInstrument, value);
            }
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

        public InstrumentalComponent(IInstrumentalIdentification containingInstrument, string name)
        {
            _player = Xamarin.Forms.DependencyService.Get<IInstrumentPlayer>(DependencyFetchTarget.NewInstance);

            Number = 1;
            Name = name;
            ContainingInstrument = containingInstrument;

            PreLoadAudio();
            PropertyChanged += (sender, args) => PreLoadAudio();

            PlaySoundCommand = DelegateCommand.FromAsyncHandler(PlaySound);
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