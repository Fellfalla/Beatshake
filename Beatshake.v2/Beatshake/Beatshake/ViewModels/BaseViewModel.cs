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

            NavigateBackCommand = DelegateCommand.FromAsyncHandler(async () => await NavigationService.GoBack());
        }

#region Facade-Pattern for Navigation

        public Task GoBack(NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            return NavigationService.GoBack(parameters, useModalNavigation, animated);
        }

        public Task Navigate<T>(NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            return NavigationService.Navigate<T>(parameters, useModalNavigation, animated);
        }

        public Task Navigate(Uri uri, NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            return NavigationService.Navigate(uri, parameters, useModalNavigation, animated);
        }

        public Task Navigate(string name, NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            return NavigationService.Navigate(name, parameters, useModalNavigation, animated);
        }

#endregion
    }
}
