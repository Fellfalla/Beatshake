using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.ViewModels;
using Prism.Navigation;
using Xamarin.Forms;

namespace Beatshake.Views
{
    public partial class DrumView
    {
        public DrumView()
        {
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
    }
}
