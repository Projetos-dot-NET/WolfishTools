using LLama;
using LLama.Common;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;

// --- 1. CONEXÃO MCP (Seu servidor Wolfish.ServerMcp) ---
var transport = new StdioClientTransport(new StdioClientTransportOptions {
    Command = "dotnet",
    Arguments = ["run", "--project", "../Wolfish.ServerMcp/Wolfish.ServerMcp.csproj", "--quiet"]
});
await using var mcpClient = await McpClientFactory.CreateAsync(transport);
var mcpTools = await mcpClient.ListToolsAsync();

// --- 2. CARREGAR MODELO GGUF ---
string modelPath = @"/home/renatolobojr/Downloads/qwen2.5-1.5b-instruct-q8_0.gguf"; // Altere para o seu arquivo
var parameters = new ModelParams(modelPath) {
   ContextSize = 4096,
    // -1 tenta carregar todas as camadas na GPU automaticamente
    GpuLayerCount = -1
};
using var weights = LLamaWeights.LoadFromFile(parameters);
using var context = weights.CreateContext(parameters);
var executor = new InteractiveExecutor(context);

// --- 3. DEFINIR O SYSTEM PROMPT COM AS FERRAMENTAS ---
// O LLamaSharp puro não tem "AsChatTool", então passamos as ferramentas no Prompt
string ferramentasTexto = string.Join("\n", mcpTools.Select(t => $"- {t.Name}: {t.Description}"));

var chatHistory = new ChatHistory();
chatHistory.AddMessage(AuthorRole.System, $"Você é um assistente que pode usar ferramentas. Ferramentas disponíveis:\n{ferramentasTexto}");

// --- 4. LOOP DO AGENTE ---
var session = new ChatSession(executor, chatHistory);
Console.WriteLine("Agente GGUF Online! Digite seu comando:");

while (true)
{
    Console.Write("\nUsuário: ");
    var input = Console.ReadLine();
    if (string.IsNullOrEmpty(input)) break;

    // O modelo processa o texto
    await foreach (var text in session.ChatAsync(new ChatHistory.Message(AuthorRole.User, input)))
    {
        Console.Write(text);
        
        // Lógica de "Tool Calling" Manual:
        // Modelos GGUF menores precisam de prompts claros como: "CALL: tool_name(args)"
        // Para uma PoC, você verifica se o 'text' contém uma intenção de chamada.
    }
}
