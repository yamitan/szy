using Microsoft.AspNetCore.Http;
using szy.WebSockets.Handlers;

namespace szy.Middleware
{
    /// <summary>
    /// API 监控中间件，用于记录所有 API 访问并通过 WebSocket 广播
    /// </summary>
    public class ApiMonitorMiddleware(RequestDelegate next, ApiMonitorHandler apiMonitorHandler)
    {
        private readonly RequestDelegate _next = next;
        private readonly ApiMonitorHandler _apiMonitorHandler = apiMonitorHandler;

        public async Task InvokeAsync(HttpContext context)
        {
            // 获取请求方法和路径
            string method = context.Request.Method;
            string path = context.Request.Path;

            // 记录 API 访问并通过 WebSocket 广播
            await _apiMonitorHandler.LogApiAccessAsync(method, path);

            // 调用管道中的下一个中间件
            await _next(context);
        }
    }

    // 扩展方法，方便在 Program.cs 中使用
    public static class ApiMonitorMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiMonitor(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiMonitorMiddleware>();
        }
    }
}