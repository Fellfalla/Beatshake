using Prism.Mvvm;
using Prism.Navigation;

namespace Beatshake.Core
{
    public abstract class BaseViewModel : BindableBase
    {
        protected readonly INavigationService NavigationService;

        public BaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

    }
}
