using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Wolfish.Commands
{
    public class WolfishCommand
    {
        public readonly string _path;
        public WolfishCommand()
        {
            _path = "./WindowsCommands.json";
        }

        public WolfishCommand(string path)
        {
            _path = path;
        }

        public async Task<List<string>> ListCommandNames()
        {
            var comandNames = new List<string>();

            using var fileStream = File.OpenRead(_path);
            var commands = await JsonSerializer.DeserializeAsync<List<TerminalCommandDto>>(fileStream);
            if (commands != null) foreach (var item in commands) comandNames.Add(item.Gun);

            return comandNames;
        }

        public async Task<bool> SeekAndExecute(string name, string target)
        {
            using var fileStream = File.OpenRead(_path);
            var commands = await JsonSerializer.DeserializeAsync<List<TerminalCommandDto>>(fileStream);
            
            var current = commands?.FirstOrDefault(_ => _.Gun == name && _.Aim == target);
            if (current != null)
            {
                Console.Clear();
                Console.Write(current.Msg);
                Console.Write("\n_________________________________________________________________________________\n");
                ProcessCommand(current.Cmd, current.Arg);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void ProcessCommand(string command, string arguments)
        {
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

            var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };

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
                    Console.CursorLeft = 0;
                    Console.Write(e.Data + "\n");
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process?.WaitForExit();
        }

    }
}
