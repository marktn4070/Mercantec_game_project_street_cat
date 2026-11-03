
using backend.Services;
using MQTTnet;
using MQTTnet.Formatter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

// HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseSession();

app.MapControllers();

app.Run();
