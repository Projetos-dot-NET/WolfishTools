using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;
using Wolfish.CloudAgents.Config;

namespace Wolfish.CloudAgents.Chat
{

    /// <summary>
    /// Agente de chat que mantém histórico de conversa e gerencia tool calling automático.
    /// </summary>
    public class ChatAgent
    {
        private IChatClient _chatClient;
        private readonly List<ChatMessage> _history = [];
        public AgentSettings _agentSettings { get; private set; }

        public ChatAgent(string name)
        {
            _agentSettings = new AgentSettings();
            var cloudAgent = _agentSettings.GetCloudAgent(name);
            var systemMessage = cloudAgent.SystemMessage;
            var endpoint = cloudAgent.Provider?.Endpoint ?? throw new Exception($"Endpoint não configurado para {name}");
            var apiKey = cloudAgent.Provider?.ApiKey ?? throw new Exception($"ApiKey não configurada para {name}");
            var model = cloudAgent.Provider?.Model ?? throw new Exception($"Model não configurado para {name}");

            if (!string.IsNullOrWhiteSpace(systemMessage)) _history.Add(new ChatMessage(ChatRole.System, systemMessage));

            var options = new OpenAIClientOptions { Endpoint = new Uri(endpoint) };
            _chatClient = new OpenAI.Chat.ChatClient(model, new ApiKeyCredential(apiKey), options).AsIChatClient();
        }

        /// <summary>
        /// Envia a mensagem do usuário ao agente e recebe a resposta em streaming.
        /// </summary>
        public async IAsyncEnumerable<string> SendMessageStreamingAsync(string userMessage)
        {
            _history.Add(new ChatMessage(ChatRole.User, userMessage));

            var fullResponse = new System.Text.StringBuilder();

            await foreach (var chunk in _chatClient.GetStreamingResponseAsync(_history))
            {
                var text = chunk.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    fullResponse.Append(text);
                    yield return text;
                }
            }

            // Adiciona resposta completa do assistente ao histórico
            _history.Add(new ChatMessage(ChatRole.Assistant, fullResponse.ToString()));
        }

        /// <summary>
        /// Retorna o histórico atual (sem a system message).
        /// </summary>
        public IEnumerable<ChatMessage> GetHistory() => _history.Where(m => m.Role != ChatRole.System);

        /// <summary>
        /// Troca de agente preservando todo o histórico da conversa.
        /// </summary>
        public void ChangeCloudAgent(string name)
        {
            var cloudAgent = _agentSettings.GetCloudAgent(name);
            var systemMessage = cloudAgent.SystemMessage;
            var endpoint = cloudAgent.Provider?.Endpoint ?? throw new Exception($"Endpoint não configurado para {name}");
            var apiKey = cloudAgent.Provider?.ApiKey ?? throw new Exception($"ApiKey não configurada para {name}");
            var model = cloudAgent.Provider?.Model ?? throw new Exception($"Model não configurado para {name}");

            var options = new OpenAIClientOptions { Endpoint = new Uri(endpoint) };
            _chatClient = new OpenAI.Chat.ChatClient(model, new ApiKeyCredential(apiKey), options).AsIChatClient();

            if (!string.IsNullOrWhiteSpace(systemMessage))
                _history.Add(new ChatMessage(ChatRole.System, systemMessage));
        }

        /// <summary>
        /// Limpa o histórico de conversa (mantém a system message).
        /// </summary>
        public void ClearHistory()
        {
            var systemMsg = _history.FirstOrDefault(m => m.Role == ChatRole.System);
            _history.Clear();
            if (systemMsg is not null)
                _history.Add(systemMsg);
        }


    }

}