using Xamarin.Forms;

namespace Beatshake.Views
{
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