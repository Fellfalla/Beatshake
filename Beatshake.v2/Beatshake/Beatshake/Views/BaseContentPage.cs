using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Navigation;
using Xamarin.Forms;

namespace Beatshake.Views
{
    public abstract class BaseContentPage : ContentPage
    {
        protected sealed override bool OnBackButtonPressed()
        {
            var navigator = BindingContext as INavigationService;
            if (navigator != null)
            {
                navigator.GoBack();
                return true;
            }

            return base.OnBackButtonPressed();
        }
    }
}
