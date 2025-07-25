namespace KafkaTester.Services
{
    public interface IKafkaService
    {
        Task<string> ProduceMessage(string message);
        Task<string> ConsumeMessage();
    }
}
