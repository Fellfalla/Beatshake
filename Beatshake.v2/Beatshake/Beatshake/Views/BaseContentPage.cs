using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Beatshake.Views
{
    public abstract class BaseContentPage : ContentPage
    {
        public BaseContentPage()
        {
        }

        protected override bool OnBackButtonPressed()
        {
            if (!this.NavigateBackWithViewModel())
            {
                return base.OnBackButtonPressed();
            }
            return true;
        }
    }

    public abstract class BaseSideNavPage : MasterDetailPage
    {

        //protected override bool OnBackButtonPressed()
        //{
        //    if (!this.NavigateBackWithViewModel())
        //    {
        //        return base.OnBackButtonPressed();
        //    }
        //    return true;
        //}
    }
    
}
