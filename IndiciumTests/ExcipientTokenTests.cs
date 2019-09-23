using System.Collections.Generic;
using System.Linq;
using Indicium.Schemas;
using NUnit.Framework;

namespace Indicium.Tests
{
    [TestFixture]
    public class ExcipientTokenTests
    {
        [Test]
        public void Test1()
        {
            var context = new TokenContext {
                Token = new List<Token> {
                    new Token { Id = "OpenBrace", Description = "Opening brace", TypedValue = "{" },
                    new Token { Id = "CloseBrace", Description = "Closing brace", TypedValue = "}" },
                    new Token { Id = "Identifier", Description = "Identifier text", TypedValue = "[\\w]+" }
                },
                IgnoreSpaces = true,
                LineNumber = 1
            };

            var output = context.ProcessTokens("survey { title { A title } }");
            var outputLexemes = output.ToList();

            Assert.IsNotEmpty(outputLexemes);
            Assert.IsTrue(outputLexemes.All(l => l.LineNumber == 1));

            var last = outputLexemes.Last();
            Assert.IsTrue(last.LineIndex == 27);
            Assert.IsTrue(last.TypedValue == "}");
        }
    }
}