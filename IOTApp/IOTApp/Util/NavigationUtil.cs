using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace IOTApp
{
    public static class NavigationUtil
    {
        public static string GetCurrentPageClassName()
        {
            string name = Application.Current.MainPage.GetType().Name;

            if(name != "NavigationPage") return name;

            return Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault().GetType().Name;
        }
    }
}
