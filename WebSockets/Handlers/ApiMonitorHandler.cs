using System;
using System.Text.Json;
using System.Threading.Tasks;
using szy.WebSockets.Core;

namespace szy.WebSockets.Handlers
{
    /// <summary>
    /// API 监控处理器，用于通过 WebSocket 广播 API 访问信息
    /// </summary>
    public class ApiMonitorHandler
    {
        private readonly IWebSocketConnectionManager _connectionManager;

        public ApiMonitorHandler(IWebSocketConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        /// <summary>
        /// 记录 API 访问并通过 WebSocket 广播
        /// </summary>
        /// <param name="method">HTTP 方法</param>
        /// <param name="path">请求路径</param>
        public async Task LogApiAccessAsync(string method, string path)
        {
            try
            {
                var apiInfo = new
                {
                    Type = "ApiAccess",
                    Timestamp = DateTime.Now,
                    Method = method,
                    Path = path
                };

                string message = JsonSerializer.Serialize(apiInfo);
                
                // 广播消息给所有连接的客户端
                await _connectionManager.BroadcastAsync(message);
            }
            catch (Exception ex)
            {
                // 记录异常但不中断请求处理
                Console.WriteLine($"API 监控广播失败: {ex.Message}");
            }
        }
    }
} 