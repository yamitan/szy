using System.Net.WebSockets;

namespace szy.WebSockets.Core
{
    /// <summary>
    /// WebSocket消息处理器接口
    /// </summary>
    public interface IWebSocketMessageHandler
    {
        Task HandleMessageAsync(WebSocketReceiveResult result, byte[] buffer, IWebSocketConnection connection);
    }
} 