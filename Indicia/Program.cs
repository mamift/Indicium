using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Alba.CsConsoleFormat.Fluent;
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

            var tokeniser = TokenContext.Load(tokenDefinitionFile);

            var reader = new StreamReader(File.Open(args[1], FileMode.Open));

            var tokens = tokeniser.ProcessTokens(reader).ToList();

            foreach (var tokenValue in tokens) {
                Colors.WriteLine(tokenValue.ToString().White());
            }
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
