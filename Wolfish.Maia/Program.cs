using System.Diagnostics;
using System.Reflection;
using Wolfish.Gemini;

namespace Wolfish.Maia
{
    public class Program
    {
        static void Main(string[] args)
        {
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

        public static void AskCommand(string[] args)
        {
            var command = args[0];
            var question = args[1];

            if (command == "ask")
            {
                AskGemini(question);
            }
        }

        public static void AskGemini(string question)
        {
            var agent = new GeminiService("AIzaSyAjR1Yw-JTzTi63K4WI93PVBHunWgHZ7JE").Builder();

            var answer = agent.GenerativeTextAsync(question);

            var resposta = answer.Result;

            Console.WriteLine(resposta);
        }

        public static void DownloadCommand(string[] args)
        {
            var command = args[0];
            var tool = args[1];

            if (command == "download")
            {
                Download(tool);
            }
        }

        public static void Download(string tool)
        {
            Console.Write($"Baixando {tool}");

            var output = ProcessCommand("curl", "-o chrome_win_x64.exe https://dl.google.com/chrome/install/standalonesetup.exe");

            Console.WriteLine("Saída do processo:\n" + output);

            //Task.Delay(200).Wait();
        }

        public static void WelcomeCommand(string[] args)
        {
            var command = args[0];

            var version = Assembly.GetEntryAssembly()?.GetName().Version;
            var semver = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            if (command == "welcome")
            {
                Console.WriteLine($"\nTank you! I'm happy to be here! \nAnd I'm now runnig on version {semver}");
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
                if (local == "in")  NewIssue(type, epic);                
                else NewIssue(type);
            }
        }

        public static void NewIssue(string type, string? epic = null)
        {
            Console.Write($"Criando um nova issue do tipo {type}");
            if (epic is not null) Console.WriteLine($" no epico {epic}");

            var output = ProcessCommand("git", "status");

            Console.WriteLine("Saída do processo:\n" + output);

            Task.Delay(200).Wait();
        }

        public static void InitCommand(string[] args)
        {
            var command = args[0];
            var issue = args[1];
            var origin = args[2];

            if (command == "init")
            {
                InitDevelopment(issue, origin);
            }
        }

        private static void InitDevelopment(string issueId , string origin)
        {            
            Console.Write($"Iniciando o desenvolvimento da issue {issueId} a partir da branch {origin}");

            ProcessCommand("git", $"pull origin {origin}");

            //retorna os dados da issue no jira
            var issue = new Issue{Id = issueId, Type = "bugfix", Subject = "titulo-da-issue"};

            //movimenta o ard para desenvolvimento

            ProcessCommand("git", $"checkout -b \"{issue.Type}\\{issue.Id}-{issue.Subject}\" \"{origin}\"");

        }

        public static void FinishCommand(string[] args)
        {
            var command = args[0];
            var issue = args[1];
            var target = args[2];

            if (command == "finish")
            {
                FinishDevelopment(issue, target);
            }
        }

        private static void FinishDevelopment(string issueId, string target)
        {
            Console.Write($"Finalizando o desenvolvimento da issue {issueId} a publicando PR em {target}");

            ProcessCommand("git", $"push origin");

            //movimenta o card para Code_Review

            ProcessCommand("git", $"pull origin {target}");

            //cria a pull request para github/gitlab/azure
        }

        private static string ProcessCommand(string command, string arguments)
        {
            string diretorioAtual = Directory.GetCurrentDirectory();

            var startInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                //WorkingDirectory = "C:\\Users\\renat\\source\\Projetos"
            };

            var process = Process.Start(startInfo);
            var output = process?.StandardOutput.ReadToEnd();
            process?.WaitForExit();

            return output;
        }

        private class Issue
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string Subject { get; set; }
        }
    }
}
