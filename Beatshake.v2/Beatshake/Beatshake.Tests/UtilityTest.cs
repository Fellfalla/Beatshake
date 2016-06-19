using System.Linq;
using System.Threading.Tasks;
using Beatshake.Core;
using Beatshake.ViewModels;
using Beatshake.Views;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Xunit;

namespace Beatshake.Tests
{
    public class UtilityTest
    {
        [Fact]
        public void MidnightFormulaTest()
        {
            double[] result;

            // x^2
            result = Utility.MidnightFormula(1, 0, 0).ToArray();
            Assert.Equal(1, result.Count());
            Assert.Equal(0, result[0]);

            // x^2 + 1
            result = Utility.MidnightFormula(1, 0, 1).ToArray();
            Assert.Equal(0, result.Count());

            // x^2 - 1
            result = Utility.MidnightFormula(1, 0, -1).ToArray();
            Assert.Equal(2, result.Count());
            Assert.True(result.Contains(1));
            Assert.True(result.Contains(-1));
        }
    }

    public class NavigationTest
    {
        [Fact]
        public async Task NavigationBackTest()
        {
            var container = new UnityContainer();
            //var container = application.Container;
            container.RegisterType<DrumViewModel>(new ContainerControlledLifetimeManager());
            container.RegisterType<MainMenuViewModel>(new ContainerControlledLifetimeManager());
            container.RegisterType<StatisticsViewModel>(new ContainerControlledLifetimeManager());
            container.RegisterType<SettingsViewModel>(new ContainerControlledLifetimeManager());

            container.RegisterTypeForNavigation<DrumView, DrumViewModel>();
            container.RegisterTypeForNavigation<MainMenuView, MainMenuViewModel>();
            container.RegisterTypeForNavigation<StatisticsView, StatisticsViewModel>();
            container.RegisterTypeForNavigation<SettingsView, SettingsViewModel>();

            var mainMenu = container.Resolve<MainMenuViewModel>();
            Assert.NotNull(mainMenu);
            await mainMenu.NavigateAsync<DrumViewModel>();

            //Assert.True(application.MainPage.GetType() == typeof(DrumView));
        }
    }
}