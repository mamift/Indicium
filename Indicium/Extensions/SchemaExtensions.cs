using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Schema;
using Xml.Schema.Linq.Extensions;

namespace Indicium.Extensions
{
    public static class SchemaExtensions
    {
        /// <summary>
        /// Resolves the <![CDATA[<xs:include />]]> elements, using a given <see cref="baseDir"/>.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="baseDir"></param>
        /// <returns></returns>
        public static XmlSchemaSet ResolveIncludes(this XmlSchema schema, string baseDir = null)
        {
            if (baseDir.IsEmpty()) baseDir = Environment.CurrentDirectory;

            var includes = schema.Includes.Cast<XmlSchemaInclude>();
            var schemaSet = new XmlSchemaSet();
            schemaSet.Add(schema);
            foreach (var include in includes) {
                var fileStream = File.OpenRead(Path.Combine(baseDir, include.SchemaLocation));
                var includedSchema = XmlSchema.Read(fileStream, (sender, args) => {
                    if (args.Exception != null) throw args.Exception;
                });
                include.Schema = includedSchema;

                schemaSet.Add(includedSchema);
            }

            schemaSet.CompilationSettings = new XmlSchemaCompilationSettings() {
                EnableUpaCheck = true
            };

            schemaSet.Compile();
            
            return schemaSet;
        }
    }
}