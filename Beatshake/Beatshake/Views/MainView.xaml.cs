﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beatshake.Core;
using Xamarin.Forms;

namespace Beatshake.Views
{
    public partial class MainView : ContentPage
    {
        public MainView()
        {
            InitializeComponent();


        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new DrumKitView(), true);
        }
    }
}
