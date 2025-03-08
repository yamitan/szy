using System.Net.WebSockets;

namespace szy.WebSockets.Core
{
    /// <summary>
    /// 表示WebSocket连接的接口
    /// </summary>
    public interface IWebSocketConnection
    {
        string Id { get; }
        WebSocket Socket { get; }
        DateTime ConnectedAt { get; }
        Task SendAsync(string message);
    }
} 