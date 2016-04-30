using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beatshake.Core;
using Xamarin.Forms;

namespace Beatshake
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            MainPage = new NavigationPage();

            Bootstrapper bs = new Bootstrapper();
            bs.Run(this);


            //MainPage = new ContentPage
            //{
            //    Content = new StackLayout
            //    {
            //        VerticalOptions = LayoutOptions.Center,
            //        Children = {
            //            new Label {
            //                XAlign = TextAlignment.Center,
            //                Text = "Welcome to Xamarin Forms!"
            //            }
            //        }
            //    }
            //};
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
