using Xamarin.Forms;

namespace Beatshake.Views
{
    public partial class MainMenuView : BaseContentPage
    {
        public MainMenuView()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }
    }
}
