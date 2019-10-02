using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Indicium.Schemas;
using NUnit.Framework;

namespace Indicium.Tests
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class XMLinCSharpTests
    {
        [Test]
        public void Test1()
        {
            var context = TokenContext.Load(@"Schemas\XMLinCSharp.xml");
            
            context.IgnoreWhitespace = true;

            Assert.IsTrue(TokenContext.RegexOptions.HasFlag(RegexOptions.Singleline));

            var cSharpCode =
                "public static void main() {\r\nConsole.WriteLine(\"XML in CSharp test\");\r\n\tvar instance = (<Invaders class=\"Irken\">\r\n\t\t<Zim>I am zim!</Zim></Invaders>);}";

            var lexemes = context.ProcessTokens(cSharpCode, default(char));

            Assert.IsNotEmpty(lexemes);
        }

        [Test]
        public void Test2()
        {
            var context = TokenContext.Load(@"Schemas\XMLinCSharp.xml");
            
            var cSharpExpression = @"var x = (<Invader>Zim</Invader>);";

            var lexemes = context.ProcessTokens(cSharpExpression, default(char));

            Assert.IsNotEmpty(lexemes);
        }
    }
}