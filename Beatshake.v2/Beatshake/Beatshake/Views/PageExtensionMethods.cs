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
}