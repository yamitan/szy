using System.Collections.Concurrent;
using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using szy.WebSockets.Core;
using szy.WebSockets.Models;

namespace szy.WebSockets.Managers
{
    /// <summary>
    /// 管理 WebSocket 连接的实现
    /// </summary>
    public class WebSocketConnectionManager : IWebSocketConnectionManager
    {
        private readonly ILogger<WebSocketConnectionManager> _logger; // 日志记录器
        private readonly ConcurrentDictionary<string, IWebSocketConnection> _connections = new(); // 存储所有 WebSocket 连接

        public WebSocketConnectionManager(ILogger<WebSocketConnectionManager> logger)
        {
            _logger = logger; // 初始化日志记录器
        }

        /// <summary>
        /// 添加新的 WebSocket 连接
        /// </summary>
        /// <param name="socket">要添加的 WebSocket 实例</param>
        /// <returns>返回新添加的 WebSocket 连接</returns>
        public async Task<IWebSocketConnection> AddConnectionAsync(WebSocket socket)
        {
            var connection = new WebSocketConnection(socket); // 创建新的 WebSocket 连接
            _connections.TryAdd(connection.Id, connection); // 将连接添加到字典中
            
            _logger.LogInformation("WebSocket连接已添加，ID: {ConnectionId}，当前连接数: {Count}", 
                connection.Id, _connections.Count); // 记录连接信息
            
            await Task.CompletedTask; // 这里可以是其他异步操作
            return connection; // 返回新连接
        }

        /// <summary>
        /// 移除 WebSocket 连接
        /// </summary>
        /// <param name="id">要移除的连接 ID</param>
        /// <returns>异步任务</returns>
        public Task RemoveConnectionAsync(string id)
        {
            if (_connections.TryRemove(id, out var connection))
            {
                _logger.LogInformation("WebSocket连接已移除，ID: {ConnectionId}，当前连接数: {Count}", 
                    id, _connections.Count); // 记录移除信息
            }
            
            return Task.CompletedTask; // 返回完成的任务
        }

        public Task RemoveConnectionAsync(WebSocket socket)
        {
            var connection = _connections.FirstOrDefault(x => x.Value.Socket == socket);
            if (connection.Key != null)
            {
                return RemoveConnectionAsync(connection.Key);
            }
            
            return Task.CompletedTask;
        }

        public IWebSocketConnection GetConnectionById(string id)
        {
            return _connections.TryGetValue(id, out var connection) ? connection : null;
        }

        public IEnumerable<IWebSocketConnection> GetAllConnections()
        {
            return _connections.Values;
        }

        public async Task BroadcastAsync(string message)
        {
            var tasks = new List<Task>();
            var deadConnections = new List<string>();

            foreach (var connection in _connections)
            {
                try
                {
                    if (connection.Value.Socket.State == WebSocketState.Open)
                    {
                        tasks.Add(connection.Value.SendAsync(message));
                    }
                    else
                    {
                        deadConnections.Add(connection.Key);
                    }
                }
                catch (Exception)
                {
                    deadConnections.Add(connection.Key);
                }
            }

            // 移除已关闭的连接
            foreach (var id in deadConnections)
            {
                await RemoveConnectionAsync(id);
            }

            await Task.WhenAll(tasks);
        }

        public async Task SendToAsync(string connectionId, string message)
        {
            if (_connections.TryGetValue(connectionId, out var connection))
            {
                try
                {
                    await connection.SendAsync(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "发送消息到连接 {ConnectionId} 时出错", connectionId);
                    await RemoveConnectionAsync(connectionId);
                }
            }
        }
    }
} 