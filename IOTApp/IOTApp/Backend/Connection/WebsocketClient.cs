using System;
using System.Collections.Generic;
using System.Text;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;

namespace IOTApp
{
    public static class Websocket
    {
        public  static ClientWebSocket websocket  = new ClientWebSocket();
        public  static uint            bufferSize = 4 * 1024;
        private static CancellationToken cancellationToken = new CancellationToken();

        public static Action         OnConnected    = null;
        public static Action         OnDisconnected = null;
        public static Action<string> OnData         = null;

        private static async Task ConnectionCheckerAsync()
        {
            while(IsConnected())
                await Task.Delay(2000);

            //Console.WriteLine("Disconnected from server!");
            Device.BeginInvokeOnMainThread(() =>
            {
                OnDisconnected?.Invoke();
            });

            _ = ConnectAsync();
        }

        private static async Task DataHandlerAsync()
        {
            while(IsConnected())
            {
                WebSocketReceiveResult result;
                var buffer = new ArraySegment<byte>(new byte[bufferSize]);

                do
                {
                    result = await websocket.ReceiveAsync(buffer, cancellationToken);

                    if(result.MessageType != WebSocketMessageType.Text)
                        break;

                    var bytes   = buffer.Skip(buffer.Offset).Take(result.Count).ToArray();
                    string data = Encoding.UTF8.GetString(bytes);
                    IOTClient.Instance.DoBlink();

                    OnData?.Invoke(data);
                }
                while(!result.EndOfMessage);
            }
        }

        public static bool IsConnected()
        {
            return websocket.State == WebSocketState.Open;
        }

        public static async Task ConnectAsync()
        {
            while(!IsConnected())
            {
                //Console.WriteLine("Connecting to server...");

                try
                {
                    await websocket.ConnectAsync(new Uri("ws://" + Settings.serverIP + ":" + Settings.serverPort.ToString()), new CancellationTokenSource(2000).Token);
                }
                catch(Exception)
                {
                    websocket.Dispose();
                    websocket = new ClientWebSocket();
                    continue;
                }
            }

            //Console.WriteLine("Connected to server!");
            Device.BeginInvokeOnMainThread(() =>
            {
                OnConnected?.Invoke();
            });

            _ = SendDataAsync("ping");
            _ = ConnectionCheckerAsync();
            _ = DataHandlerAsync();
        }

        public static async Task SendDataAsync(string data)
        {
            if(!IsConnected()) return;

            var byteData    = Encoding.UTF8.GetBytes(data);
            var dataSegment = new ArraySegment<byte>(byteData);
            await websocket.SendAsync(dataSegment, WebSocketMessageType.Text, true, cancellationToken);
        }
    }
}
