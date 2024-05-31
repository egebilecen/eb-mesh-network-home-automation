using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using Entry = Microcharts.ChartEntry;
using Microcharts;

namespace IOTApp.Views
{
    public partial class MainPage : ContentPage
    {
        List<Entry> dummyEntries = new List<Entry>  
        {  
            new Entry(15)  
            {  
                Color=SKColor.Parse(((Color)Application.Current.Resources["PrimaryColor"]).ToHex()),  
                Label ="13.00",  
                ValueLabel = "15",
                ValueLabelColor = SKColors.White
            },  
            new Entry(36)  
            {  
                Color = SKColor.Parse(((Color)Application.Current.Resources["PrimaryColor"]).ToHex()),  
                Label = "14.00",  
                ValueLabel = "36",
                ValueLabelColor = SKColors.White 
            },  
            new Entry(27)  
            {  
                Color =  SKColor.Parse(((Color)Application.Current.Resources["PrimaryColor"]).ToHex()),  
                Label = "15.00",
                ValueLabel = "27",
                ValueLabelColor = SKColors.White
            },
        }; 

        public MainPage()
        {
            InitializeComponent();

            // Binding: https://github.com/microcharts-dotnet/Microcharts/issues/30
            TemperatureChart.Chart = new LineChart()
            { 
                Entries = dummyEntries,
                BackgroundColor = SKColors.Transparent,
                LabelOrientation = Orientation.Horizontal,
                LabelColor = SKColors.White,
                LabelTextSize = 40,
                ValueLabelOrientation = Orientation.Horizontal,
                PointSize = 0,
                LineSize = 6
            };
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Clear sensor data callbacks
            ViewModels.MainPageViewModel vm = BindingContext as ViewModels.MainPageViewModel;
        }
    }
}
