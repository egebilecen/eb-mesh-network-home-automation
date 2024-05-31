using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;

namespace IOTApp
{
    public sealed class IOTClient : BindableObject, INotifyPropertyChanged
    {
        private static readonly object _lock = new object();
        private static bool initialized = false;

        private static IOTClient instance = null;
        public static IOTClient Instance
        {
            get
            {
                if(instance == null)
                {
                    lock(_lock)
                    {
                        if(instance == null)
                        {
                            instance = new IOTClient();
                        }
                    }
                }

                return instance;
            }
        }

        private bool isConnected = false;
        public bool IsConnected
        {
            get => isConnected;
            private set
            {
                if(isConnected != value)
                {
                    isConnected = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool blink = false;
        public bool Blink
        {
            get => blink;
            private set
            {
                blink = value;
                OnPropertyChanged();
            }
        }

        public void Init()
        {
            if(initialized) return;

            Websocket.OnConnected = () => {
                //Console.WriteLine("OnConnected");
                IsConnected = true;
                Application.Current.MainPage = new NavigationPage(new Views.MainPage());
            };

            Websocket.OnDisconnected = () =>
            {
                //Console.WriteLine("OnDisconnected");
                IsConnected = false;
                //Application.Current.MainPage = new Views.ConnectingPage();
            };

            Websocket.OnData = (data) =>
            {
                if(Application.Current.MainPage.GetType().Name != "NavigationPage") return;
                NavigationPage navPage = Application.Current.MainPage as NavigationPage;

                try
                {
                    string currentPage = navPage.RootPage.GetType().Name;
                    JObject dataJSON = JObject.Parse(data);

                    switch(dataJSON.Value<string>("type"))
                    {
                        case "sensor_data":
                        {
                            Models.SensorData sensorData = dataJSON.Value<JObject>("data").ToObject<Models.SensorData>();
                        }
                        break;

                        case "mesh_info":
                        {
                            JObject meshInfo = dataJSON.Value<JObject>("data");
                        }
                        break;
                    }
                }
                catch(Exception ex)
                {}
            };

            _ = Websocket.ConnectAsync();
            initialized = true;
        }

        public void DoBlink()
        {
            Blink = false;
            Blink = true;
        }

        public async Task SendDataAsync(string data)
        {
            await Websocket.SendDataAsync(data);
        }
    }
}
