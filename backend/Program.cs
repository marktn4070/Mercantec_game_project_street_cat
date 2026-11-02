using MQTTnet;
using MQTTnet.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var factory = new MqttFactory();
var client = factory.CreateMqttClient();

var options = new MqttClientOptionsBuilder()
    .WithTcpServer("138.199.155.78", 1883)
    .WithClientId("csharp-dashboard")
    .Build();

client.ApplicationMessageReceivedAsync += e => {
    var topic   = e.ApplicationMessage.Topic;
    var payload = e.ApplicationMessage.ConvertPayloadToString();
    Console.WriteLine($"{topic}: {payload}");
    return Task.CompletedTask;
};

await client.ConnectAsync(options);
await client.SubscribeAsync("demo/opla/#");
Console.WriteLine("Lytter…");
Console.ReadLine();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.Run();

