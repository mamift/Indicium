using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Indicium.Tokens;
using Newtonsoft.Json;

namespace Indicium
{
    public class Parser
    {
        public readonly string GrammarFilePath;

        public readonly Grammar Grammar;

        public List<Token> Tokens { get; private set; } = new List<Token>();

        public Parser(string grammarFilePath)
        {
            GrammarFilePath = grammarFilePath;
            Grammar = Grammar.Load(GrammarFilePath);
        }

        public void Tokenise(string inputString)
        {
            Grammar.Tokeniser.InputString = inputString;
            Tokens = Grammar.Tokeniser.GetTokens().ToList();
        }

        public string ToJson()
        {
            var jsonSerializerSettings = new JsonSerializerSettings {
                Culture = CultureInfo.InvariantCulture
            };
            return JsonConvert.SerializeObject(Grammar.Tokeniser.TokenDefinitions, Formatting.Indented,
                jsonSerializerSettings);
        }
    }
}