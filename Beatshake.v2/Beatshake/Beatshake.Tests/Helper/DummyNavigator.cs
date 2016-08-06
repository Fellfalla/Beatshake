using System;
using System.Threading.Tasks;
using Prism.Navigation;

namespace Beatshake.Tests.ViewModels
{
    class DummyNavigator : INavigationService
    {


        public Task NavigateAsync<T>(NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            throw new NotImplementedException();
        }

        public Task NavigateAsync(Uri uri, NavigationParameters parameters = null, bool? useModalNavigation = null,
            bool animated = true)
        {
            throw new NotImplementedException();
        }

        public Task NavigateAsync(string name, NavigationParameters parameters = null, bool? useModalNavigation = null,
            bool animated = true)
        {
            throw new NotImplementedException();
        }

        Task<bool> INavigationService.GoBackAsync(NavigationParameters parameters, bool? useModalNavigation, bool animated)
        {
            throw new NotImplementedException();
        }
    }
}