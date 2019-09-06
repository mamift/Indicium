using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Indicium.Schemas;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace Indicium.Tests
{
    [TestFixture]
    public class TypeGeneratorTests
    {
        public TokenContext Context { get; set; }

        [SetUp]
        public void Setup()
        {
            var cwd = Environment.CurrentDirectory;
            var file = Path.Combine(cwd, @"Schemas\Prototype1.xml");

            Context = TokenContext.Load(file);

            Assert.IsNotNull(Context);
        }

        [Test]
        public void TestRoslynCodeGeneration()
        {
            var cns = TypeGenerator.GenerateTokenClasses(Context);
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(cns.NormalizeWhitespace().ToFullString());
            //foreach (var c in cns)
            //{
            //    roslynSb.Append($"{c.NormalizeWhitespace().ToFullString()}\r\n");
            //}

            var str = stringBuilder.ToString();
            File.WriteAllText(@".\RoslynCode.cs", str);
        }

        [Test]
        public void TestCodeDomGeneration()
        {
            var ccu = TypeGenerator.GenerateClassesForTokenDefinitions(Context.Token);

            var cdProvider = CodeDomProvider.CreateProvider("CSharp");
            var cdOptions = new CodeGeneratorOptions
            {
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
    }
}