using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beatshake.Core;
using Beatshake.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform;

namespace Beatshake
{
    public class App : Application
    {
        private NavigationPage _navigator = new NavigationPage(new MainView());

        public App()
        {
            MessagingCenter.Subscribe<object>(this, BeatshakeGlobals.NavigateToDrumKit, OnNavigateToDrumKit);

            
            // The root page of your application
            MainPage = _navigator;
        }

        private void OnNavigateToDrumKit(object sender)
        {
            _navigator.PushAsync(new DrumKitView());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            
            
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes

        }
    }
}
