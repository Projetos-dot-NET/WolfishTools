using System.Reflection;
using Wolfish.Commands;

namespace Wolfish.Maia
{
    public class Program
    {
        static void Main(string[] args)
        {
            //string[] args = ["merge","developer", "master"];
            //string[] args = ["download","chrome"];
            //string[] args = ["ask", "me dê dicas de comandos shell mais utilizados em desenvolvimento de software"];


            //Console.Write("Hello, World!");
            //foreach (var arg in args)
            //{
            //    Console.Write($" {arg}");
            //}

            NoArgsValidation(args);

            var wolfishCommand = args[0].ToLower();

            // status config help list move deploy pipieline log start stop run 

            switch (wolfishCommand)
            {
                case "welcome":
                    WelcomeCommand(args);
                    break;
                case "create":
                    CreateCommand(args);
                    break;
                case "init":
                    InitCommand(args);
                    break;
                case "finish":
                    FinishCommand(args);
                    break;
                case "merge":
                    MergeCommand(args);
                    break;
                case "download":
                    DownloadCommand(args);
                    break;
                case "ask":
                    AskCommand(args);
                    break;
                default:
                    CommandValitadon(wolfishCommand);
                    break;
            }
        }

        public static void NoArgsValidation(string[] args)
        {
            if (args.Length == 0)
            {
                //ShowHelp();
                return;
            }

        }

        public static void CommandValitadon(string command)
        {

            if (command != "new" &&
                command != "ask" &&
                command != "download" &&
                command != "welcome" &&
                command != "init" &&
                command != "finish")
            {
                Console.Error.WriteLine("Erro: Comando deve ser 'new' ou 'init' ou 'finish'.");
                //ShowHelp();
                return;
            }

        }

        public static void WelcomeCommand(string[] args)
        {
            var command = args[0];
            var semver = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            if (command == "welcome")
            {
                Console.WriteLine($"\nTank you! I'm happy to be here! \nAnd I'm now runnig on version {semver}");
            }
        }

        public static void AskCommand(string[] args)
        {
            var command = args[0];
            var question = args[1];

            if (command == "ask")
            {
                WolfishCommand.AskGemini(question);
            }
        }

        public static void DownloadCommand(string[] args)
        {
            var command = args[0];
            var tool = args[1];

            if (command == "download")
            {
                WolfishCommand.Download(tool);
            }
        }

        public static void CreateCommand(string[] args)
        {
            var command = args[0];
            var type = args[1];
            var local = args[2];
            var epic = args[3];

            if (command == "new")
            {
                if (local == "in")  WolfishCommand.NewIssue(type, epic);                
                else WolfishCommand.NewIssue(type);
            }
        }

        public static void InitCommand(string[] args)
        {
            var command = args[0];
            var issue = args[1];
            var origin = args[2];

            if (command == "init")
            {
                WolfishCommand.InitDevelopment(issue, origin);
            }
        }

        public static void FinishCommand(string[] args)
        {
            var command = args[0];
            var issue = args[1];
            var target = args[2];

            if (command == "finish")
            {
                WolfishCommand.FinishDevelopment(issue, target);
            }
        }

        public static void MergeCommand(string[] args)
        {
            var command = args[0];
            var origin = args[1];
            var target = args[2];

            if (command == "merge")
            {
                WolfishCommand.MergeBranch(origin, target);
            }
        }

    }
}
