using System.Linq;
using Beatshake.ViewModels;
using Microsoft.Practices.ObjectBuilder2;
using Xamarin.Forms;

namespace Beatshake.Views
{
    public partial class DrumView : BaseContentPage
    {
        public DrumView()
        {
            InitializeComponent();
            //NavigationPage.SetHasBackButton(this, true);
        }

        public DrumViewModel ViewModel { get { return (DrumViewModel) BindingContext; } }

        protected override void OnAppearing()
        {
            if (ViewModel != null) ViewModel.IsProcessingMotionData = true;
            base.OnAppearing();
        }

        protected override bool OnBackButtonPressed()
        {
            if (ViewModel != null)
            {
                ViewModel.IsProcessingMotionData = false;
            }

            return base.OnBackButtonPressed();
        }

        //private async void TeachingStart(object sender, EventArgs e)
        //{
        //    await ViewModel.TeachCommand.Execute();
        //}
    }
}
