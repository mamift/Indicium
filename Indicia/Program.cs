using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Alba.CsConsoleFormat.Fluent;
using Indicium;
using Indicium.Schemas;
using Microsoft.CodeAnalysis;

namespace Indicia
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 2) {
                Colors.WriteLine($"Usage: {Path.GetFileName(Assembly.GetExecutingAssembly().Location)} ".White(), 
                    "<tokenSchema.xml> ".OnDarkBlue().White(),
                    "<inputFileToBeTokenised.txt>".OnDarkRed().White());
                Colors.WriteLine("Will then output tokenised output to:".White());
                Colors.WriteLine("<inputFileToBeTokenised.txt>.output".Yellow());

                return 0;
            }

            var tokenDefinitionFile = args.First();

            var tokeniser = TokenContext.Load(tokenDefinitionFile);

            var inputTextFile = args[1];
            var reader = new StreamReader(File.Open(inputTextFile, FileMode.Open));

            var tokens = tokeniser.ProcessTokens(reader).ToList();

            var dirOfInputTextFile = Path.GetDirectoryName(inputTextFile);
            var outputPath = Path.Combine(dirOfInputTextFile, $"{inputTextFile}.output");
            File.WriteAllText(outputPath, tokens.ToDelimitedString());

            return 0;
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
