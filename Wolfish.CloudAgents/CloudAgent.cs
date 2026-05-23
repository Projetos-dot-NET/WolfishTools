namespace Wolfish.CloudAgents.ValueObjects
{
    public class CloudAgent
    {
        public CloudAgent(string? name, string? systemMessage, string? providerName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(Name));
            SystemMessage = systemMessage ?? throw new ArgumentNullException(nameof(SystemMessage));
            ProviderName = providerName ?? throw new ArgumentNullException(nameof(ProviderName));
        }

        public string Name { get; init; }
        public string SystemMessage { get; init; }
        public string ProviderName { get; init; }

        public LlmProvider? Provider { get; set; }
    }



}