using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using Indicium.Extensions;
using NUnit.Framework;
using Xml.Fxt;
using Xml.Schema.Linq.Extensions;

namespace Indicium.Tests
{
    [TestFixture]
    public class SystemXmlSchemaParsingTests
    {
        [Test]
        public void T1()
        {
            const string file = @"Schemas\\SimpleC.xsd";

            var xmlReaderSettings = new XmlReaderSettings() {
                CloseInput = true,
                DtdProcessing = DtdProcessing.Ignore
            };
            XmlSchemaSet schemaSet;
            using (var reader = XmlReader.Create(file, xmlReaderSettings)) {
                schemaSet = reader.ToXmlSchemaSet();

                Assert.IsNotNull(schemaSet);
            }

            var globalTypes = schemaSet.GlobalXsdTypes().ToList();
            Assert.IsNotEmpty(globalTypes);

            var simpleTypes = globalTypes.OfType<XmlSchemaSimpleType>().ToList();
            Assert.IsNotEmpty(simpleTypes);

            var complexTypes = globalTypes.OfType<XmlSchemaComplexType>().ToList();
            Assert.IsNotEmpty(complexTypes);

            List<XmlSchemaSimpleType> simpleTypesWthFacets = simpleTypes.Where(st =>
                (st.Content as XmlSchemaSimpleTypeRestriction)?.Facets.Cast<XmlSchemaFacet>().Any() ?? false).ToList();

            List<XmlSchemaSimpleType> simpleTypesWithPatternFacets = simpleTypesWthFacets.Where(st =>
                    (st.Content as XmlSchemaSimpleTypeRestriction)?.Facets.OfType<XmlSchemaPatternFacet>().Any() ?? false)
                .ToList();

            var regexsAndTypes = new Dictionary<string, Regex>();
            foreach (XmlSchemaSimpleType type in simpleTypesWithPatternFacets) {
                var strFacets = type.Content as XmlSchemaSimpleTypeRestriction;

                Assert.IsNotNull(strFacets);

                var patternFacets = strFacets.GetXmlSchemaPatternFacets().ToList();
                Assert.IsNotEmpty(patternFacets);

                regexsAndTypes.Add(type.Name, new Regex(patternFacets.First().Value));
            }

            Assert.IsNotEmpty(regexsAndTypes);
        }
    }
}