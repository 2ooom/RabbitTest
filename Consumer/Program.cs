using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("hello", false, false, false, null);
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicQos(0, 1, false);
                    channel.BasicConsume("hello", false, consumer);

                    Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

                    while (true)
                    {
                        var packet = consumer.Queue.Dequeue();
                        var body = packet.Body;
                        var msg = Encoding.UTF8.GetString(body);
                        var sleep = msg.Split('.').Length;
                        Console.WriteLine(" [x] Working on message: [{0}] at {1}s {2}ms", msg, DateTime.Now.Second, DateTime.Now.Millisecond);
                        Thread.Sleep(sleep * 1000);
                        channel.BasicAck(packet.DeliveryTag, false);
                        Console.WriteLine(" [x] Message processed");
                    }
                }
            }
        }
    }
}
