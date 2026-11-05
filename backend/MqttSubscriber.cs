
using MQTTnet;
using MQTTnet.Formatter;
using backend.Services;
using System.Text.Json;
using backend.Models;

public class MqttSubscriber : BackgroundService
{
    private readonly GameStateService _gameState;
    private int _nextPlayerId = 1;

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
            .WithTcpServer("138.199.155.78", 1883)
            .WithClientId($"backend-{Guid.NewGuid()}")
            .WithProtocolVersion(MqttProtocolVersion.V311)
            .Build();

        client.ApplicationMessageReceivedAsync += async e =>
        {
            Console.WriteLine("EVENT FIRED");

            var topic = e.ApplicationMessage.Topic;
            var payload = e.ApplicationMessage.ConvertPayloadToString();

            Console.WriteLine($"Topic: {topic}");
            Console.WriteLine($"Payload: {payload}");

            if (topic == "demo/YMJ-Wildcat/join")
            {
                var request = JsonSerializer.Deserialize<JoinRequest>(payload, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var deviceId = request!.DeviceId;
                var pin = request.Pin;
                var assignedPlayerId = $"player{_nextPlayerId++}";
                
                await client.PublishAsync(new MqttApplicationMessageBuilder()
                    .WithTopic($"demo/YMJ-Wildcat/{deviceId}/assigned")
                    .WithPayload(assignedPlayerId)
                    .Build(), stoppingToken);
                Console.WriteLine($"Assigned {assignedPlayerId} to {deviceId}");
            }
        };
        client.ConnectedAsync += e =>
        {
            Console.WriteLine("MQTT CONNECTED");
            return Task.CompletedTask;
        };

        client.DisconnectedAsync += e =>
        {
            Console.WriteLine("MQTT DISCONNECTED");
            Console.WriteLine($"Reason: {e.Reason}");

            if (e.Exception != null)
            {
                Console.WriteLine($"Exception: {e.Exception.Message}");
            }

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

        var result = await client.SubscribeAsync(subscribeOptions);
        foreach (var item in result.Items)
        {
            Console.WriteLine(
                $"Subscribed to {item.TopicFilter.Topic}. Result={item.ResultCode}");
        }

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
