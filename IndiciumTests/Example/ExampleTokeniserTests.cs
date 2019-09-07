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

            var lexemes = eg.ReadTokens(@"  identifier", 1).ToList();

            Assert.IsNotEmpty(lexemes);
            Assert.IsTrue(lexemes.Count == 3);
        }
    }
}