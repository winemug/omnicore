﻿using OmniCore.Mobile.Services;
using OmniCore.Mobile.Views;
using System;
using OmniCore.Services.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OmniCore.Mobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            AmqpTestClient.InitializeClient();
            var xwsc = new XDripWebServiceClient();
            xwsc.Test();
            
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
