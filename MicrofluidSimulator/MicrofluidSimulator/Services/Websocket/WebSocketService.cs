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


    public async Task<string> ReceiveMessagesAsync()
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
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                // Process the message (you could raise an event or notify a callback here)
                return message;
            }
        }

        return "ERROR: NO MESSAGE";
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