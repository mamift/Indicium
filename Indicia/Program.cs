using System.IO;
using System.Linq;
using Indicium.Tokens;

namespace Indicia
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var tokenDefinitionFile = args.First();

            var tokenProcessor = new TokenProcessor(File.ReadAllLines(tokenDefinitionFile));
            var tokenDefinitions = tokenProcessor.Tokeniser.TokenDefinitions;

            tokenProcessor.Tokeniser.InputString = File.ReadAllText(args[1]);
            var tokensFromInput = tokenProcessor.Tokeniser.GetTokens().ToList();
        }
    }
}
