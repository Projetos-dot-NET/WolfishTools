using System.Reflection;
using System.Text;
using Wolfish.Commands;

namespace Wolfish.Maia
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            //string[] args = ["merge","developer","master"];
            //string[] args = ["download","chrome"];

            //string[] args = ["install", "sdk8"];

            //string[] args = ["apt", "search", "octopi"];
            //string[] args = ["uninstall", "dotnet8"];
            //string[] args = ["ask", "qqumn", "para", "me", "dar", "dicas", "de", "comandos", "shell", "windows", "e", "linux", "mais", "utilizados", "em", "desenvolvimento", "de", "software", "em", "no", "máximo", "200", "palavras", "e", "em", "portugues"];

            var semver = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            if (args.Length == 0) args = ["welcome", "version"];

            if (args[0] == "welcome")
            {                
                Console.WriteLine($"\nTank you! I'm happy to be here! \nAnd I'm now runnig on {args[1]} {semver}");
            }

            var terminalCommand = new WolfishCommand("./Lists/TerminalCommands.json");
            var found = await terminalCommand.SeekAndExecute(args[0], args[1]);

            var allArguments = new StringBuilder();

            if (!found)
            {
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
