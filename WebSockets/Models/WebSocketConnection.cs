using System.Net.WebSockets;
using System.Text;
using szy.WebSockets.Core;

namespace szy.WebSockets.Models
{
    /// <summary>
    /// 表示一个 WebSocket 连接的实现
    /// </summary>
    public class WebSocketConnection : IWebSocketConnection
    {
        public WebSocketConnection(WebSocket socket)
        {
            Socket = socket; // 初始化 WebSocket 实例
            Id = Guid.NewGuid().ToString(); // 生成唯一连接 ID
        }

        /// <summary>
        /// 获取连接的唯一标识符
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// 获取 WebSocket 实例
        /// </summary>
        public WebSocket Socket { get; }

        /// <summary>
        /// 获取连接的时间戳
        /// 该属性表示连接建立的时间。当前实现抛出 NotImplementedException，您可以根据需要实现该属性。
        /// </summary>
        public DateTime ConnectedAt => throw new NotImplementedException();

        /// <summary>
        /// 发送消息到 WebSocket 连接
        /// </summary>
        /// <param name="message">要发送的消息</param>
        public async Task SendAsync(string message)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(message); // 将消息编码为字节数组
            var segment = new ArraySegment<byte>(buffer); // 创建字节数组段
            await Socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None); // 发送消息
        }
    }
} 