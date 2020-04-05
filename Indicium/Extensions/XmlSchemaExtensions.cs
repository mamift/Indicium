using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Xml.Fxt;
using Xml.Schema.Linq.Extensions;

namespace Indicium.Extensions
{
    public static class XmlSchemaExtensions
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

        /// <summary>
        /// Converts the current <see cref="XmlSchemaObjectTable"/> to an
        /// equivalent <see cref="Dictionary{TKey,TValue}"/> of its contents.
        /// </summary>
        /// <typeparam name="TXmlSchemaObject"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static Dictionary<XmlQualifiedName, TXmlSchemaObject> ToDictionary<TXmlSchemaObject>(this XmlSchemaObjectTable table)
            where TXmlSchemaObject: XmlSchemaObject
        {
            var names = table.Names.Cast<XmlQualifiedName>();
            var values = table.Values.Cast<TXmlSchemaObject>().ToList();

            var dictionary = new Dictionary<XmlQualifiedName, TXmlSchemaObject>();

            var count = 0;
            foreach (var name in names) {
                dictionary[name] = values.ElementAt(count);
                count++;
            }

            return dictionary;
        }
    }
}