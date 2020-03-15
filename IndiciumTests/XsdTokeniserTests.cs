using System.IO;
using System.Linq;
using NUnit.Framework;
using W3C.XSD;

namespace Indicium.Tests
{
    public class XsdTokeniserTests
    {
        [Test]
        public void ConstructorTest()
        {
            var simpleC = schema.Load(new FileInfo(@"Schemas\SimpleC.xsd"));
            var sensibleSql = schema.Load(new FileInfo(@"Schemas\SensibleSQL_KeywordTypes.xsd"));
            
            simpleC = simpleC.ResolveIncludes();
            var xt = new XsdTokeniser(simpleC);
        }
    }
}