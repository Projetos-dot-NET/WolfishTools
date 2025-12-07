using System.Reflection;
using System.Text;
using Wolfish.Commands;

namespace Wolfish.Maia
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var found = false;
            
            //string[] args = ["welcome"];
            //string[] args = ["install","github"];
            //string[] args = ["merge","developer","master"];
            //string[] args = ["download","chrome"];

            //string[] args = ["install", "sdk8"];

            //string[] args = ["apt", "search", "octopi"];
            //string[] args = ["uninstall", "dotnet8"];
            //string[] args = ["ask", "qqumn", "para", "me", "dar", "dicas", "de", "comandos", "shell", "windows", "e", "linux", "mais", "utilizados", "em", "desenvolvimento", "de", "software", "em", "no", "máximo", "200", "palavras", "e", "em", "portugues"];

            if (args.Length == 0) args = ["welcome", "version"];

            if (args[0] == "welcome")
            {
                args = ["welcome", "version"];
                found = true;
                var semver = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
                Console.WriteLine($"\nTank you! I'm happy to be here! \nAnd I'm now runnig on {args[1]} {semver}");
            }

            if (!found)
            {
                var baseDirectory = AppContext.BaseDirectory;
                var terminalCommand = new WolfishCommand($"{baseDirectory}/Lists/TerminalCommands.json");
                found = await terminalCommand.SeekAndExecute(args[0], args[1]);
            }                     

            if (!found)
            {
                var allArguments = new StringBuilder();

                if (args[0] == "ask" && args[1] == "gemini")
                {
                    for (var i = 2; i < args.Length; i++) allArguments.Append(" " + args[i]);
                    AgentCommand.AskGeminiPro(allArguments.ToString());
                }
                else
                {
                    foreach (var arg in args) allArguments.Append(" " + arg);
                    AgentCommand.AskGeminiFlash($"Pesquise por comandos via terminal utilizados no windows " +
                                                $"e o linux que se pareça com esses e me oriente como utiliza-los " +
                                                $"em no máximo 256 caracteres e em portugues:{allArguments.ToString()}");
                }
            }
            //end if
        }
        //end main
    }
    //end class
}
