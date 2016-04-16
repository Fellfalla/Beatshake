using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.Views;
using Prism.Commands;
using Prism.Mvvm;
using Xamarin.Forms;

namespace Beatshake.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel()
        {
            StartDrumKitCommand = new DelegateCommand(StartDrumKit);
        }

        private void StartDrumKit()
        {
            Application.Current.MainPage = new DrumKitView();
        }

        private DelegateCommand _startDrumKitCommand;

        public string Title { get { return AppResources.Title; } }

        public DelegateCommand StartDrumKitCommand
        {
            get { return _startDrumKitCommand; }
            set { SetProperty(ref _startDrumKitCommand, value); }
        }
    }
}
