using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace IOTApp
{
    public sealed class IOTClient : BindableObject, INotifyPropertyChanged
    {
        private static readonly object _lock = new object();
        private static bool _initialized = false;

        private static IOTClient _instance = null;
        public static IOTClient Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock(_lock)
                    {
                        if(_instance == null)
                        {
                            _instance = new IOTClient();
                        }
                    }
                }

                return _instance;
            }
        }

        private bool _isConnected = false;
        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if(_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _blink = false;
        public bool Blink
        {
            get => _blink;
            private set
            {
                _blink = value;
                OnPropertyChanged();
            }
        }

        public Dictionary<string, Action<JObject>> DataHandlers { get; private set; } =
            new Dictionary<string, Action<JObject>>();

        public void Init()
        {
            if(_initialized) return;

            Websocket.OnConnected = () => {
                IsConnected = true;
                
                if(NavigationUtil.GetCurrentPageClassName() == "ConnectingPage")
                    Application.Current.MainPage = new NavigationPage(new Views.MainPage());
            };

            Websocket.OnDisconnected = () =>
            {
                IsConnected = false;
                Application.Current.MainPage.DisplayAlert("Uyarı", "Sunucu ile olan bağlantı kesildi. Tekrar bağlanılmaya çalışılıyor... Bağlantı durumunu sağ üst köşeden kontrol edebilirsiniz.", "Tamam");
                //Application.Current.MainPage = new Views.ConnectingPage();
            };

            Websocket.OnData = (data) =>
            {
                if(NavigationUtil.GetCurrentPageClassName() == "ConnectionPage") return;
                NavigationPage navPage = Application.Current.MainPage as NavigationPage;

                try
                {
                    string currentPage = navPage.RootPage.GetType().Name;
                    JObject dataJSON = JObject.Parse(data);

                    if(DataHandlers.ContainsKey(dataJSON.Value<string>("type")))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DataHandlers[dataJSON.Value<string>("type")](dataJSON.Value<JObject>("data"));
                        });
                    }
                }
                catch(Exception)
                {}
            };

            _ = Websocket.ConnectAsync();
            _initialized = true;
        }

        public void DoBlink()
        {
            Blink = false;
            Blink = true;
        }

        public async Task SendDataAsync(object data)
        {
            if(!Instance.IsConnected) return;

            JObject json = new JObject
            {
                { "type", "app_data" },
                { "data", JToken.FromObject(data) }
            };

            await Websocket.SendDataAsync(JsonConvert.SerializeObject(json));
        }
    }
}
