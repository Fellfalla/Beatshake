using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;

namespace Beatshake.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {

        string _title = "Beatshake";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public DelegateCommand NavigateCommand { get; set; }

        public DelegateCommand ShowStatisticsCommand { get; set; }

        public DelegateCommand NavigateToSettingsCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationService">This parameter has to be named navigationService for Prism injecting the right one.</param>
        public MainMenuViewModel(INavigationService navigationService) : base(navigationService)
        {
            NavigateCommand = DelegateCommand.FromAsyncHandler(Navigate);
            ShowStatisticsCommand = DelegateCommand.FromAsyncHandler(ShowStatistics);
            NavigateToSettingsCommand = DelegateCommand.FromAsyncHandler(NavigateToSettings);

        }

        async Task Navigate()
        {
            await NavigateAsync<DrumViewModel>(useModalNavigation: false);
            //NavigationService.NavigateAsync<DrumViewModel>();
        }

        async Task ShowStatistics()
        {
            //NavigationService.NavigateAsync<StatisticsViewModel>();
            await NavigateAsync<StatisticsViewModel>(useModalNavigation: false);

        }

        async Task NavigateToSettings()
        {
            //NavigationService.NavigateAsync<SettingsViewModel>();
            await NavigateAsync<SettingsViewModel>(useModalNavigation: false);

        }
    }
}
