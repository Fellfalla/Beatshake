using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Beatshake.Core
{
    public static class PageNavigationManager
    {
        public static Page MainPage;

        public static Page DrumKitPage;

        public static async void SwitchToPage(Page page)
        {
            await ((NavigationPage) Application.Current.MainPage).PushAsync(page, true);
        }

        public static async void RemoveCurrentPage()
        {
            await ((NavigationPage) Application.Current.MainPage).PopAsync(true);
        }

    }
}
