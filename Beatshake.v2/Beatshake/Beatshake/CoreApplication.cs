using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Beatshake.Core;
using Beatshake.ViewModels;
using Beatshake.Views;
using Microsoft.Practices.Unity;
using Prism;
using Prism.Unity;
using Prism.Unity.Navigation;
using Xamarin.Forms;

namespace Beatshake
{
    public class CoreApplication : PrismApplication
    {
        public CoreApplication()
        {
            MainPage = CreateMainPage();
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
            return Container.Resolve<MainMenuView>();
        }

        protected override Prism.Navigation.INavigationService CreateNavigationService()
        {
            return new UnityPageNavigationService(Container);
        }

        protected override void OnInitialized()
        {
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<DrumView, DrumViewModel>();
            Container.RegisterTypeForNavigation<MainMenuView, MainMenuViewModel>();
            Container.RegisterTypeForNavigation<StatisticsView, StatisticsViewModel>();
            //foreach (var exportedType in GetType().GetTypeInfo().Assembly.DefinedTypes)
            //{
            //    Container.RegisterType(exportedType.AsType());
            //}
        }
    }
}
