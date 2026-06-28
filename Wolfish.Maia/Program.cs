using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Wolfish.ChatAgent;
using Wolfish.Commands;
using Wolfish.Shared;

namespace Wolfish.Maia
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var found = false;
            var baseDirectory = AppContext.BaseDirectory;
            var terminalCommand = new WolfishCommand($"{baseDirectory}Lists/TerminalCommands.json");

            //args = ["welcome"];
            //args = ["list"];

            //args = ["install","github"];
            //args = ["merge","developer","master"];
            //args = ["download","chrome"];
            //args = ["update","system"];

            //args = ["install", "sdk10"];

            //args = ["apt", "search", "octopi"];
            //args = ["uninstall", "dotnet8"];
            //args = ["ask", "especial", "para", "me", "dar", "dicas", "de", "comandos", "shell", "windows", "e", "linux", "mais", "utilizados", "em", "desenvolvimento", "de", "software", "em", "no", "máximo", "200", "palavras", "e", "em", "portugues"];

            if (args.Length == 0)
            {
                ShowHelp();
            }

            if (args.Length == 1) //quick shots tiro rapido
            {
                if (args[0] == "welcome")
                {
                    found = true;
                    var semver = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
                    Console.WriteLine($"\nTank you! I'm happy to be here! \nAnd I'm now runnig on version {semver}");
                }

                if (args[0] == "list")
                {
                    found = true;
                    var commandList = terminalCommand.LoadFromJson();
                    var commandtable = terminalCommand.BuildLimidetTable(commandList);
                    Console.WriteLine(commandtable);
                }

                if (!found && args[0] == "platform")
                {
                    found = true;
                    string infoSO = RuntimeInformation.OSDescription;
                    string arch = RuntimeInformation.OSArchitecture.ToString();
                    string runtime = RuntimeInformation.RuntimeIdentifier;
                    string platform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" :
                        RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux" :
                        RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "macOS" : "Unknown OS";

                    Console.WriteLine($"OS: {platform} {arch} ({infoSO} based in {runtime}) ");
                }

                if (!found && args[0] == "directory")
                {
                    found = true;
                    var basedir = AppContext.BaseDirectory;
                    Console.WriteLine(basedir);
                }

                if (!found && args[0] == "help")
                {
                    found = true;
                    ShowHelp();
                }

            }

            if (!found && args.Length == 2)//clean shots tiro certeiro
            {
                found = await terminalCommand.SeekAndExecute(args[0], args[1]);
            }

            if (!found && args.Length > 2) //burst rajada
            {
                var allArguments = new StringBuilder();
                
                // expunge apagar totalmente
                //if (args[0] == "expunge")

                // efface apagar aos poucos
                //if (args[0] == "efface")

                if (args[0] == "ask")
                {
                    var agentName = args[1];
                    var allAgents = new List<CloudAgent>();

                    if (string.IsNullOrWhiteSpace(agentName))
                    {
                        Console.WriteLine("Please provide a valid agent name.");
                        return;
                    }

                    if (string.Equals(agentName, "all", StringComparison.OrdinalIgnoreCase))
                    {
                        allAgents = GetAllAgents();
                    }
                    else
                    {
                        var busca = GetAllAgents() ?? new List<CloudAgent>();
                        var agentFound = SearchAgentByName(agentName, busca);
                        if (agentFound != null)
                        {
                            allAgents.Add(agentFound);
                        }
                        else
                        {
                            Console.WriteLine($"Agent '{agentName}' not found.");
                            return;
                        }
                    }

                    if (allAgents == null || allAgents.Count == 0)
                    {
                        Console.WriteLine("No agents available.");
                        return;
                    }

                    foreach (var agent in allAgents)
                    {
                        Console.WriteLine($"Asking {agent.Name}... ");
                        var provider = ConfigProvider(agent!.ProviderName);
                        if (provider == null)
                        {
                            Console.WriteLine($"Provider '{agent.ProviderName}' não encontrado em appsettings.json.");
                            return;
                        }
                        

                        for (var i = 2; i < args.Length; i++) allArguments.Append(" " + args[i]);
                        
                        var agentHistory = new AgentHistory($"history-{agent.Name}.json");
                        
                        if (agent.History == "self")
                        {
                            agentHistory.Load();
                        }
                        else if (agent.History == "global")
                        {
                            AgentHistory.LoadGlobalHistories(".", "history-*.json");
                        }
                        
                        agentHistory.AddSystem(agent.SystemMessage!);
                        agentHistory.AddUser(allArguments.ToString());
                        
                        var cloudAgent = new OpenAiAgent(agent.Model, provider?.Endpoint!, provider?.ApiKey!, agentHistory);
                        var outputFile = Path.Combine(Directory.GetCurrentDirectory(), $"ask-{agent.Name}-{DateTime.Now:yyyyMMdd-HHmmss}.md");
                        var responseBuilder = new StringBuilder();

                        if (agent.History == "self" || agent.History == "none")
                        {
                            agentHistory.Save();
                        }

                        try
                        {
                            //await using var writer = new StreamWriter(outputFile, false, Encoding.UTF8);
                            IAsyncEnumerable<string> teste = cloudAgent.SendMessageStreamingAsync();

                            await foreach (var message in teste)
                            {
                                responseBuilder.Append(message);
                            }

                            await File.WriteAllTextAsync(outputFile, responseBuilder.ToString(), Encoding.UTF8);
                            Console.WriteLine($"Answer written to {outputFile}\n");                            
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }                      
                    }
                }
                else
                {
                    ShowHelp();

                    // foreach (var arg in args) allArguments.Append(" " + arg);
                    // var promptDefault = $"Me dê uma lista de comandos via terminal utilizados no windows " +
                    //                     $"e o linux que se pareça com esses e me oriente como utiliza-los " +
                    //                     $"em no máximo 256 caracteres e em portugues:{allArguments.ToString()}";

                    // IAsyncEnumerable<string> teste = cloudAgent.SendMessageStreamingAsync(promptDefault.ToString());

                    // await foreach (var message in teste)
                    // {
                    //     Console.Write(message);
                    // }
                }
            }

        }

        private static void ShowHelp()
        {
            Console.WriteLine("Wolfish.Maia - Assistente de linha de comando impulsionado por IA");
            Console.WriteLine("Uso:");
            Console.WriteLine("  maia welcome                     Exibe uma mensagem de boas-vindas.");
            Console.WriteLine("  maia list                        Lista todos os comandos disponíveis.");
            Console.WriteLine("  maia platform                    Exibe info do sistema operacional.");
            Console.WriteLine("  maia directory                   Exibe o diretório base do aplicativo.");
            Console.WriteLine("  maia help                        Exibe esta mensagem de ajuda.");
            Console.WriteLine("  maia install <nome_do_pacote>    Instala o pacote especificado.");
            Console.WriteLine("  maia uninstall <nome_do_pacote>  Desinstala o pacote especificado.");
            Console.WriteLine("  maia ask <pergunta>              Faz uma pergunta ao assistente de IA.");
            Console.WriteLine();
        }

        private static CloudAgent? SearchAgentByName(string agentName, List<CloudAgent>? allAgents)
        {
            var selectedAgent = allAgents?.FirstOrDefault(c => c.Name.Equals(agentName, StringComparison.OrdinalIgnoreCase));
            if (selectedAgent is null) return null;
            return selectedAgent;
        }

        private static List<CloudAgent>? GetAllAgents()
        {
            var baseDirectory = AppContext.BaseDirectory;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{baseDirectory}cloudagents.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();
            var cloudAgent = new CloudAgent();
            config.GetSection("CloudAgents").Bind(cloudAgent);
            var allAgents = config.GetSection("CloudAgents").Get<List<CloudAgent>>();

            return allAgents;
        }

        private static LlmProvider? ConfigProvider(string providerName)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{baseDirectory}appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();
            var providers = new LlmProvider();
            config.GetSection("LLMProviders").Bind(providers);
            var allProviders = config.GetSection("LLMProviders").Get<List<LlmProvider>>();

            var selectedProvider = allProviders?.FirstOrDefault(c => c.Name.Equals(providerName, StringComparison.OrdinalIgnoreCase));
            if (selectedProvider is null) return null;
            return selectedProvider;
        }

    }

}
