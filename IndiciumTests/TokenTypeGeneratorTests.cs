using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using Indicium.Schemas;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Xml.Schema.Linq.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Indicium.Tests
{
    [TestFixture]
    public class TokenTypeGeneratorTests
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
            var cns = TokenTypeGenerator.GenerateTokenClasses(Context);
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
            var ccu = TokenTypeGenerator.GenerateClassesForTokenDefinitions(Context.Token);

            var code = ccu.ToCSharpString();

            File.WriteAllText("CodeDom.cs", code, Encoding.UTF8);
        }

        [Test]
        public void TestRoslynTransformationOfCodeDomCode()
        {
            var ccu = TokenTypeGenerator.GenerateClassesForTokenDefinitions(Context.Token);

            var tree = ccu.ToSyntaxTree();

            var nds = tree.GetRoot().DescendantNodes()?.OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

            nds = nds.AddExtractLexemeExtensionMethod();

            Assert.IsNotNull(nds);

            var fullString = nds.NormalizeWhitespace(elasticTrivia: true).ToFullString();

            File.WriteAllText("CodeDom_transformByRoslyn.cs", fullString, Encoding.UTF8);
        }
    }
}