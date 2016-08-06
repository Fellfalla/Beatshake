using Xamarin.Forms;

namespace Beatshake.Views
{
    public partial class MainMenuView
    {
        public MainMenuView()
        {
            //NavigationPage.SetHasNavigationBar(this, true);

            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }
    }
}
