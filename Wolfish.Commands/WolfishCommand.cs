using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Wolfish.Gemini;

namespace Wolfish.Commands
{
    public static class WolfishCommand
    {
        

        public static void Download(string tool)
        {
            Console.Write($"Baixando {tool}");

            var output = ProcessCommand("curl", "-o chrome_win_x64.exe https://dl.google.com/chrome/install/standalonesetup.exe");

            Console.WriteLine("Saída do processo:\n" + output);

            //Task.Delay(200).Wait();
        }


        public static void AskGemini(string question)
        {
            var agent = new GeminiService("AIzaSyAjR1Yw-JTzTi63K4WI93PVBHunWgHZ7JE").Builder();

            var answer = agent.GenerativeTextAsync(question);

            var resposta = answer.Result;

            Console.WriteLine(resposta);
        }

        public static void InitDevelopment(string issueId, string origin)
        {
            Console.Write($"Iniciando o desenvolvimento da issue {issueId} a partir da branch {origin}");

            ProcessCommand("git", $"pull origin {origin}");

            //retorna os dados da issue no jira
            var issue = new Issue { Id = issueId, Type = "bugfix", Subject = "titulo-da-issue" };

            //movimenta o ard para desenvolvimento

            ProcessCommand("git", $"checkout -b \"{issue.Type}\\{issue.Id}-{issue.Subject}\" \"{origin}\"");

        }

        private class Issue
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string Subject { get; set; }
        }

        public static void FinishDevelopment(string issueId, string target)
        {
            Console.Write($"Finalizando o desenvolvimento da issue {issueId} a publicando PR em {target}");

            ProcessCommand("git", $"push origin");

            //movimenta o card para Code_Review

            ProcessCommand("git", $"pull origin {target}");

            //cria a pull request para github/gitlab/azure
        }

        public static void NewIssue(string type, string? epic = null)
        {
            Console.Write($"Criando um nova issue do tipo {type}");
            if (epic is not null) Console.WriteLine($" no epico {epic}");

            var output = ProcessCommand("git", "status");

            Console.WriteLine("Saída do processo:\n" + output);

            Task.Delay(200).Wait();
        }

        public static void MergeBranch(string origin, string target)
        {
            var organization = "Projetos-dot-NET";
            var repository = "WolfishTools";

            var headDetalhado = "-H \"Accept: application/vnd.github+json\" " +
                                "-H \"Authorization: Bearer MEU_TOKEN\" " +
                                "-H \"X-GitHub-Api-Version: 2022-11-28\"";

            var link = $"https://api.github.com/repos/{organization}/{repository}/pulls";

            var jsonSimples = new
            {
                title = "Meu novo PR Incrivel",
                body = "Por favor, revise este codigo",
                head = origin,
                Base = target,
                draft = false,
            };

            var jsonSerializado = JsonSerializer.Serialize(jsonSimples);

            var jsonPreparado = jsonSerializado.Replace("\"", "\\\"").Replace("B", "b");

            var comandoDetalhado = $"curl -L -X POST {headDetalhado} \"{link}\" -d \"{jsonPreparado}\"";

            var resposta = ProcessCommand("curl", comandoDetalhado);
            Console.WriteLine(resposta);
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
    }
}
