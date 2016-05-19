using Beatshake.ViewModels;
using Xamarin.Forms;

namespace Beatshake.Views
{
    public partial class DrumView
    {
        public DrumView()
        {
            NavigationPage.SetHasBackButton(this, true);
            InitializeComponent();
        }
        public DrumViewModel ViewModel { get { return (DrumViewModel) BindingContext; } }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        //protected override bool OnBackButtonPressed()
        //{
        //    var task = ViewModel.NavigateBackCommand.Execute();
        //    task.Wait();
        //    //((Navigation.Application.Current.MainPage = new MainView();
        //    return true;
        //    //return base.OnBackButtonPressed();
        //}
        //private async void TeachingStart(object sender, EventArgs e)
        //{
        //    await ViewModel.TeachCommand.Execute();
        //}
    }
}
