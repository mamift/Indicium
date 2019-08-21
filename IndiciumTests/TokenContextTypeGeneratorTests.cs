using System.IO;
using CSharpSyntax.Printer;
using NUnit.Framework;

namespace Indicium.Tests
{
    [TestFixture]
    public class TokenContextTypeGeneratorTests
    {
        [Test]
        public void Test1()
        {
            var context = TokenContextTests.GetDefaultContext();

            var classDeclaration = TokenContextTypeGenerator.TokenDefinitionClasses(context, "CustomContext");

            var textWriter = new StringWriter();
            var printer = new SyntaxPrinter(new SyntaxWriter(textWriter));
            printer.Visit(classDeclaration);
            
            var code = textWriter.GetStringBuilder().ToString();

            File.WriteAllText(@".\RoslynCode.cs", code);
        }
    }
}