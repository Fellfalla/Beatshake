using Beatshake.DependencyServices;
using Beatshake.ViewModels;
using Beatshake.Views;
using Microsoft.Practices.Unity;
using Prism.Common;
using Prism.Navigation;
using Prism.Unity;
using Prism.Unity.Navigation;
using Xamarin.Forms;

namespace Beatshake
{
    public partial class CoreApplication : PrismApplication
    {
        public CoreApplication()
        {
            //MainPage = CreateMainPage();
            //Bootstrapper bs = new Bootstrapper();
            //bs.Run(this);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps

        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected Page CreateMainPage()
        {

            var mainMenuView = new MainMenuView(); //Container.Resolve<MainMenuView>();
            var navPage = new NavigationPage(mainMenuView);
            //var navPage = new NavigationPage();
            return navPage;
        }

        protected override INavigationService CreateNavigationService()
        {
            if (_navigationService == null)
            {
                var applicationProvider = new ApplicationProvider();
                _navigationService = new UnityPageNavigationService(Container, applicationProvider, Logger);

                // if _navigationService is not set before MainMenuView is Resolved there will be a endless loop
                applicationProvider.MainPage = CreateMainPage();
            }
            return _navigationService;
        }

        private INavigationService _navigationService;

        protected override void OnInitialized()
        {
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
        }


        public ContainerControlledLifetimeManager MainMenuViewModelLifetimeManager = new ContainerControlledLifetimeManager();
        public ContainerControlledLifetimeManager MainMenuViewLifetimeManager = new ContainerControlledLifetimeManager();
        public ContainerControlledLifetimeManager DrumViewModelLifetimeManager = new ContainerControlledLifetimeManager();
        public ContainerControlledLifetimeManager DrumViewLifetimeManager = new ContainerControlledLifetimeManager();
        public ContainerControlledLifetimeManager StatisticsViewModelLifetimeManager = new ContainerControlledLifetimeManager();
        public ContainerControlledLifetimeManager SettingsViewModelLifetimeManager = new ContainerControlledLifetimeManager();


        protected override void RegisterTypes()
        {
            Container.RegisterInstance<IMotionDataProvider>(
    Xamarin.Forms.DependencyService.Get<IMotionDataProvider>(DependencyFetchTarget.GlobalInstance));
            
            //Container.RegisterInstance(CreateNavigationService());
            Container.RegisterType<DrumViewModel>(DrumViewModelLifetimeManager);
            Container.RegisterType<MainMenuViewModel>(MainMenuViewModelLifetimeManager);
            Container.RegisterType<StatisticsViewModel>(StatisticsViewModelLifetimeManager);
            Container.RegisterType<SettingsViewModel>(SettingsViewModelLifetimeManager);

            Container.RegisterType<DrumView>(DrumViewLifetimeManager);
            Container.RegisterType<MainMenuView>(MainMenuViewLifetimeManager);

            Container.RegisterTypeForNavigation<DrumView, DrumViewModel>();
            Container.RegisterTypeForNavigation<MainMenuView, MainMenuViewModel>();
            Container.RegisterTypeForNavigation<StatisticsView, StatisticsViewModel>();
            Container.RegisterTypeForNavigation<SettingsView, SettingsViewModel>();

        }
    }
}
