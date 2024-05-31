using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IOTApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectingPage : ContentPage
    {
        public ConnectingPage()
        {
            InitializeComponent();
            IOTClient.Instance.Init();
        }
    }
}