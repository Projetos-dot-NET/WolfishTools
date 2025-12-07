using System.Diagnostics;
using System.Text;

namespace Wolfish.Commands
{
    public static class TerminalCommand
    {

        // Comando      argumento       message     realcomand      real argument     
        // download     chrome          baixando... curl            -o chrome...
        // install      sdk8            instalano... winget            -o chrome...

        public static void Download(string tool)
        {
            Console.Clear();
            Console.Write($"Baixando {tool}\n_________________________________________________________________________________\n");
            ProcessCommand("curl", "-o chrome_win_x64.exe https://dl.google.com/chrome/install/standalonesetup.exe");
        }

        public static void Install(string tool)
        {
            Console.Clear();
            Console.Write($"Instalando {tool} sem perguntas\n ");
            ProcessCommand("winget", "install -h Microsoft.DotNet.SDK.7");
        }

        public static void Uninstall(string tool)
        {
            Console.Clear();
            Console.Write($"Desinstalando {tool} sem perguntas\n ");
            ProcessCommand("winget", "uninstall -h Microsoft.DotNet.SDK.9");
        }

        public static void List(string tool)
        {
            Console.Clear();
            Console.Write($"Listando todos  {tool} sem perguntas\n ");
            ProcessCommand("winget", "list");
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
                    Console.CursorLeft = 0;
                    Console.Write(e.Data + "                                         ");
                }
                    
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.CursorVisible = false;
                    Console.CursorLeft=0;
                    Console.Write(e.Data+"\n");
                }
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
