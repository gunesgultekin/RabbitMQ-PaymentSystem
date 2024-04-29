using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace APIGateway
{
    [ApiController]
    [Route("/api/[controller]")]
    public class APIGatewayController : ControllerBase
    {
        private ConnectionFactory _connectionFactory;
        
        public APIGatewayController() 
        { 
            _connectionFactory = new ConnectionFactory { HostName = "localhost" } ;
        }

        // Add payment request to the "payments" queue 
        // PARAMETERS: paymentId, deviceId

        [HttpPost("makePayment")]
        public async Task<ActionResult> makePayment([FromBody] paymentRequestModel parameters)
        {
            try
            {
                // Connect to local RabbitMQ server
                using var connection = _connectionFactory.CreateConnection();
                using var channel = connection.CreateModel();

                // Prepare message content
                var body = Encoding.UTF8.GetBytes(parameters.paymentId + " " + parameters.deviceId);

                // Send message (parameters) to the "Payments" queue
                // paymentId, deviceId
                channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: "Payments",
                    basicProperties: null,
                    body: body
                    );
                
                return Ok($"Payment Sent to Queue | PaymentID:{parameters.paymentId} DeviceID:{parameters.deviceId}");

            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
    }
}
