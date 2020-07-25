using System.IO;
using System.Xml;
using System.Xml.Schema;
using Xml.Schema.Linq.Extensions;

namespace Indicium.Tests
{
    public static class Shared
    {
        public static readonly string Lyrics = File.ReadAllText("lyrics.txt");
        public static readonly FileInfo SimpleCSchema = new FileInfo(@"Schemas\SimpleC.xsd");
        public static readonly FileInfo SensibleSqlKeywordTypesSchema = new FileInfo(@"Schemas\SensibleSQL_KeywordTypes.xsd");

        public static XmlSchemaSet GetSimpleCSchemaSet()
        {
            var xmlReaderSettings = new XmlReaderSettings()
            {
                CloseInput = true,
                DtdProcessing = DtdProcessing.Ignore
            };
            using (var reader = XmlReader.Create(SimpleCSchema.FullName, xmlReaderSettings)) {
                var schemaSet = reader.ToXmlSchemaSet();
                return schemaSet;
            }
        }
    }
}