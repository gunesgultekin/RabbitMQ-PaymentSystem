using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace NotificationService
{
    [ApiController]
    [Route("/api/[controller]")]
    public class NotificationServiceController : ControllerBase
    {
        private ConnectionFactory _connectionFactory;

        public NotificationServiceController()
        {
            _connectionFactory = new ConnectionFactory { HostName = "localhost" };

        }

        // Send notifications to devices according to parameter information from the notifications queue
        // deviceId parameter can be compared to the device token in services such as Firebase cloud messaging.
        [HttpGet("sendNotification")]
        public async Task<ActionResult> sendNotification()
        {
            try
            {
                // Connect to local RabbitMQ server
                using var connection = _connectionFactory.CreateConnection();
                using var channel = connection.CreateModel();

                var consumer = new EventingBasicConsumer(channel);

                // Set what to do when messages came from "Notifications" Queue
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    // Split message according to space character
                    // [0] => deviceId
                    // [1] => status
                    string[] splittedParams = message.Split(" ");
                    // Check status of payment
                    if (splittedParams[1] == "succesful")
                    {
                        Console.WriteLine($"* Payment Successful for DeviceID {splittedParams[0]}");

                    }
                    else
                    {
                        Console.WriteLine($"* Payment Failed for DeviceID {splittedParams[0]}");

                    }
                    // Imitate async operations
                    await Task.Delay(2000);
                };

                channel.BasicConsume(
                   queue: "Notifications",
                   autoAck: true,
                   consumer: consumer
                   );

                return Ok("All notifications sent to users");
            }
            catch (Exception ex)
            {
                return StatusCode(500);

            }

        }

    }
}
