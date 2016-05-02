using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Beatshake.Droid.DependencyServices;
using Microsoft.Practices.Unity;

namespace Beatshake.Droid
{
    public class AndroidApplication : CoreApplication
    {
        protected override void RegisterTypes()
        {
            Xamarin.Forms.DependencyService.Register<InstrumentPlayerImplementation>();
            Xamarin.Forms.DependencyService.Register<UserTextNotifierImplementation>();
            Xamarin.Forms.DependencyService.Register<MotionDataProviderImplementation>();
            Xamarin.Forms.DependencyService.Register<UserSoudNotifierImplementation>();

            Container.RegisterType<Beatshake.DependencyServices.IInstrumentPlayer, InstrumentPlayerImplementation>();
            Container.RegisterType<Beatshake.DependencyServices.IUserTextNotifier, UserTextNotifierImplementation>();
            Container.RegisterType<Beatshake.DependencyServices.IMotionDataProvider, MotionDataProviderImplementation>();
            Container.RegisterType<Beatshake.DependencyServices.IUserSoudNotifier, UserSoudNotifierImplementation>();

            base.RegisterTypes();
        }

    }
}