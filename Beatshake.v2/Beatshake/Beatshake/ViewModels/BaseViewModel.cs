using System;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace Beatshake.ViewModels
{
    public abstract class BaseViewModel : BindableBase, INavigationService
    {
        protected readonly INavigationService NavigationService;

        public DelegateCommand NavigateBackCommand { get; set; }

        public BaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
            NavigateBackCommand = DelegateCommand.FromAsyncHandler(async () => { await NavigationService.GoBackAsync(); });
            //NavigateBackCommand = DelegateCommand.FromAsyncHandler(NavigationService.GoBackAsync);
        }

        #region Facade-Pattern for Navigation

        public async Task GoBackAsync(NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            await NavigationService.GoBackAsync(parameters, useModalNavigation, animated);
        }

        public async Task NavigateAsync<T>(NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            await NavigationService.NavigateAsync<T>(parameters, useModalNavigation, animated);
        }

        public async Task NavigateAsync(Uri uri, NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            await NavigationService.NavigateAsync(uri, parameters, useModalNavigation, animated);
        }

        public async Task NavigateAsync(string name, NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            await NavigationService.NavigateAsync(name, parameters, useModalNavigation, animated);
        }

#endregion
    }
}
