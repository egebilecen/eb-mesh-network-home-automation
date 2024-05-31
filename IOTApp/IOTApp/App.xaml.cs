using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IOTApp
{
    public partial class App : Application
    {
        public App()
        {
            Current.UserAppTheme = OSAppTheme.Dark;
            InitializeComponent();
        }

        protected override void OnStart()
        {
            MainPage = new Views.ConnectingPage();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
