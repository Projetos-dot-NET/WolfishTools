using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Text;
using Wolfish.Commands;
using Wolfish.Llama;

namespace Wolfish.Maia
{
    public class Program
    {        
        private static async Task Main(string[] args)
        {
            var found = false;
            var baseDirectory = AppContext.BaseDirectory;
            var terminalCommand = new WolfishCommand($"{baseDirectory}/Lists/TerminalCommands.json");

            //string[] args = ["welcome"];
            //string[] args = ["list"];

            //string[] args = ["install","github"];
            //string[] args = ["merge","developer","master"];
            //string[] args = ["download","chrome"];

            //string[] args = ["install", "sdk8"];

            //string[] args = ["apt", "search", "octopi"];
            //string[] args = ["uninstall", "dotnet8"];
            //string[] args = ["ask", "qwen", "para", "me", "dar", "dicas", "de", "comandos", "shell", "windows", "e", "linux", "mais", "utilizados", "em", "desenvolvimento", "de", "software", "em", "no", "máximo", "200", "palavras", "e", "em", "portugues"];

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

            }

            if (!found && args.Length == 2)//clean shots tiro certeiro
            {
                found = await terminalCommand.SeekAndExecute(args[0], args[1]);
            }

            

            if (!found && args.Length > 2) //burst rajada
            {
                var allArguments = new StringBuilder();
                var modelName = args[1];
                var settings = Config(modelName);
                var agent = new LlamaService(settings);

                if (args[0] == "ask")
                {
                    for (var i = 2; i < args.Length; i++) allArguments.Append(" " + args[i]);
                    
                    agent.ChatWithAgent(allArguments.ToString()).Wait();
                }
                else
                {
                    foreach (var arg in args) allArguments.Append(" " + arg);
                    var promptDefault = $"Me dê uma lista de comandos via terminal utilizados no windows " +
                                        $"e o linux que se pareça com esses e me oriente como utiliza-los " +
                                        $"em no máximo 256 caracteres e em portugues:{allArguments.ToString()}";

                    agent.ChatWithAgent(promptDefault).Wait();
                }
            }
            //end if
        }
        //end main

        private static void ShowHelp()
        {
            Console.WriteLine("Wolfish.Maia - Assistente de linha de comando impulsionado por IA");
            Console.WriteLine("Uso:");
            Console.WriteLine("  Wolfish.Maia welcome                     Exibe uma mensagem de boas-vindas.");
            Console.WriteLine("  Wolfish.Maia list                        Lista todos os comandos disponíveis.");
            Console.WriteLine("  Wolfish.Maia install <nome_do_pacote>    Instala o pacote especificado.");
            Console.WriteLine("  Wolfish.Maia uninstall <nome_do_pacote>  Desinstala o pacote especificado.");
            Console.WriteLine("  Wolfish.Maia ask <pergunta>              Faz uma pergunta ao assistente de IA.");
            Console.WriteLine();
        }

        private static LlamaSettings? Config(string modelName) 
        {

            var baseDirectory = AppContext.BaseDirectory;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{baseDirectory}/llamasettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            var settings = new LlamaSettings();
            config.GetSection("LanguageModels").Bind(settings);

            var allModels = config.GetSection("LanguageModels").Get<List<LlamaSettings>>();
            var selectedConfig = allModels?.FirstOrDefault(c => c.Name.Equals(modelName, StringComparison.OrdinalIgnoreCase));

            if (selectedConfig == null) return null;
            if (!File.Exists(selectedConfig.ModelPath))
            {
                Console.WriteLine($"[ERRO] Modelo não encontrado no caminho: {settings.ModelPath}");
                Console.WriteLine("Verifique seu appsettings.json");
                return null;
            }
            return selectedConfig;
        }
        
    }
    //end class
}
