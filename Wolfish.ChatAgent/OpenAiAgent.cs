using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace Wolfish.ChatAgent
{
    public class OpenAiAgent
    {
        public ChatClient _openAiClient;
        private IChatClient _chatClient;
        private readonly List<ChatMessage> _messages = [];

        public OpenAiAgent(string model, string endpoint, string apiKey, AgentHistory history)
        {
            _messages.AddRange(history.GetMessages());

            var options = new OpenAIClientOptions { Endpoint = new Uri(endpoint) };            
            if (string.IsNullOrEmpty(apiKey))
            {
                _openAiClient = new ChatClient(model, new ApiKeyCredential("*"), options);
            }
            else
            {
                _openAiClient = new ChatClient(model, new ApiKeyCredential(apiKey), options);
            }

            _chatClient = _openAiClient.AsIChatClient();
        }

        /// <summary>
        /// Envia a mensagem do usuário ao agente e recebe a resposta em string.
        /// </summary>
        public async Task<string> SendMessageAsync(string userMessage)
        {            
            if (!string.IsNullOrWhiteSpace(userMessage)) _messages.Add(new ChatMessage(ChatRole.User, userMessage));

            var resposta = await _chatClient.GetResponseAsync(_messages);

            var fullResponse = new System.Text.StringBuilder();
            foreach (var message in resposta.Messages)
            {
                fullResponse.Append(message.Text);
            }

            _messages.Add(new ChatMessage(ChatRole.Assistant, fullResponse.ToString()));

            return fullResponse.ToString();
        }

        /// <summary>
        /// Envia a mensagem do usuário ao agente e recebe a resposta em streaming.
        /// </summary>
        public async IAsyncEnumerable<string> SendMessageStreamingAsync(string userMessage)
        {
            if (!string.IsNullOrWhiteSpace(userMessage)) _messages.Add(new ChatMessage(ChatRole.User, userMessage));

            await foreach (var chunk in _chatClient.GetStreamingResponseAsync(_messages))
            {
                var text = chunk.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    yield return text;
                }
            }
        }

        /// <summary>
        /// Envia o historico de mensagens do usuário ao agente e recebe a resposta em streaming.
        /// </summary>
        public async IAsyncEnumerable<string> SendMessageStreamingAsync()
        {
            await foreach (var chunk in _chatClient.GetStreamingResponseAsync(_messages))
            {
                var text = chunk.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    yield return text;
                }
            }
        }
    }
}
