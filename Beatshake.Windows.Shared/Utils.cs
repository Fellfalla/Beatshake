using System;
using System.Collections.Generic;
using System.Text;
#if WINDOWS_UWP
using Beatshake.UWP.DependencyServices;
using Beatshake.UWP;
#endif

#if WINDOWS_PHONE_APP
using Beatshake.WinPhone.DependencyServices;
using Beatshake.WinPhone;
#endif

namespace Beatshake.Shared.Windows
{
    public class Utils
    {
        public static void RegisterDependencyServices()
        {
            // Register Services
            Xamarin.Forms.DependencyService.Register<InstrumentPlayerImplementation>();
            Xamarin.Forms.DependencyService.Register<UserTextNotifierImplementation>();
            Xamarin.Forms.DependencyService.Register<MotionDataProviderImplementation>();
            Xamarin.Forms.DependencyService.Register<UserSoudNotifierImplementation>();
        }
    }
}
