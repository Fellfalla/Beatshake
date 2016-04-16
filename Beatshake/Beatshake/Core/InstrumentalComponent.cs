using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;

namespace Beatshake.Core
{
    public class InstrumentalComponent : BindableBase, IInstrumentalComponentIdentification
    {
        public IInstrumentalIdentification ContainingInstrument { get; set; }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        [DefaultValue(1)]
        public int Number { get; set; }

        public InstrumentalComponent(IInstrumentalIdentification containingInstrument)
        {
            Number = 1;
            ContainingInstrument = containingInstrument;
            PlaySoundCommand = DelegateCommand.FromAsyncHandler(PlaySound);
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
                var dependencyService = new DependencyService();
                await dependencyService.Get<IInstrumentPlayer>().Play(this);
            }
            catch (Exception e)
            {
                var dependencyService = new DependencyService();
                dependencyService.Get<IUserNotifier>().Notify(e);
            }
        }

        private DelegateCommand _playSoundCommand;
        private string _name;

        public DelegateCommand PlaySoundCommand
        {
            get { return _playSoundCommand; }
            set { SetProperty(ref _playSoundCommand, value); }
        }
    }
}