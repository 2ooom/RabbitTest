using System;
using System.Text;
using RabbitMQ.Client;

namespace LogPublisher
{
    class Program
    {
        private static string exchangeName = "direct_logs";
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchangeName, "direct");
                    while (true)
                    {
                        Console.WriteLine("input message");
                        var line = Console.ReadLine();
                        if (string.IsNullOrEmpty(line) || line.IndexOf(' ') < 0)
                        {
                            Console.WriteLine("Format is: {Seveirity} {Message}");
                            continue;
                        }
                        var delimImndex = line.IndexOf(' ');
                        var severity = line.Substring(0, delimImndex);
                        var msg = line.Substring(delimImndex + 1);
                        var body = Encoding.UTF8.GetBytes(msg);
                        channel.BasicPublish(exchangeName, severity, null, body);
                        Console.WriteLine(" ** [{0}s {1}ms] [{3}] Send message: {2}", DateTime.Now.Second, DateTime.Now.Millisecond, msg, severity);
                    }
                }
            }
        }
    }
}
