using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Beatshake.Core;
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
            StartDrumKitCommand = DelegateCommand.FromAsyncHandler(StartDrumKit);
        }

        private async Task StartDrumKit()
        {
            await Task.Factory.StartNew(() =>
            {
                MessagingCenter.Send<object>(this,
                    BeatshakeGlobals.NavigateToDrumKit);
            });

            //await ((NavigationPage)Application.Current.MainPage).PushAsync(new DrumKitView(), true);
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
