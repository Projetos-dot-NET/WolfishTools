using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Wolfish.Commands
{
    public class WolfishCommand
    {
        public readonly string _path;
        public static int MaxColumnWidth { get; set; } = 30;

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
        
        public List<TerminalCommandDto> LoadFromJson()
        {
            if (!File.Exists(_path))
                throw new FileNotFoundException("Arquivo JSON não encontrado", _path);

            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<TerminalCommandDto>>(json)!;
        }

        public string BuildTable(List<TerminalCommandDto> items)
        {
            var headers = new[] { "GUN", "AIM", "MESSAGE", "CMD", "ARG" };

            var rows = items.Select(i => new[]
            {i.Gun, i.Aim, i.Msg, i.Cmd, i.Arg}).ToList();


            int[] colWidths = new int[headers.Length];

            for (int c = 0; c < headers.Length; c++)
            {
                int headerWidth = headers[c].Length;
                int maxRowWidth = rows.Max(r => r[c].Length);
                colWidths[c] = Math.Max(headerWidth, maxRowWidth) + 2;
            }

            string Separator()
            {
                string line = "+";
                foreach (var w in colWidths)
                    line += new string('-', w) + "+";
                return line;
            }

            string header = "|" + string.Join("", headers.Select((h, i) => h.PadRight(colWidths[i]) + "|"));
            string sep = Separator();

            var body = rows.Select(row =>
                "|" + string.Join("", row.Select((c, i) => c.PadRight(colWidths[i]) + "|"))
            );

            return sep + "\n" + header + "\n" + sep + "\n" + string.Join("\n", body) + "\n" + sep;
        }

        public string BuildLimidetTable(List<TerminalCommandDto> items)
        {
            var headers = new[] { "GUN", "AIM", "MESSAGE", "CMD", "ARG" };

            var rows = items.Select(i => new[]{i.Gun, i.Aim, Truncate(i.Msg, MaxColumnWidth), Truncate(i.Cmd, MaxColumnWidth), Truncate(i.Arg, MaxColumnWidth)}).ToList();

            int[] colWidths = new int[headers.Length];

            for (int c = 0; c < headers.Length; c++)
            {
                int headerWidth = headers[c].Length;
                int maxRowWidth = rows.Max(r => r[c].Length);
                colWidths[c] = Math.Min(Math.Max(headerWidth, maxRowWidth) + 2, MaxColumnWidth + 2);
            }

            string Separator()
            {
                string line = "+";
                foreach (var w in colWidths)
                    line += new string('-', w) + "+";
                return line;
            }

            string header = "|" + string.Join("", headers.Select((h, i) => h.PadRight(colWidths[i]) + "|"));
            string sep = Separator();

            var body = rows.Select(row =>
                "|" + string.Join("", row.Select((c, i) => c.PadRight(colWidths[i]) + "|"))
            );

            return sep + "\n" + header + "\n" + sep + "\n" + string.Join("\n", body) + "\n" + sep;
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

        private static string Truncate(string text, int max)
        {
            if (text.Length <= max) return text;
            return text.Substring(0, max - 1) + "…"; // adiciona reticência
        }

    }
}
