using Microsoft.Practices.Unity;

public static class Utilities
{
    public static void RegisterDependencyServices()
    {
        Xamarin.Forms.DependencyService.Register<InstrumentPlayerImplementation>();
        Xamarin.Forms.DependencyService.Register<UserTextNotifierImplementation>();
        Xamarin.Forms.DependencyService.Register<MotionDataProviderImplementation>();
        Xamarin.Forms.DependencyService.Register<UserSoudNotifierImplementation>();
        Xamarin.Forms.DependencyService.Register<PlotViewProviderImplementation>();
    }

    public static void RegisterDependencyServices(IUnityContainer container)
    {
        container.RegisterType<Beatshake.DependencyServices.IInstrumentPlayer, InstrumentPlayerImplementation>(new TransientLifetimeManager());
        container.RegisterType<Beatshake.DependencyServices.IUserTextNotifier, UserTextNotifierImplementation>();
        container.RegisterType<Beatshake.DependencyServices.IMotionDataProvider, MotionDataProviderImplementation>();
        container.RegisterType<Beatshake.DependencyServices.IUserSoudNotifier, UserSoudNotifierImplementation>();
        container.RegisterType<Beatshake.DependencyServices.IPlotViewProdiver, PlotViewProviderImplementation>();

        //container.RegisterInstance(container.Resolve<InstrumentPlayerImplementation>());
        //container.RegisterInstance(container.Resolve<UserTextNotifierImplementation>());
        //container.RegisterInstance(container.Resolve<MotionDataProviderImplementation>());
        //container.RegisterInstance(container.Resolve<UserSoudNotifierImplementation>());
    }
}