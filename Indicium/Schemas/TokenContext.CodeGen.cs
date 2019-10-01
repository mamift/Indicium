using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Indicium.CodeGen;

namespace Indicium.Schemas
{
    public partial class TokenContext
    {
        /// <summary>
        /// Generates code necessary to model this <see cref="TokenContext"/> as a separate, compilable C# library.
        /// </summary>
        /// <returns></returns>
        public string GenerateCode()
        {
            //if (string.IsNullOrWhiteSpace(NamespaceName)) throw new Exception($"There is no @{nameof(NamespaceName)} defined in the token definition XML!");

            var ns = TokenContextTypeGenerator.GenerateTokeniserCode(this, NamespaceName);

            return ns.ToFullString();
        }

        /// <summary>
        /// Before loading a new token definition file, validate it first. Accepts an <see cref="IProgress{T}"/> that will output error messages.
        /// <para>The error messages include line and index numbers.</para>
        /// </summary>
        /// <param name="inputXml"></param>
        /// <param name="progress"></param>
        public static void Validate(string inputXml, IProgress<ValidationEventArgs> progress = null)
        {
            var thisAssembly = typeof(TokenContext).Assembly;
            var schemaName = thisAssembly.GetManifestResourceNames().First(n => n == "Indicium.Schemas.TokenSchema.xsd");
            var schemaStream = thisAssembly.GetManifestResourceStream(schemaName);

            if (schemaStream == null) return;

            using (schemaStream) {
                var schema = XmlSchema.Read(schemaStream, (sender, args) => throw args.Exception);

                var schemaSet = new XmlSchemaSet() {
                    CompilationSettings = new XmlSchemaCompilationSettings() {
                        EnableUpaCheck = false
                    },
                    XmlResolver = new XmlUrlResolver()
                };

                schemaSet.Add(schema);

                var xmlReaderSettings = new XmlReaderSettings() {
                    Schemas = schemaSet,
                    XmlResolver = new XmlUrlResolver(),
                    ConformanceLevel = ConformanceLevel.Document,
                    ValidationType = ValidationType.Schema,
                    ValidationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints |
                                      XmlSchemaValidationFlags.ReportValidationWarnings
                };

                if (progress != null) {
                    xmlReaderSettings.ValidationEventHandler += (sender, args) => progress.Report(args);
                }

                var reader = XmlReader.Create(File.OpenRead(inputXml), xmlReaderSettings);

                while (reader.Read()) { }
            }
        }
    }
}