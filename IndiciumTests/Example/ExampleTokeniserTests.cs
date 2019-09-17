using System.Linq;
using Indicium.Example;
using NUnit.Framework;

namespace Indicium.Tests.Example
{
    public class ExampleTokeniserTests
    {
        [Test]
        public void Test1()
        {
            var eg = new ExampleTokeniser();

            var lexemes = eg.ReadTokens(@"  identifier ", 1).ToList();

            Assert.IsNotEmpty(lexemes);
            Assert.IsTrue(lexemes.Count == 3);

            var next = eg.ReadToken();

            Assert.IsNull(next);
        }

        [Test]
        public void PeekTwiceTokenTest()
        {
            const string inputString = "@  whitespace ";
            var eg = new ExampleTokeniser {
                InputString = inputString
            };

            Assert.IsTrue(eg.LineIndex == 0);
            Assert.IsTrue(eg.LineNumber == 1);

            var lexeme1 = eg.ReadToken();

            Assert.IsNotNull(lexeme1);
            Assert.IsTrue(lexeme1.LineIndex == 0); // undefined token at the beginning

            // incremented by one
            Assert.IsTrue(eg.LineIndex == 1);
            // so string remaining is now less 1 char
            Assert.IsTrue(eg.RemainingString.Length == inputString.Length - 1);
            // the tokeniser line index should've moved past the lexeme line index
            Assert.IsTrue(eg.LineIndex != lexeme1.LineIndex);
            Assert.IsTrue(eg.LineIndex > lexeme1.LineIndex);

            // take a peek without consuming
            var peeked = eg.PeekToken();

            // confirm these remained the same...
            Assert.IsTrue(eg.LineIndex == 1);
            Assert.IsTrue(eg.RemainingString.Length == inputString.Length - 1);

            Assert.IsNotNull(peeked);
            Assert.IsTrue(peeked.Token is WhitespaceToken);
            // the peeked LI should equal the tokeniser LI at this point
            Assert.IsTrue(peeked.LineIndex == 1);
            Assert.IsTrue(eg.LineIndex == peeked.LineIndex);

            var anotherPeek = eg.PeekToken();

            Assert.AreEqual(peeked, anotherPeek);

            // another peek and nothing should've changed
            Assert.IsTrue(eg.LineIndex == 1);
            Assert.IsTrue(eg.RemainingString.Length == inputString.Length - 1);

            // after reading, now it should be incremented on the tokeniser
            Assert.IsTrue(eg.LineIndex == anotherPeek.LineIndex);

            Assert.AreNotEqual(lexeme1, peeked);
            Assert.AreNotEqual(lexeme1, anotherPeek);
        }
    }
}