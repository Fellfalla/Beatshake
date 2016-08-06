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

        //public DelegateCommand NavigateBackCommand { get; set; }

        public BaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
            //NavigateBackCommand = DelegateCommand.FromAsyncHandler(async () => { await NavigationService.GoBackAsync(); });
            //NavigateBackCommand = DelegateCommand.FromAsyncHandler(NavigationService.GoBackAsync);
        }

        #region Facade-Pattern for Navigation

        public async Task<bool> GoBackAsync(NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            return await NavigationService.GoBackAsync(parameters, useModalNavigation, animated);
        }

        public async Task NavigateAsync<TViewModel>(NavigationParameters parameters = null, bool? useModalNavigation = null,
            bool animated = true) where TViewModel : BindableBase
        {
            //Prism.Navigation.
            string toTrim = "Model";
            string viewModelName = typeof(TViewModel).Name;
            var viewName = viewModelName.Substring(0, viewModelName.Length - toTrim.Length);

            await this.NavigateAsync(viewName, parameters, useModalNavigation, animated);
            //await NavigationService.NavigateAsync<TViewModel>(parameters, useModalNavigation, animated);
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
