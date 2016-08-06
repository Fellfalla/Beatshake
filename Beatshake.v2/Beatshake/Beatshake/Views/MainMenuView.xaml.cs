using System;
using Beatshake.ViewModels;
using Xamarin.Forms;

namespace Beatshake.Views
{
    public partial class MainMenuView
    {

        private MainMenuViewModel ViewModel
        {
            get { return (MainMenuViewModel) BindingContext; }
            set { BindingContext = value; }
        }

        public MainMenuView(MainMenuViewModel viewModel)
        {
            //NavigationPage.SetHasNavigationBar(this, true);
            ViewModel = viewModel;
            InitializeComponent();
            Menu.ItemSelected += (sender, e) => {
                                                    if (MenuItemSelected != null)
                                                        MenuItemSelected(this, e.SelectedItem as MenuItem);
            };

        }

        public event EventHandler<MenuItem> MenuItemSelected;

        //protected override bool OnBackButtonPressed()
        //{
        //    return base.OnBackButtonPressed();
        //}

        //void NavigateTo(MenuItem menu)
        //{
        //    //Detail = new NavigationPage(displayPage);

        //}

 
    }
}
