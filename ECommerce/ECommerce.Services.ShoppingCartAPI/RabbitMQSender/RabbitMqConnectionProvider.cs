using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ECommerce.Services.ShoppingCartAPI.RabbitMQSender
{
    public sealed class RabbitMqConnectionProvider : IRabbitMqConnectionProvider, IAsyncDisposable
    {
        private readonly RabbitMqOptions _options;
        private readonly SemaphoreSlim _sync = new(1, 1);
        private IConnection? _connection;

        public RabbitMqConnectionProvider(IOptions<RabbitMqOptions> options)
        {
            _options = options.Value;
        }

        public async Task<IConnection> GetConnectionAsync(CancellationToken ct = default)
        {
            // Fast path
            if (_connection is { IsOpen: true })
                return _connection;

            await _sync.WaitAsync(ct);
            try
            {
                if (_connection is { IsOpen: true })
                    return _connection;

                // Dispose old connection if present
                if (_connection is not null)
                {
                    try { await _connection.CloseAsync(ct); } catch { /* ignore */ }
                    await _connection.DisposeAsync();
                    _connection = null;
                }

                var factory = new ConnectionFactory
                {
                    HostName = _options.HostName,
                    Port = _options.Port,
                    VirtualHost = _options.VirtualHost,
                    UserName = _options.UserName,
                    Password = _options.Password,
                    AutomaticRecoveryEnabled = false, // manual reconnect
                };

                _connection = await factory.CreateConnectionAsync(ct);
                return _connection;
            }
            finally
            {
                _sync.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _sync.WaitAsync();
            try
            {
                if (_connection is not null)
                {
                    try { await _connection.CloseAsync(); } catch { /* ignore */ }
                    await _connection.DisposeAsync();
                    _connection = null;
                }
            }
            finally
            {
                _sync.Release();
                _sync.Dispose();
            }
        }
    }
}
