using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace AZServiceBusReceive
{
    class Program
    {
        static string connectionString = "Your ServiceBusConnectionString Or SAS Token";

        static string topicName = "Your Topic Name";

        static string subscriptionName = "Your Subscription Name";

        static ServiceBusClient client;

        static ServiceBusProcessor processor;
        static async Task Main(string[] args)
        {
            client = new ServiceBusClient(connectionString);

            processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

            try
            {
                processor.ProcessMessageAsync += MessageHandler;

                processor.ProcessErrorAsync += ErrorHandler;

                await processor.StartProcessingAsync();

                Console.WriteLine("Press any key to end processing");
                Console.ReadLine();

                Console.WriteLine("\n Stopping the receiver");
                await processor.StartProcessingAsync();
                Console.WriteLine("Stopeed receiving messages");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.ToString()}");
            }
            finally
            {
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();

            Console.WriteLine($"Received: {body} from subscription: {subscriptionName}");

            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());

            return Task.CompletedTask;
        }
    }
}
