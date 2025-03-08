using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using szy.WebSockets.Core;
using szy.WebSockets.Handlers;

namespace szy.WebSockets.Middleware
{
    /// <summary>
    /// WebSocket 服务器中间件，用于处理 WebSocket 连接
    /// </summary>
    public class WebSocketServerMiddleware(
        RequestDelegate next,
        ILogger<WebSocketServerMiddleware> logger,
        IWebSocketConnectionManager connectionManager,
        IWebSocketMessageHandler messageHandler)
    {
        private readonly RequestDelegate _next = next; // 下一个中间件
        private readonly ILogger<WebSocketServerMiddleware> _logger = logger; // 日志记录器
        private readonly IWebSocketConnectionManager _connectionManager = connectionManager; // WebSocket 连接管理器
        private readonly IWebSocketMessageHandler _messageHandler = messageHandler; // WebSocket 消息处理器

        /// <summary>
        /// 处理 WebSocket 请求
        /// </summary>
        /// <param name="context">HTTP 上下文</param>
        public async Task InvokeAsync(HttpContext context)
        {
            // 检查请求路径是否为 WebSocket 连接
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest) // 检查是否为 WebSocket 请求
                {
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync(); // 接受 WebSocket 请求
                    var connection = await _connectionManager.AddConnectionAsync(webSocket); // 添加连接

                    await HandleWebSocketAsync(webSocket, connection); // 处理 WebSocket 连接
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest; // 非 WebSocket 请求返回 400
                }
            }
            else
            {
                await _next(context); // 调用下一个中间件
            }
        }

        /// <summary>
        /// 处理 WebSocket 消息
        /// </summary>
        /// <param name="webSocket">WebSocket 实例</param>
        /// <param name="connection">WebSocket 连接</param>
        private async Task HandleWebSocketAsync(WebSocket webSocket, IWebSocketConnection connection)
        {
            var buffer = new byte[1024 * 4]; // 创建接收缓冲区

            try
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None); // 接收消息

                while (!result.CloseStatus.HasValue) // 处理接收到的消息
                {
                    await _messageHandler.HandleMessageAsync(result, buffer, connection); // 处理消息

                    result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), CancellationToken.None); // 继续接收消息
                }

                await webSocket.CloseAsync(
                    result.CloseStatus.Value,
                    result.CloseStatusDescription,
                    CancellationToken.None); // 关闭 WebSocket 连接
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理 WebSocket 连接时出错，连接ID: {ConnectionId}", connection.Id); // 记录错误
            }
            finally
            {
                await _connectionManager.RemoveConnectionAsync(webSocket); // 移除连接
            }
        }
    }

    // 扩展方法
    public static class WebSocketServerMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebSocketServer(this IApplicationBuilder app)
        {
            return app.UseMiddleware<WebSocketServerMiddleware>(); // 注册中间件
        }
    }
}