
using MQTTnet;
using MQTTnet.Formatter;
using backend.Services;

public class MqttSubscriber : BackgroundService
{
    private readonly GameStateService _gameState;

    public MqttSubscriber(GameStateService gameState)
    {
        _gameState = gameState;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new MqttClientFactory();
        using var client = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .
           TcpServer("138.199.155.78", 1883)
            .WithClientId("csharp-dashboard")
            .WithProtocolVersion(MqttProtocolVersion.V311)
            .Build();

        client.ApplicationMessageReceivedAsync += e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = e.ApplicationMessage.ConvertPayloadToString();

            Console.WriteLine("MQTT message received:");
            Console.WriteLine($"Topic: {topic}");
            Console.WriteLine($"Payload: {payload}");

            // GameStateService is available here.
            // We will process the player and action
            // after confirming the Arduino message format.

            return Task.CompletedTask;
        };

        var connectResult = await client.ConnectAsync(options);

        if (connectResult.ResultCode !=
            MqttClientConnectResultCode.Success)
        {
            throw new InvalidOperationException(
                $"MQTT connection rejected: {connectResult.ResultCode}");
        }

        var subscribeOptions = factory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter("demo/YMJ-Wildcat/#")
            .Build();

        await client.SubscribeAsync(subscribeOptions);

        Console.WriteLine("MQTT listening...");

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Application is shutting down
        }
    }
}
