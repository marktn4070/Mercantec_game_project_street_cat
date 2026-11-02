using MQTTnet;
using MQTTnet.Formatter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var factory = new MqttClientFactory();
using var client = factory.CreateMqttClient();

var options = new MqttClientOptionsBuilder()
    .WithTcpServer("138.199.155.78", 1883)
    .WithClientId("csharp-dashboard")
    .WithProtocolVersion(MqttProtocolVersion.V311)
    .Build();

client.ApplicationMessageReceivedAsync += e =>
{
    var topic = e.ApplicationMessage.Topic;
    var payload = e.ApplicationMessage.ConvertPayloadToString();
    Console.WriteLine($"{topic}: {payload}");
    return Task.CompletedTask;
};

var connectResult = await client.ConnectAsync(options);
if (connectResult.ResultCode != MqttClientConnectResultCode.Success)
{
    throw new InvalidOperationException($"MQTT-forbindelse afvist ({connectResult.ResultCode}).");
}

var subscribeOptions = factory.CreateSubscribeOptionsBuilder()
    .WithTopicFilter("demo/opla/#")
    .Build();

await client.SubscribeAsync(subscribeOptions);
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

