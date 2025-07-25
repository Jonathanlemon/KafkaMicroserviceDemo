using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaTester.Services;



public class OnboardingService : IKafkaService, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly IConsumer<Null, string> _consumer;
    private readonly string _serviceTopic;

    public OnboardingService()
    {
        _serviceTopic = "ReadyForOnboarding";

        // Hardcoded producer config
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "172.19.87.201:9092",
            Acks = Acks.All,
            Debug = "all"
            // Add other configs as needed
        };

        // Hardcoded consumer config
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "172.19.87.201:9092",
            GroupId = "Jonathan-Service-ID", // Unique group per topic
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
        };

        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        _consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();

        _consumer.Subscribe(_serviceTopic);
    }

    public async Task<string> ProduceMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return "No message to produce.";

        try
        {
            var result = await _producer.ProduceAsync(_serviceTopic, new Message<Null, string> { Value = message });
            return $"Produced message to: {result.TopicPartitionOffset}";
        }
        catch (ProduceException<Null, string> ex)
        {
            return $"Produce error: {ex.Error.Reason}";
        }
    }

    public async Task<string> ConsumeMessage()
    {
        try
        {
            // Consume with timeout (or cancellation token)
            var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(5));
            if (consumeResult == null)
                return "No message consumed.";

            return $"Consumed message: {consumeResult.Message.Value} at {consumeResult.TopicPartitionOffset}";
        }
        catch (ConsumeException ex)
        {
            return $"Consume error: {ex.Error.Reason}";
        }
        catch (OperationCanceledException)
        {
            return "Consume operation canceled.";
        }
    }

    public void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
        _producer?.Dispose();
    }
}
