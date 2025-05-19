using System.Net.WebSockets;
using Lab8Csharp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// EF Core cu SQLite
builder.Services.AddDbContext<InotContextE>(options =>
    options.UseSqlite("Data Source=inot.db"));

// CORS pt. React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ WebSocketHandler ca Singleton
builder.Services.AddSingleton<WebSocketHandler>();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.UseWebSockets();

// ✅ Endpoint pt. ws://localhost:5243/ws
app.Map("/ws", async context =>
{
    var wsHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
    await wsHandler.HandleWebSocket(context);
});

app.MapControllers();

app.Run();

// ✅ WebSocketHandler definitie
public class WebSocketHandler
{
    private readonly List<WebSocket> _sockets = new();

    public async Task HandleWebSocket(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            _sockets.Add(webSocket);

            while (webSocket.State == WebSocketState.Open)
                await Task.Delay(1000);

            _sockets.Remove(webSocket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }

    public async Task BroadcastAsync(string message)
    {
        var buffer = System.Text.Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(buffer);

        foreach (var socket in _sockets.Where(s => s.State == WebSocketState.Open))
        {
            await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
