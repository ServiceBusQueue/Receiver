using Azure.Messaging.ServiceBus;

class Program
{
    private const string connectionString = "<ConnectioonString />";
    private const string queueName = "<QueuName />";
    static async Task Main(string[] args)
    {
        await ReceiveMessagesAsync();
    }

    static async Task ReceiveMessagesAsync()
    {
        // Create a Service Bus client
        await using var client = new ServiceBusClient(connectionString);
        // Create a processor for the queue
        ServiceBusProcessor processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

        // Add a handler to process messages
        processor.ProcessMessageAsync += MessageHandler;
        processor.ProcessErrorAsync += ErrorHandler;

        // Start processing
        await processor.StartProcessingAsync();

        Console.WriteLine("Receiving messages... Press any key to stop.");
        Console.ReadKey();

        // Stop processing
        await processor.StopProcessingAsync();
    }

    static async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        Console.WriteLine($"Received: {body}");

        // Complete the message so it is not received again
        await args.CompleteMessageAsync(args.Message);
    }

    static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine($"Message handler encountered an error: {args.Exception.Message}");
        return Task.CompletedTask;
    }
}
