using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Indicium;
using Indicium.Tokens;
using Microsoft.CodeAnalysis;

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

            RoslynCodeGen(tokenProcessor);

            CodeDomCodeGen(tokenDefinitions);
        }

        private static void CodeDomCodeGen(IEnumerable<TokenDefinition> tokenDefinitions)
        {
            var ccu = TypeGenerator.GenerateCodeUnits(tokenDefinitions);

            var cdProvider = CodeDomProvider.CreateProvider("CSharp");
            var cdOptions = new CodeGeneratorOptions {
                BlankLinesBetweenMembers = true,
                BracingStyle = "C",
                VerbatimOrder = true
            };
            var codeDomSb = new StringBuilder();
            var sw = new StringWriter(codeDomSb);
            cdProvider.GenerateCodeFromCompileUnit(ccu, sw, cdOptions);
            var code = sw.ToString();
        }

        private static void RoslynCodeGen(TokenProcessor tokenProcessor)
        {
            var cn = TypeGenerator.GenerateTokenClasses(tokenProcessor);
            var roslynSb = new StringBuilder();
            foreach (var c in cn) {
                roslynSb.Append($"{c.NormalizeWhitespace().ToFullString()}");
            }

            var str = roslynSb.ToString();
        }
    }
}
