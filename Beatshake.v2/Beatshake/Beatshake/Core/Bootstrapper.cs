using System;
using System.Reflection;
using Beatshake.ViewModels;
using Beatshake.Views;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Prism.Unity.Navigation;
using Xamarin.Forms;

namespace Beatshake.Core
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override Page CreateMainPage()
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
            Container.RegisterTypeForNavigation<DrumView>();
            Container.RegisterTypeForNavigation<MainMenuView>();
            foreach (var exportedType in GetType().GetTypeInfo().Assembly.ExportedTypes)
            {
                Container.RegisterType(exportedType);
            }
            //Container.RegisterType<MainMenuView>();
            //Container.RegisterType<MainMenuViewModel>();
            //Container.RegisterType<DrumViewModel>();
            //Container.RegisterType<DrumView>();
        }
    }
}
