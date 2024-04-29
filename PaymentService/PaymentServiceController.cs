using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PaymentService
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PaymentServiceController : ControllerBase
    {
        private ConnectionFactory _connectionFactory;

        public PaymentServiceController()
        {
            _connectionFactory = new ConnectionFactory { HostName = "localhost" };
        }

        // Process payment according to the parameter information coming from the "Payments" queue.
        [HttpGet("processPayment")]
        public async Task<ActionResult> processPayment()
        {
            try
            {
                // Connect to local RabbitMQ server
                using var connection = _connectionFactory.CreateConnection();
                using var channel = connection.CreateModel();

                var consumer = new EventingBasicConsumer(channel);

                string deviceId = " ";

                // Set what to do when messages are obtained from "Payments Queue"
                consumer.Received += async (model, ea) =>
                {
                    // Get message
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    // Split message content according to space character. (expected message format : paymentId (space) deviceId)
                    string[] splittedParams = message.Split(" ");
                    // [0] => paymentId
                    // [1] => deviceId
                    deviceId = splittedParams[1]; // Current Device ID

                    // Set message content to send "Notifications Queue"
                    // message format {deviceId (space) status}
                    // Status will be set successful for testing
                    var notificationQueueMessage = Encoding.UTF8.GetBytes(deviceId + " " + "succesful");

                    // Create other channel for "Notifications" Queue
                    using var notificationChannelConnection = _connectionFactory.CreateConnection();
                    using var notificationChannel = notificationChannelConnection.CreateModel();

                    // Send message to "Notifications" queue
                    notificationChannel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: "Notifications",
                    basicProperties: null,
                    body: notificationQueueMessage
                    );
                    // Imitate async operation
                    await Task.Delay(2000);

                    Console.WriteLine($"* Payment with ID {splittedParams[0]} succesful");
                };
                // Consume messages came from "Payments Queue"
                channel.BasicConsume(
                    queue: "Payments",
                    autoAck: true,
                    consumer: consumer
                    );
                
                return Ok($"Payment Succesfull. Notification Requests send to Notifications Queue.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

    }
}
