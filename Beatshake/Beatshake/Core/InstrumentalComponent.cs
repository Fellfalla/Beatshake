using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
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
            _audionInstance = await _player.PreLoadAudio(this);
        }

        /// <summary>
        /// This is a template methode for playing sounds.
        /// Theres no common Sound API in Xamarin, so the play-logic has to be implemented specific by each platform implementation seperately
        /// </summary>
        /// <returns></returns>
        public async Task PlaySound()
        {
            if (cooldown.IsCoolingDown)
            {
                //return;
            }

            try
            {
                using (cooldown.Activate())
                {
                    await _player.Play(_audionInstance);
                    await Task.Delay(BeatshakeSettings.InstrumentalCooldown);
                }

            }
            catch (Exception e)
            {
                var dependencyService = new DependencyService();
                dependencyService.Get<IUserNotifier>().Notify(e);
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

    public class Cooldown : IDisposable
    {
        public bool IsCoolingDown;

        public Cooldown Activate()
        {
            IsCoolingDown = true;
            return this;
        }

        public void Dispose()
        {
            IsCoolingDown = false;
        }
    }
}