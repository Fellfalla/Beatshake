using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.Core;
using Prism.Navigation;

namespace Beatshake.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public int FunctionSamples
        {
            get { return BeatshakeSettings.SamplePoints; }
            set { SetProperty(ref BeatshakeSettings.SamplePoints, value); }
        }
    }
}
