using RabbitMQ.Client;
using Microsoft.EntityFrameworkCore;
using SuperComData.Context;
using System.Text;

namespace SuperComWorker
{
    public class Worker : BackgroundService, IDisposable
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;
        private readonly string _queueName;
        private readonly int _pollingIntervalSeconds;
        private readonly int _retryDelaySeconds;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _queueName = _configuration.GetValue<string>("RabbitMQ:QueueName") ?? "task_reminders";
            _pollingIntervalSeconds = _configuration.GetValue<int?>("Worker:PollingIntervalSeconds") ?? 30;
            _retryDelaySeconds = _configuration.GetValue<int?>("Worker:RetryDelaySeconds") ?? 5;
        }

        private void InitRabbitMQ()
        {
            var hostName = _configuration.GetValue<string>("RabbitMQ:HostName") ?? "localhost";
            var factory = new ConnectionFactory() { HostName = hostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new RabbitMQ.Client.Events.EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("CONSUMER RECEIVED: {Message}", message);
            };
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_connection == null)
                    {
                        InitRabbitMQ();
                    }

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var overdueTasks = await db.Tasks
                            .Where(t => t.DueDate < DateTime.UtcNow && !t.IsOverdueProcessed)
                            .ToListAsync();

                        foreach (var task in overdueTasks)
                        {
                            var message = $"Hi your Task is due {{Task {task.Title}}}";
                            var body = Encoding.UTF8.GetBytes(message);

                            _channel.BasicPublish(exchange: "", routingKey: _queueName, body: body);

                            //mark as processed so we don't send it again
                            task.IsOverdueProcessed = true;
                            _logger.LogInformation("PRODUCER SENT: {TaskTitle}", task.Title);
                        }

                        await db.SaveChangesAsync();
                    }
                }

                catch (Exception ex)
                {
                    //if api is still creating the db, we catch it here
                    _logger.LogWarning("Database not ready yet. Worker is waiting... Error: {Message}", ex.Message);

                    await Task.Delay(TimeSpan.FromSeconds(_retryDelaySeconds), stoppingToken);
                    continue;
                }

                await Task.Delay(TimeSpan.FromSeconds(_pollingIntervalSeconds), stoppingToken);
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
