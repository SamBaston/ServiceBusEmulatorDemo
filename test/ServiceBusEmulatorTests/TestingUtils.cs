using Azure.Messaging.ServiceBus;

namespace ServiceBusEmulatorTests;

public class TestingUtils
{
    public static async Task ClearSubscriptionAsync(
        string topicName, 
        string subscriptionName, 
        ServiceBusClient client)
    {
        var cleanupReceiver = client.CreateReceiver(
            topicName, 
            subscriptionName, 
            new ServiceBusReceiverOptions { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete});

        while (true)
        {
            var batch = await cleanupReceiver.ReceiveMessagesAsync(
                maxMessages: 50,
                maxWaitTime: TimeSpan.FromSeconds(5));

            if (batch.Count == 0) break;
        }
    }
}
