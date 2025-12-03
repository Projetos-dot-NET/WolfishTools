
using System.Text.Json.Serialization;

namespace Wolfish.Commands
{    public class TerminalCommandDto
    {
        [JsonPropertyName("gun")]
        public string Gun { get; set; }

        [JsonPropertyName("aim")]
        public string Aim { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("cmd")]
        public string Cmd { get; set; }

        [JsonPropertyName("arg")]
        public string Arg { get; set; }
    }
}
