using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.Core;
using Prism.Navigation;

namespace Beatshake.ViewModels
{
    public class DrumViewModel : BaseViewModel
    {
        private string _heading = "Shake your Drums!";

        public string Heading
        {
            get { return _heading; }
            set { SetProperty(ref _heading, value); }
        }

        public DrumViewModel(INavigationService navigationService) : base(navigationService)
        {
            
        }
    }
}
