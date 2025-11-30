using Wolfish.Gemini;

namespace Wolfish.Commands
{
    public static class AgentCommand
    {
        public static void AskGemini(string question, string instruction)
        {
            var agent = new GeminiService("SEU_TOKEN_DO_GEMINI", instruction).BuilderPro();

            var answer = agent.GenerativeTextAsync(question);

            var resposta = answer.Result;

            Console.WriteLine(resposta);
        }
        
    }
}
