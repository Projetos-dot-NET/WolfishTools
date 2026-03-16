using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;

var transportOptions = new StdioClientTransportOptions
{
    Command = "dotnet",
    // Use o caminho relativo ou absoluto para o projeto do servidor
    Arguments = ["run", "--project", "../Wolfish.ServerMcp/Wolfish.ServerMcp.csproj", "--quiet"]
};



var transport = new StdioClientTransport(transportOptions);

// 2. Criar o cliente (Usando o método estático que é o padrão atual)
await using var client = await McpClientFactory.CreateAsync(transport);

// 2. O método foi renomeado para ConnectAsync ou InitializeAsync (tente ConnectAsync se falhar)
// Em algumas versões de preview do .NET 10, o Initialize é implícito no Create ou chama-se:
//await client.ConnectAsync(); 

// 3. A resposta do ListToolsAsync agora retorna diretamente uma LISTA, não um objeto com .Tools
var tools = await client.ListToolsAsync();

// 4. Como 'tools' já é a lista (IList<McpClientTool>), você acessa direto:
Console.WriteLine($"Conectado! Encontradas {tools.Count} ferramentas.");

foreach (var tool in tools)
{
    Console.WriteLine($"- Ferramenta: {tool.Name}");
}

Console.WriteLine("\nTentando gerar um número aleatório via MCP...");
// Use um Dictionary<string, object?> para passar os argumentos
var argumentos = new Dictionary<string, object?> 
{ 
    { "min", 0 }, 
    { "max", 100 } 
};

var result = await client.CallToolAsync("get_random_number", argumentos);

// No SDK 1.0, o resultado é uma lista de conteúdos. Pegamos o texto do primeiro:
var texto = result.Content.FirstOrDefault()?.Text;

Console.WriteLine($"Número gerado pelo servidor: {texto}");