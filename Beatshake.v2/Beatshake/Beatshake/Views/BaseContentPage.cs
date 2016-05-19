using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Navigation;
using Xamarin.Forms;

namespace Beatshake.Views
{
    public static class PageExtensionMethods
{
        public static bool NavigateBackWithViewModel(this Page page)
        {
            var navigator = page.BindingContext as INavigationService;
            if (navigator != null)
            {
                navigator.GoBackAsync();
                return true;
            }
            return false;
        }
}

    public abstract class BaseContentPage : ContentPage
    {

        protected override bool OnBackButtonPressed()
        {
            if (!this.NavigateBackWithViewModel())
            {
                return base.OnBackButtonPressed();
            }
            return true;
        }
    }

    public abstract class BaseCarouselPage : CarouselPage
    {
        protected sealed override bool OnBackButtonPressed()
        {
            if (!this.NavigateBackWithViewModel())
            {
                return base.OnBackButtonPressed();
            }
            return true;
        }
    }
}
