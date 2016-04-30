using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.Views;
using Prism.Unity;

namespace Beatshake.WinPhone
{
    class WinPhoneApplication : CoreApplication
    {
        protected override void RegisterTypes()
        {
            base.RegisterTypes();
            Utilities.RegisterDependencyServices(Container);
            //Container.RegisterTypeForNavigation<MainPage, MainMenuViewModel>();
        }
    }
}
