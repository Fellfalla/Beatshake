using System;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;

namespace Beatshake.Core
{
    public class InstrumentalComponent : BindableBase
    {
        public InstrumentalComponent()
        {
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
                await dependencyService.Get<IAudioPlayer>().Play("test");
            }
            catch (Exception e)
            {
                var dependencyService = new DependencyService();
                dependencyService.Get<IUserNotifier>().Notify(e);
            }
        }

        private DelegateCommand _playSoundCommand;

        public DelegateCommand PlaySoundCommand
        {
            get { return _playSoundCommand; }
            set { SetProperty(ref _playSoundCommand, value); }
        }
    }
}