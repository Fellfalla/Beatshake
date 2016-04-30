using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.Unity;

public static class Utilities
{
    public static void RegisterDependencyServices()
    {
        Xamarin.Forms.DependencyService.Register<InstrumentPlayerImplementation>();
        Xamarin.Forms.DependencyService.Register<UserTextNotifierImplementation>();
        Xamarin.Forms.DependencyService.Register<MotionDataProviderImplementation>();
        Xamarin.Forms.DependencyService.Register<UserSoudNotifierImplementation>();
    }

    public static void RegisterDependencyServices(IUnityContainer container)
    {
        container.RegisterType<Beatshake.DependencyServices.IInstrumentPlayer, InstrumentPlayerImplementation>();
        container.RegisterType<Beatshake.DependencyServices.IUserTextNotifier, UserTextNotifierImplementation>();
        container.RegisterType<Beatshake.DependencyServices.IMotionDataProvider, MotionDataProviderImplementation>();
        container.RegisterType<Beatshake.DependencyServices.IUserSoudNotifier, UserSoudNotifierImplementation>();
    }
}
