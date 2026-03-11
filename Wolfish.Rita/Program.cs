using Azure.AI.OpenAI;
using Azure.Identity;
//using Microsoft.Extensions.AI;
//using Microsoft.Extensions.AI.Mcp; // Este depende do pacote acima
using ModelContextProtocol;
using ModelContextProtocol.Client;


// Create an IChatClient using Azure OpenAI.
IChatClient client =
    new ChatClientBuilder(
        new AzureOpenAIClient(new Uri("<your-azure-openai-endpoint>"),
        new DefaultAzureCredential())
        .GetChatClient("gpt-4o").AsIChatClient())
    .UseFunctionInvocation()
    .Build();

// Create the MCP client
// IMPORTANTE: Use a Factory que vem do pacote .Client
using IMcpClient mcpClient = await McpClientFactory.CreateAsync(
    new StdioClientTransport(new StdioClientTransportOptions
    {
        Command = "dotnet",
        Arguments = ["run", "--project", "/home/renatolobojr/repos-dot-net/WolfishTools/Wolfish.ServerMcp/Wolfish.ServerMcp.csproj"],
        Name = "Minimal MCP Server",
    }));

// List e conversão correta
Console.WriteLine("Available tools:");
var mcpTools = await mcpClient.ListToolsAsync();
// .AsChatTool() agora deve funcionar com o using Microsoft.Extensions.AI.Mcp
var chatTools = mcpTools.Select(t => t.AsChatTool()).ToList();

foreach (var tool in chatTools)
{
    Console.WriteLine($"- {tool.Name}");
}
Console.WriteLine();

List<ChatMessage> messages = [];
while (true)
{
    Console.Write("Prompt: ");
    string input = Console.ReadLine();
    if (string.IsNullOrEmpty(input)) break;
    
    messages.Add(new(ChatRole.User, input));

    List<ChatResponseUpdate> updates = [];
    // AQUI: use a variável 'chatTools' que você criou lá em cima
    await foreach (ChatResponseUpdate update in client
        .GetStreamingResponseAsync(messages, new() { Tools = chatTools }))
    {
        Console.Write(update);
        updates.Add(update);
    }
    Console.WriteLine();

    messages.AddMessages(updates);
}
