using Wolfish.Gemini;

namespace Wolfish.Commands
{
    public static class AgentCommand
    {
        public static void AskGeminiPro(string question, string instruction = null)
        {
            var agent = new GeminiService("SEU_TOKEN_DO_GEMINI", instruction).BuilderPro();

            var answer = agent.GenerativeTextAsync(question);

            var resposta = answer.Result;

            Console.WriteLine(resposta);
        }

        public static void AskGeminiFlash(string question, string instruction = null)
        {
            var agent = new GeminiService("SEU_TOKEN_DO_GEMINI", instruction).BuilderFlash();

            var answer = agent.GenerativeTextAsync(question);

            var resposta = answer.Result;

            Console.WriteLine(resposta);
        }

    }
}
