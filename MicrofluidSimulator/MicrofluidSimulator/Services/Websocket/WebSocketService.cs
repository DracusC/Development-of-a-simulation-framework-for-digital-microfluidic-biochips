using System;
using System.Net.WebSockets;
using System.Text;
using MicrofluidSimulator.SimulatorCode.DataTypes;

namespace MicrofluidSimulator.Services.Websocket;
public class WebSocketService : IAsyncDisposable
{
    private ClientWebSocket _webSocket;

    public async Task ConnectAsync(Uri uri)
    {
        _webSocket = new ClientWebSocket();
        Console.WriteLine("Trying to connect to WebSocket server.");
        await _webSocket.ConnectAsync(uri, CancellationToken.None);
        Console.WriteLine("Connected to WebSocket server.");
    }

    public class WebSocketMessage<T>
    {
        public WebSocketMessage(Guid requestId, string type, T data)
        {
            RequestId = requestId;
            Type = type;
            Data = data;
        }

        public Guid RequestId { get; set; }
        public string? Type { get; set; }
        public T? Data { get; set; }
    }

    public class SensorRequest
    {
        public int Id { get; set; }
        public decimal Time { get; set; }
        public override string ToString()
        {
            return $"SensorRequest: {{ SensorId: {Id} }}";
        }
    }


    //public class Point
    //{
    //    public Point(int x, int y)
    //    {
    //        X = x;
    //        Y = y;
    //    }

    //    public int X { get; set; }
    //    public int Y { get; set; }
    //}

    //public class Droplet
    //{
    //    public Droplet(string name)
    //    {
    //        Name = name;
    //    }
    //    public string Name { get; set; }
    //}

    //public void Move(Droplet d, Point p)
    //{

    //}

    //public void Test()
    //{
    //    Droplet wrong = new Droplet("point");

    //    Droplet d = new Droplet("alex");
    //    Move(d, new Point(10, 10));

    //    Move(d, wrong);
    //}


    public async Task<object> ReceiveMessageAsync()
    {
        var buffer = new byte[1024 * 32];
        var messageBuilder = new StringBuilder();

        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            else
            {
                var chunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine(chunk);
                messageBuilder.Append(chunk);

                if (result.EndOfMessage)
                {
                    string fullMessage = messageBuilder.ToString();

                    var baseMessage = Utf8Json.JsonSerializer.Deserialize<WebSocketMessage<object>>(fullMessage);

                    switch (baseMessage.Type)
                    {
                        case "action":
                            var actionData = Utf8Json.JsonSerializer.Deserialize<WebSocketMessage<Queue<ActionQueueItem>>>(fullMessage);
                            return actionData;

                        case "sensor_request":
                            var sensorData = Utf8Json.JsonSerializer.Deserialize<WebSocketMessage<SensorRequest>>(fullMessage);
                            return sensorData;

                        default:
                            throw new InvalidOperationException("Unknown message type received.");
                    }
                }
            }
        }

        return null;
    }



    public async Task<ActionQueueItem> ReceiveActionQueueItemAsync()
    {
        var buffer = new byte[1024 * 4];
         
        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            else
            {
                var serializedObj = Encoding.UTF8.GetString(buffer, 0, result.Count);
                //Console.WriteLine($"Received JSON: {serializedObj}");

                ActionQueueItem action = Utf8Json.JsonSerializer.Deserialize<ActionQueueItem>(serializedObj);

                //Console.WriteLine($"Deserialized: {action.ToString()}");

                return action;
            }
        }

        return null;
    }


    public async Task SendMessageAsync(string message)
    {
        if (_webSocket.State != WebSocketState.Open)
            throw new InvalidOperationException("WebSocket connection is not open.");

        var bytes = Encoding.UTF8.GetBytes(message);
        await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async ValueTask DisposeAsync()
    {
        if (_webSocket != null)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disposing", CancellationToken.None);
            _webSocket.Dispose();
        }
    }
}