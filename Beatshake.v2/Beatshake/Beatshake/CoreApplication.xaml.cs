using System;
using System.Linq;
using System.Threading.Tasks;
using Beatshake.DependencyServices;
using Beatshake.ViewModels;
using Beatshake.Views;
using Microsoft.Practices.Unity;
using Prism.Common;
using Prism.Logging;
using Prism.Mvvm;
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
            // instanciate staring views
            var rootPage = Container.Resolve<RootPage>();
            var mainMenu = Container.Resolve<MainMenuView>();
            var firstPage = Container.Resolve<DrumView>();
            var navPage = ((NavigationPageNavigationService)NavigationService).NavigationPage = new NavigationPage(firstPage);
            navPage.Pushed += (sender, args) => rootPage.IsPresented = false;
            // set events and other mechanism
            mainMenu.MenuItemSelected += (sender, e) => _navigationService.NavigateAsync(e.TargetType);

            // Assign all staring views
            rootPage.Master = mainMenu;
            rootPage.Detail = navPage;
            MainPage = rootPage;
            
            NavigationService.NavigateAsync(nameof(SettingsView));
        }

        //protected override void OnStart()
        //{
        //    // Handle when your app starts
        //}

        //protected override void OnSleep()
        //{
        //    // Handle when your app sleeps

        //}

        //protected override void OnResume()
        //{
        //    // Handle when your app resumes
        //}

        //protected Page CreateMainPage()
        //{

        //    var mainMenuView = Container.Resolve<MainMenuView>();
        //    var navPage = new NavigationPage(mainMenuView);
        //    //var navPage = new NavigationPage();
        //    return navPage;
        //}

        //private ApplicationProvider _applicationProvider;
        //private NavigationPage _rootNavigationPage;

        protected override INavigationService CreateNavigationService()
        {
            //var _navigationService = base.CreateNavigationService();

            if (_navigationService == null)
            {
                //_applicationProvider = new ApplicationProvider();
                //_rootNavigationPage = new NavigationPage(new ContentPage());
                // var navPage = new NavigationPage();
                //_applicationProvider.MainPage = navPage;
                //_navigationService = new UnityPageNavigationService(Container, _applicationProvider, Logger);                
                _navigationService = new NavigationPageNavigationService(Container);
                
                //// if _navigationService is not set before MainMenuView is Resolved there will be a endless loop
                ////var mainMenu = Container.Resolve<MainMenuView>();


                ////Master = menuPage;
                ////Detail = new NavigationPage(new ContractsPage());

                ////var navPage = new NavigationPage(Container.Resolve<MainMenuView>());

                //var rootPage = Container.Resolve<RootPage>();

                //var mainMenu = new MainMenuView(null);//Container.Resolve<MainMenuView>();
                //mainMenu.MenuItemSelected += (sender, e) => _navigationService.NavigateAsync(((Type)e.BindingContext).Name);
                //rootPage.Master = mainMenu;

                //var navPage = ;
                //rootPage.Detail = navPage;

            }

            return _navigationService;
        }

        private IBeatshakeNavigationServie _navigationService;

        protected override void OnInitialized()
        {
            InitializeComponent();
            //NavigationService.NavigateAsync<MainMenuViewModel>(useModalNavigation: false);
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
        }


        public LifetimeManager MainMenuViewModelLifetimeManager = new PerResolveLifetimeManager();
        public LifetimeManager MainMenuViewLifetimeManager = new PerResolveLifetimeManager();
        public LifetimeManager DrumViewModelLifetimeManager = new PerThreadLifetimeManager();
        public LifetimeManager DrumViewLifetimeManager = new PerResolveLifetimeManager();
        public LifetimeManager StatisticsViewModelLifetimeManager = new PerResolveLifetimeManager();
        public LifetimeManager SettingsViewModelLifetimeManager = new PerResolveLifetimeManager();


        protected override void RegisterTypes()
        {
            Container.RegisterInstance<IMotionDataProvider>(
    Xamarin.Forms.DependencyService.Get<IMotionDataProvider>(DependencyFetchTarget.GlobalInstance));
            
            Container.RegisterInstance(CreateNavigationService());
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

    public interface IBeatshakeNavigationServie : INavigationService
    {

        Task NavigateAsync(Type type, NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true);

    }

    public class NavigationPageNavigationService : IBeatshakeNavigationServie, IPageAware
    {
        private IUnityContainer _container;
        private NavigationPage _navigationPage;

        public NavigationPage NavigationPage
        {
            get { return _navigationPage; }
            set
            {
                _navigationPage = value;
            }
        }

        public NavigationPageNavigationService(IUnityContainer container, NavigationPage navigationPage = null)
        {
            _container = container;
            _navigationPage = navigationPage;
        }

        public async Task<bool> GoBackAsync(NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            if (_navigationPage == null)
            {
                return false;
            }

            await _navigationPage.PopAsync(animated);
            Page = _navigationPage.CurrentPage;
            return true;
        }

        public async Task NavigateAsync(Uri uri, NavigationParameters parameters = null, bool? useModalNavigation = null,
            bool animated = true)
        {
            await this.NavigateAsync(uri.OriginalString, parameters, useModalNavigation, animated);
        }

        public async Task NavigateAsync(string name, NavigationParameters parameters = null, bool? useModalNavigation = null,
            bool animated = true)
        {
            var type = _container.Registrations.First(registration => registration.MappedToType.Name.Equals(name)).MappedToType;
            await this.NavigateAsync(type, parameters, useModalNavigation, animated);
        }

        public async Task NavigateAsync(Type type, NavigationParameters parameters = null, bool? useModalNavigation = null,
            bool animated = true)
        {
            var page = _container.Resolve(type) as Page;
            Page = page;
            await _navigationPage.PushAsync(page);
        }

        public Page Page { get; set; }
    }
}
