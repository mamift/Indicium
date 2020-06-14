using System.IO;
using System.Xml;
using System.Xml.Resolvers;
using System.Xml.Schema;
using NUnit.Framework;
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
            using (var reader = XmlReader.Create(file, xmlReaderSettings)) {
                var schemaSet = reader.ToXmlSchemaSet();

                Assert.IsNotNull(schemaSet);
            }
        }
    }
}