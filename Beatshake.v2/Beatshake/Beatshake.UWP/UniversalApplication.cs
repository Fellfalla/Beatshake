using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.DependencyServices;
using Microsoft.Practices.Unity;

namespace Beatshake.UWP
{
    class UniversalApplication : CoreApplication
    {
        protected override void RegisterTypes()
        {
            base.RegisterTypes();
            Utilities.RegisterDependencyServices(Container);
            //Container.RegisterType<Beatshake.DependencyServices.IInstrumentPlayer,InstrumentPlayerImplementation>();
            //Container.RegisterType<Beatshake.DependencyServices.IUserTextNotifier, UserTextNotifierImplementation>();
            //Container.RegisterType<Beatshake.DependencyServices.IMotionDataProvider, MotionDataProviderImplementation>();
            //Container.RegisterType<Beatshake.DependencyServices.IUserSoudNotifier, UserSoudNotifierImplementation>();
        }
    }
}
