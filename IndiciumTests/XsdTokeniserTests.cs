using System.IO;
using System.Linq;
using Indicium.Extensions;
using NUnit.Framework;
using W3C.XSD;
using Xml.Schema.Linq;

namespace Indicium.Tests
{
    public class XsdTokeniserTests
    {
        [Test]
        public void SimpleTest1()
        {
            var xsd = schema.Load(Shared.SensibleSqlKeywordTypesSchema);
            var resolved = xsd.ResolveIncludes(Shared.SensibleSqlKeywordTypesSchema.DirectoryName);
            
            var simpleTypesForElements =
                resolved.element.Select(e => resolved.simpleType.First(st => st.Content.name == e.Content.type.Name)).ToList();

            Assert.IsNotEmpty(simpleTypesForElements);

            var patterns = simpleTypesForElements.Select(s => s.Content.restriction.pattern.First()).ToList();

            Assert.IsNotEmpty(patterns);
            Assert.IsTrue(simpleTypesForElements.Count == patterns.Count);
        }

        [Test]
        public void ResolveElementTypesTest()
        {
            var xsd = schema.Load(Shared.SensibleSqlKeywordTypesSchema);
            var resolved = xsd.ResolveIncludes(Shared.SensibleSqlKeywordTypesSchema.DirectoryName);

            var typesResolved = resolved.ResolveElementTypes();

            var simpleTypes = typesResolved.element.Select(e => e.Content.simpleType);
            Assert.IsTrue(simpleTypes.Any(s => s != null));
        }

        [Test]
        public void ConstructorTest()
        {
            var xsd = schema.Load(Shared.SensibleSqlKeywordTypesSchema);
            var tokeniser = new XsdTokeniser(xsd);
        }
    }
}