using System;
using System.Text;
using RabbitMQ.Client;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("hello", false, false, false, null);
                    while (true)
                    {
                        Console.WriteLine("Press a key");
                        var msg = Console.ReadLine();
                        var body = Encoding.UTF8.GetBytes(msg);
                        channel.BasicPublish("", "hello", null, body);
                        Console.WriteLine(" ** [{0}s {1}ms] Send message: {2}", DateTime.Now.Second, DateTime.Now.Millisecond, msg);
                    }
                }
            }
        }
    }
}
