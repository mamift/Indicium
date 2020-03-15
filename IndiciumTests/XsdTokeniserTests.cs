using System.IO;
using NUnit.Framework;

namespace Indicium.Tests
{
    public class XsdTokeniserTests
    {
        [Test]
        public void ConstructorTest()
        {
            var simpleC = new FileInfo(@"Schemas\SimpleC.xsd");
            var sensibleSql = new FileInfo(@"Schemas\SensibleSQL_KeywordTypes.xsd");
            
        }
    }
}