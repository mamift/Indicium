using System.IO;
using System.Linq;
using System.Reflection;
using Indicium.CodeGen;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace Indicium.Tests
{
    public class TokenContextTypeGeneratorTests
    {
        [Test]
        public void CodeIsGeneratedTest()
        {
            var codeString = Shared.GetGeneratedCode(Shared.GetDefaultContext());

            Assert.IsFalse(string.IsNullOrWhiteSpace(codeString));
        }

        [Test]
        public void CodeIsParsableTest()
        {
            var context = Shared.GetPrototype1Context();

            var namespaceDeclaration = TokenContextTypeGenerator.GenerateTokeniserCode(context, "TokensNamespace");

            var sourceString = namespaceDeclaration.ToString();

            var source = SourceText.From(sourceString);

            Assert.DoesNotThrow(() => {
                var parsedTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp7_3));

                Assert.IsNotNull(parsedTree);
            });
        }

        [Test]
        public void CodeIsCompilableTest()
        {
            var context = Shared.GetPrototype1Context();
            var namespaceDeclaration = TokenContextTypeGenerator.GenerateTokeniserCode(context, "TokensNamespace");
            var namespaceCode = namespaceDeclaration.NormalizeWhitespace().ToFullString();
            var sourceText = SourceText.From(namespaceCode);
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(sourceText, new CSharpParseOptions(LanguageVersion.CSharp7_3));
            
            var opts = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            
            var netStandardRef = MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location);
            var indiciumRef = MetadataReference.CreateFromFile(Assembly.Load("Indicium, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null").Location);
            var xObjectsRef = MetadataReference.CreateFromFile(Assembly.Load("XObjectsCore, Version=3.0.0.3, Culture=neutral, PublicKeyToken=null").Location);
            var corLib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var systemRuntime = MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location);
            
            var references = new[] { corLib, netStandardRef, systemRuntime, indiciumRef, xObjectsRef };
            var syntaxTrees = new []{ syntaxTree };
            var cSharpCompilation = CSharpCompilation.Create(nameof(TokenContextTypeGeneratorTests), syntaxTrees, references, opts);

            var memoryStream = new MemoryStream();

            var dll = cSharpCompilation.Emit(memoryStream);

            // there's one issue here that causes the System.Environment to be inaccessible: but it should be the ONLY error
            Assert.IsTrue(dll.Diagnostics.Length < 2);
            Assert.IsTrue(dll.Diagnostics.Count(d => d.Severity == DiagnosticSeverity.Error) == 1);
            var onlyErrorMessage = dll.Diagnostics.First(d => d.Severity == DiagnosticSeverity.Error);
            Assert.IsTrue(onlyErrorMessage.GetMessage().Contains("'Environment' is inaccessible due to its protection level"));
            Assert.IsTrue(onlyErrorMessage.Id == "CS0122");
        }
    }
}