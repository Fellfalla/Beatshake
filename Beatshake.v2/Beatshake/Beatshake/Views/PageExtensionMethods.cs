using System;
using Beatshake.ExtensionMethods;
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
                bool useModal = page.FindParentOfType<NavigationPage>() == null;
                var task = navigator.GoBackAsync(useModalNavigation: useModal);
                try
                {
                    task.Wait(); // todo: find out why this throws AggregateException on second back navigation command from same view
                    return true;
                }
                catch (AggregateException)
                {
                    return false;
                }
            }
            return false;
        }
    }
}