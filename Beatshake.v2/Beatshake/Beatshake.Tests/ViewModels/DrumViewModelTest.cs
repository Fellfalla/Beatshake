using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.Core;
using Beatshake.DependencyServices;
using Beatshake.ExtensionMethods;
using Beatshake.ViewModels;
using Microsoft.Practices.Unity;
using Prism.Navigation;
using Xunit;

namespace Beatshake.Tests.ViewModels
{
    public class DrumViewModelTest
    {
        [Fact]
        public void ToggleButtonsExcludeEachOther()
        {
            var container = new UnityContainer();
            container.RegisterType<INavigationService,DummyNavigator>();
            container.RegisterType<IInstrumentPlayer,DummyPlayer>();
            container.RegisterType<IMotionDataProvider, DummyMotionDataProvier>();
            container.RegisterType<DrumViewModel>();
            DrumViewModel viewModel = container.Resolve<DrumViewModel>();

            viewModel.UseFunctionAnalysis = false;
            viewModel.UseRandom = false;
            viewModel.UseTeachement = false;

            Assert.False(viewModel.UseFunctionAnalysis);
            Assert.False(viewModel.UseRandom);
            Assert.False(viewModel.UseTeachement);

            // set all true for creating equal testcondition in next 3 testSteps
            viewModel.UseFunctionAnalysis = true;
            viewModel.UseRandom = true;
            viewModel.UseTeachement = true;

            viewModel.UseRandom = true;
            Assert.False(viewModel.UseFunctionAnalysis);
            Assert.True(viewModel.UseRandom);
            Assert.False(viewModel.UseTeachement);

            viewModel.UseFunctionAnalysis = true;
            Assert.True(viewModel.UseFunctionAnalysis);
            Assert.False(viewModel.UseRandom);
            Assert.False(viewModel.UseTeachement);

            viewModel.UseTeachement = true;
            Assert.False(viewModel.UseFunctionAnalysis);
            Assert.False(viewModel.UseRandom);
            Assert.True(viewModel.UseTeachement);
            return;
        }
    }

    class DummyMotionDataProvier : IMotionDataProvider
    {
        public DataContainer<double> Pose { get; }
        public DataContainer<double> Velocity { get; }
        public DataContainer<double> Acceleration { get; }
        public uint RefreshRate { get; set; }
        public event Custom.TypedEventHandler<IMotionDataProvider> MotionDataRefreshed;
    }

    class DummyNavigator : INavigationService
    {
        public Task GoBackAsync(NavigationParameters parameters = null, bool? useModalNavigation = null, bool animated = true)
        {
            throw new NotImplementedException();
        }

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
    }

    class DummyPlayer : IInstrumentPlayer
    {
        public Task Play(object audioData)
        {
            Trace.WriteLine("Audio played for " + audioData.ToString());
            return new Task(() => {});
        }

        public Task<object> PreLoadAudio(IInstrumentalComponentIdentification component)
        {
            Trace.WriteLine("Audio Preloaded for " + component.ToString());
            return new Task<object>(component.ToString);
        }
    }


}
