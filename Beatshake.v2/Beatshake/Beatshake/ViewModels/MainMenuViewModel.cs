using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.Core;
using Beatshake.Views;
using Prism.Commands;
using Prism.Navigation;

namespace Beatshake.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {

        string _title = "Beatshake";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public DelegateCommand NavigateCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationService">This parameter has to be named navigationService for Prism injecting the right one.</param>
        public MainMenuViewModel(INavigationService navigationService) : base(navigationService)
        {
            NavigateCommand = new DelegateCommand(Navigate);
        }

        void Navigate()
        {
            NavigationService.Navigate<DrumViewModel>();
        }
    }
}
