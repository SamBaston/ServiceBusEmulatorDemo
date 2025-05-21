using System.Text.Json;
using Azure.Messaging.ServiceBus;

var serviceBusClient = new ServiceBusClient("Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;");

var sender = serviceBusClient.CreateSender("Receiver");

var message = new DemoMessage()
{
    Id = Guid.NewGuid(),
    Message = "Test Message"
};

var body = new BinaryData(JsonSerializer.Serialize(message));
var serviceBusMessage = new ServiceBusMessage(body);

await sender.SendMessageAsync(serviceBusMessage);
Console.WriteLine("Message sent");

var processor = serviceBusClient.CreateProcessor("Receiver", "Subscription");
processor.ProcessMessageAsync += (args) => {

    var receivedMessage = JsonSerializer.Deserialize<DemoMessage>(args.Message.Body);
    Console.WriteLine($"Message received: {receivedMessage.Message}");
    return Task.CompletedTask;
};
processor.ProcessErrorAsync += (args) =>
{
    Console.WriteLine($"Error occurred: {args.Exception.Message}");
    return Task.CompletedTask;
};

await processor.StartProcessingAsync();
Console.WriteLine("Message processing started");

while (true)
{
    await Task.Delay(1000);
}


public class DemoMessage
{
    public Guid Id { get; set; }
    public string Message { get; set; }
}