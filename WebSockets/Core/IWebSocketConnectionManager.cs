using System.Net.WebSockets;

namespace szy.WebSockets.Core
{
    /// <summary>
    /// WebSocket连接管理器接口
    /// </summary>
    public interface IWebSocketConnectionManager
    {
        Task<IWebSocketConnection> AddConnectionAsync(WebSocket socket);
        Task RemoveConnectionAsync(string id);
        Task RemoveConnectionAsync(WebSocket socket);
        IWebSocketConnection GetConnectionById(string id);
        IEnumerable<IWebSocketConnection> GetAllConnections();
        Task BroadcastAsync(string message);
        Task SendToAsync(string connectionId, string message);
    }
} 