using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Wolfish.Commands
{
    public static class IssueManagerCommand
    {

        public static void InitDevelopment(string issueId, string origin)
        {
            Console.Write($"Iniciando o desenvolvimento da issue {issueId} a partir da branch {origin}");

            ProcessCommand("git", $"pull origin {origin}");

            //retorna os dados da issue no jira
            var issue = new Issue { Id = issueId, Type = "bugfix", Subject = "titulo-da-issue" };

            //movimenta o ard para desenvolvimento

            ProcessCommand("git", $"checkout -b \"{issue.Type}\\{issue.Id}-{issue.Subject}\" \"{origin}\"");

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

            ProcessCommand("git", "status");
        }

        public static void MergeBranch(string origin, string target)
        {
            var organization = "Projetos-dot-NET";
            var repository = "WolfishTools";

            var headDetalhado = "-H \"Accept: application/vnd.github+json\" " +
                                "-H \"Authorization: Bearer SEU_TOKEN_GITHUB\" " +
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

            ProcessCommand("curl", comandoDetalhado);
        }

        private static void ProcessCommand(string command, string arguments)
        {
            //string diretorioAtual = Directory.GetCurrentDirectory();

            var startInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                WindowStyle = ProcessWindowStyle.Normal,
                //WorkingDirectory = "C:\\Users\\renat\\source\\Projetos"
            };

            var process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.CursorVisible = false;
                    Console.SetCursorPosition(0, 0);
                    Console.Write(e.Data + "                                         ");
                }

            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Console.WriteLine("[ERRO] " + e.Data);
            };

            process.Start();


            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process?.WaitForExit();
        }

        private class Issue
        {
            public string? Id { get; set; }
            public string? Type { get; set; }
            public string? Subject { get; set; }
        }
    }
}
