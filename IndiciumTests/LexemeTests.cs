using System.Reflection;
using Indicium.Schemas;
using NUnit.Framework;

namespace Indicium.Tests
{
    public class LexemeTests
    {
        [Test]
        public void EqualityTest1()
        {
            var lex = new Lexeme() {
                Id = "Test",
                TypedValue = "_test"
            };

            var undefined = Lexeme.Undefined;

            Assert.AreNotEqual(lex, undefined);

            Assert.IsFalse(lex == undefined);
        }
    }
}