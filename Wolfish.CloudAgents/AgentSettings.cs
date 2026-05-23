using Microsoft.Extensions.Configuration;
using Wolfish.CloudAgents.ValueObjects;

namespace Wolfish.CloudAgents.Config
{
    public class AgentSettings
    {
        public List<CloudAgent> Agents { get; set; } = [];
        public List<LlmProvider> Providers { get; set; } = [];


        public AgentSettings()
        {
            var configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
                                                          .AddJsonFile("agentsettings.json", optional: false, reloadOnChange: false)
                                                          .Build();

            Agents.AddRange(configuration.GetSection("CloudAgents").GetChildren().Select(s => new CloudAgent(s["Name"], s["SystemMessage"], s["ProviderName"])!));
            Providers.AddRange(configuration.GetSection("LLMProviders").GetChildren().Select(s => new LlmProvider() { Name = s["Name"], Endpoint = s["Endpoint"], ApiKey = s["ApiKey"], Model = s["Model"] }));
        }

        public CloudAgent GetCloudAgent(string name)
        {
            var agent = Agents.Find(a => a.Name == name) ?? throw new NullReferenceException("Agente não encontrado");
            agent.Provider = Providers.Find(p => p.Name == agent.ProviderName) ?? throw new NullReferenceException("Provedor não encontrado");

            return agent;
        }

    }



}