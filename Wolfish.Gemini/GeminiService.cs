using Mscc.GenerativeAI;
using System.IO;
//using Wolfish.Core;

namespace Wolfish.Gemini
{
    public class GeminiService
    {
        private GenerativeModel _model;
        private readonly GoogleAI _google;

        public GeminiService(string apiKey) 
        {
            _google = new GoogleAI(apiKey);
            _model = _google.GenerativeModel(Model.Gemini25FlashLite);
        }

        public GeminiService(string apiKey, string instruction)
        {
            _google = new GoogleAI(apiKey);
            _model = _google.GenerativeModel(Model.Gemini25FlashLite, systemInstruction: new Content(instruction));
        }

        public GeminiService BuilderPro()
        {
            _model = _google.GenerativeModel(Model.Gemini25Pro);
            return this;
        }

        public GeminiService Builder()
        {
            _model = _google.GenerativeModel(Model.Gemini25Flash);
            return this;
        }

        public void ReadConfig (/*IConfiguration config*/)
        {
            //var apikey = config["gemini:apikey"];

        }

        public async Task<string> GenerativeTextAsync(string prompt)
        {
            try
            {
                
                var response = await _model.GenerateContent(prompt);
                return response.Text??"Sem Resposta!!!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> GenerativeStreamingTextAsync(string prompt)
        {
            try
            {
                
                var response = await _model.GenerateContent(prompt);
                return response.Text?? "Sem Resposta!!!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
