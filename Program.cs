using szy.Middleware;
using szy.WebSockets.Core;
using szy.WebSockets.Handlers;
using szy.WebSockets.Managers;
using szy.WebSockets.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 添加 CORS 服务
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 注册定时服务
builder.Services.AddHostedService<szy.Services.TimerService>();

// 注册WebSocket服务
builder.Services.AddSingleton<IWebSocketConnectionManager, WebSocketConnectionManager>();
builder.Services.AddSingleton<IWebSocketMessageHandler, DefaultWebSocketHandler>();
builder.Services.AddSingleton<ApiMonitorHandler>();

// 注册数据库服务
builder.Services.AddSingleton<szy.Services.DatabaseService>();

// 注册配置服务
builder.Services.AddSingleton<szy.Services.ConfigService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 启用 CORS
app.UseCors("AllowAll");

// 启用WebSocket
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
});

// 使用完全限定名调用方法
app.UseMiddleware<WebSocketServerMiddleware>();

// 添加 API 访问监控中间件
app.UseApiMonitor();

// 启用静态文件
app.UseStaticFiles();
// websocket测试页面 https://localhost:port/websocket-test.html

// 添加 API 访问日志中间件（放在 MapControllers 之前）

app.UseAuthorization();
app.MapControllers();

app.Run();
