namespace ECommerce.Services.ShoppingCartAPI.RabbitMQSender
{
    public sealed class RabbitMqOptions
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string VirtualHost { get; set; } = "/";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";

        public bool DurableQueues { get; set; } = true; // important to standardize
        public bool UsePublisherConfirms { get; set; } = false;
    }
}
