using System;
using System.Collections.Generic;
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
        /// <returns>Returns any validation events (<see cref="ValidationEventArgs"/>).</returns>
        /// <param name="inputXml"></param>
        public static List<ValidationEventArgs> Validate(string inputXml)
        {
            var thisAssembly = typeof(TokenContext).Assembly;
            var schemaName = thisAssembly.GetManifestResourceNames().First(n => n == "Indicium.Schemas.TokenSchema.xsd");
            var schemaStream = thisAssembly.GetManifestResourceStream(schemaName);

            if (schemaStream == null) throw new Exception("Cannot read the TokenSchema.xsd schema file.");

            using (schemaStream) {
                // throw exceptions if there are any errors with the XSD itself
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

                var validationErrors = new List<ValidationEventArgs>();
                xmlReaderSettings.ValidationEventHandler += (sender, args) => validationErrors.Add(args);

                using (var reader = XmlReader.Create(File.OpenRead(inputXml), xmlReaderSettings)) {
                    while (reader.Read()) { }
                }

                return validationErrors;
            }
        }
    }
}