using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;

namespace Beatshake.Core
{
    public class InstrumentalComponent : BindableBase, IInstrumentalComponentIdentification
    {
        private readonly IInstrumentPlayer _player;

        private object _audionInstance;

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
            var dependencyService = new DependencyService();
            _player = dependencyService.Get<IInstrumentPlayer>();

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
            try
            {
                await _player.Play(_audionInstance);
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
}