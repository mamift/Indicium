using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Indicium;
using Indicium.Schemas;
using Microsoft.CodeAnalysis;

namespace Indicia
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var tokenDefinitionFile = args.First();

            var tokenProcessor = TokenContext.Load(tokenDefinitionFile);

            tokenProcessor.InputString = File.ReadAllText(args[1]);
            var tokens = tokenProcessor.GetTokens().ToList();

            RoslynCodeGen(tokenProcessor);

            CodeDomCodeGen(tokens);
        }

        private static void CodeDomCodeGen(IEnumerable<Token> tokenDefinitions)
        {
            var ccu = TypeGenerator.GenerateClassesForTokenDefinitions(tokenDefinitions);

            var cdProvider = CodeDomProvider.CreateProvider("CSharp");
            var cdOptions = new CodeGeneratorOptions {
                BlankLinesBetweenMembers = true,
                BracingStyle = "Block",
                VerbatimOrder = true
            };
            var codeDomSb = new StringBuilder();
            var sw = new StringWriter(codeDomSb);
            cdProvider.GenerateCodeFromCompileUnit(ccu, sw, cdOptions);
            var code = sw.ToString();
            File.WriteAllText("CodeDom.cs", code, Encoding.UTF8);
        }

        private static void RoslynCodeGen(TokenContext tokenProcessor)
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
