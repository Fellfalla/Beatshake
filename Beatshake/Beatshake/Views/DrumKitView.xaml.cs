using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.ViewModels;
using Xamarin.Forms;

namespace Beatshake.Views
{
    public partial class DrumKitView : ContentPage
    {
        public DrumKitViewModel ViewModel { get { return (DrumKitViewModel) BindingContext; } }

        public DrumKitView()
        {
            InitializeComponent();

        }


        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();  
        }
    }
}
