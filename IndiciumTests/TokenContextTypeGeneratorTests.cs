using Indicium.CodeGen;
using Microsoft.CodeAnalysis;
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

            var namespaceDeclaration = TokenContextTypeGenerator.GenerateTokeniserType(context, "TokensNamespace");
            var codeString = namespaceDeclaration.NormalizeWhitespace().ToFullString();

            Assert.IsFalse(string.IsNullOrWhiteSpace(codeString));
        }
    }
}