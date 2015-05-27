using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace LogReceiver
{
    class Program
    {
        private static string exchangeName = "direct_logs";
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Specify at severity to subscribe for");
                return;
            }
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchangeName, "direct");

                    var queueName = channel.QueueDeclare().QueueName;
                    foreach (var severity in args)
                    {
                        channel.QueueBind(queueName, exchangeName, severity);
                        Console.WriteLine(" [i] Subscribing for log messages of severity [{0}]", severity);
                    }
                    
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(queueName, true, consumer);

                    Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

                    while (true)
                    {
                        var packet = consumer.Queue.Dequeue();
                        var body = packet.Body;
                        var msg = Encoding.UTF8.GetString(body);
                        var sleep = msg.Split('.').Length;
                        Console.WriteLine(" [x] [{3}] Working on message: [{0}] at {1}s {2}ms", msg, DateTime.Now.Second, DateTime.Now.Millisecond, packet.RoutingKey);
                        Thread.Sleep(sleep * 1000);
                        Console.WriteLine(" [x] Message processed");
                    }
                }
            }
        }
    }
}
