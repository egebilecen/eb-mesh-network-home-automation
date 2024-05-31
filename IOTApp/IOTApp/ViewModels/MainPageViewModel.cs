using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace IOTApp.ViewModels
{
    public class MainPageViewModel : BindableObject, INotifyPropertyChanged
    {
        //JObject meshInfo = dataJSON.Value<JObject>("data");
        //Models.SensorData sensorData = dataJSON.Value<JObject>("data").ToObject<Models.SensorData>();

        private double _temperature = 0;
        public double Temperature
        {
            get => _temperature;
            set
            {
                if(value == _temperature) return;

                _temperature = value;
                OnPropertyChanged();
            }
        }

        private bool _isLambToggled = false;
        public bool IsLambToggled
        {
            get => _isLambToggled;
            set
            {
                if(value == _isLambToggled) return;

                _isLambToggled = value;
                OnPropertyChanged();

                _ = IOTClient.Instance.SendDataAsync(new {
                    nodeID = 1,
                    state  = IsLambToggled ? 1 : 0
                });
            }
        }

        private bool _isLambToggled2 = false;
        public bool IsLambToggled2
        {
            get => _isLambToggled2;
            set
            {
                if(value == _isLambToggled2) return;

                _isLambToggled2 = value;
                OnPropertyChanged();

                _ = IOTClient.Instance.SendDataAsync(new {
                    nodeID = 2,
                    state  = IsLambToggled2 ? 1 : 0
                });
            }
        }

        public MainPageViewModel()
        {
            ButtonClick = new Command(OnClick);

            IOTClient.Instance.DataHandlers["node_data"] = (dataJSON) =>
            {
                if(!GetType().Name.Contains(NavigationUtil.GetCurrentPageClassName()))
                    return;

                int     nodeType   = dataJSON.Value<int>("nodeType");
                JObject sensorData = dataJSON.Value<JObject>("value");

                if((nodeType & NodeTypedef.TEMPERATURE_SENSOR_LM35) != 0)
                {
                    double temperature = sensorData.Value<double>(NodeTypedef.TEMPERATURE_SENSOR_LM35.ToString());
                    Temperature = temperature;
                }
            };
        }

        public ICommand ButtonClick { get; }
        public void OnClick()
        {
            _ = IOTClient.Instance.SendDataAsync(new {
                nodeID = 1,
                state  = IsLambToggled ? 1 : 0
            });
        }
    }
}
