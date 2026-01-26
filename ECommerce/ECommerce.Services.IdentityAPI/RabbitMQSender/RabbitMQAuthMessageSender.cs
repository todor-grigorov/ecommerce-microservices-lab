using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ECommerce.Services.IdentityAPI.RabbitMQSender
{
    public class RabbitMQAuthMessageSender : IRabbitMQAuthMessageSender
    {
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private IConnection? _connection;

        public RabbitMQAuthMessageSender()
        {
            _hostName = "localhost";
            _userName = "guest";
            _password = "guest";
        }

        public async Task SendMessageAsync<T>(T message, string queueName)
        {
            if (ConnectionExists())
            {

                var channel = await _connection!.CreateChannelAsync();

                await channel.QueueDeclareAsync(queueName, false, false, false, null);
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);
                //var body = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(message);
                await channel.BasicPublishAsync(exchange: "",
                                     routingKey: queueName,
                                     body: body);
            }

        }

        private async void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    UserName = _userName,
                    Password = _password,
                    Port = 5672,
                    VirtualHost = "/",
                };

                _connection = await factory.CreateConnectionAsync();
            }
            catch (Exception ex)
            {

            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }
            CreateConnection();
            return true;
        }

    }
}
