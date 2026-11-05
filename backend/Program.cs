
using backend.Services;
using MQTTnet;
using MQTTnet.Formatter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("https://mk.trp.solutions")
              .WithOrigins("http://127.0.0.1")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Session State
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Game State
builder.Services.AddSingleton<GameStateService>();

// MQTT background service
builder.Services.AddHostedService<MqttSubscriber>();

builder.Services.AddOpenApi();

var app = builder.Build();
var gameState = app.Services.GetRequiredService<GameStateService>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseSession();

app.MapControllers();

app.Run();
