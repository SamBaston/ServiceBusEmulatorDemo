using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace ServiceBusEmulatorTests;

[TestClass]
public sealed class MessageTests
{
    private const string SenderTopic = "Sender";
    private const string ReceiverTopic = "Receiver";
    private const string SubscriptionName = "Subscription";

    private ServiceBusClient _serviceBusclient;
    private ServiceBusSender _sender;
    private ServiceBusReceiver _receiver;

    [TestInitialize]
    public async Task Setup()
    {
        var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.local.json");
        Assert.IsTrue(File.Exists(configPath), $"Missing config file: {configPath}");

        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile(configPath, optional: false)
            .Build();

        _serviceBusclient = new ServiceBusClient(config.GetConnectionString("DefaultConnection"));
        _sender = _serviceBusclient.CreateSender(SenderTopic);
        _receiver = _serviceBusclient.CreateReceiver(ReceiverTopic);

        await TestingUtils.ClearSubscriptionAsync(SenderTopic, SubscriptionName, _serviceBusclient);
        await TestingUtils.ClearSubscriptionAsync(ReceiverTopic, SubscriptionName, _serviceBusclient);
    }

    [TestMethod]
    public async Task CheckSendingMessages()
    {
        // Arrange
        var body = "Test message";
        var subject = "Test Subject";
        var message = new ServiceBusMessage(body)
        {
            Subject = subject
        };

        // Act
        await _sender.SendMessageAsync(message);

        var receivedMessage = await _receiver.ReceiveMessageAsync(maxWaitTime : TimeSpan.FromMinutes(2));  

        // Assert
        Assert.IsNotNull(receivedMessage, "No message was received.");
        Assert.AreEqual(body, receivedMessage.Body.ToString(), "The received message's body does not match what was sent.");
        Assert.AreEqual(subject, receivedMessage.Subject, "The received message's subject does not match what was sent.");

        await _receiver.CompleteMessageAsync(receivedMessage);
    }
}
