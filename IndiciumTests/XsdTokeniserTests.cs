using System.IO;
using System.Linq;
using Indicium.Extensions;
using NUnit.Framework;
using W3C.XSD;
using Xml.Schema.Linq;

namespace Indicium.Tests
{
    [TestFixture]
    public class XsdTokeniserTests
    {
        [Test]
        public void SimpleTypeDefinitionTest()
        {
            var xsd = schema.Load("Schemas\\test_SimpleTypeDefinitions.xsd");

            xsd = xsd.CopyTypeDefinitionsInline();
            Assert.IsTrue(xsd.element.All(e => e.Content.simpleType != null || e.Content.complexType != null));

            Assert.IsTrue(xsd.simpleType.Any());
        }

        [Test]
        public void SimpleCSimpleTypeDefinitionTest()
        {
            var xsd = schema.Load("Schemas\\SimpleC.xsd");

            xsd = xsd.ResolveIncludes("Schemas");

            xsd = xsd.CopyTypeDefinitionsInline();

            Assert.IsNotEmpty(xsd.simpleType);
            Assert.IsNotEmpty(xsd.complexType);
            Assert.IsNotEmpty(xsd.element);
        }

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

            var typesResolved = resolved.CopyTypeDefinitionsInline();

            var simpleTypes = typesResolved.element.Select(e => e.Content.simpleType);
            Assert.IsTrue(simpleTypes.Any(s => s != null));
        }

        [Test]
        public void ConstructorTest()
        {
            var xsd = schema.Load(Shared.SimpleCSchema);
            var tokeniser = new XsdTokeniser(xsd);
        }
    }
}