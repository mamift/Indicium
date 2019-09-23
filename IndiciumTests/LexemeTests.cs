using System.Reflection;
using Indicium.Schemas;
using NUnit.Framework;

namespace Indicium.Tests
{
    public class LexemeTests
    {
        [Test]
        public void EqualityTestById1()
        {
            var lex = new Lexeme() {
                Id = "Test",
                TypedValue = "_test"
            };

            Assert.IsFalse(lex == Lexeme.Undefined);
        }

        [Test]
        public void EqualityTestById2()
        {
            var lex = new Lexeme() {
                Id = "Undefined",
                TypedValue = "_test"
            };

            Assert.IsTrue(lex == Lexeme.Undefined);
        }

        [Test]
        public void EqualityTestByTypedValue()
        {
            var defaultLexeme = new Lexeme() {
                Id = "Test",
                TypedValue = "default"
            };

            var returnLexeme = new Lexeme() {
                Id = "DefaultKeyword",
                TypedValue = "default"
            };

            Assert.IsFalse(defaultLexeme == returnLexeme);
        }
    }
}