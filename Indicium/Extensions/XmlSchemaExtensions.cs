﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        /// <summary>
        /// Wraps the regular <see cref="Enumerable.OfType{TResult}"/> method against an <see cref="XmlSchemaObjectCollection"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xsoc"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetOfType<T>(this XmlSchemaObjectCollection xsoc)
            where T: XmlSchemaObject
        {
            return xsoc.Cast<XmlSchemaObject>().OfType<T>();
        }

        public static XmlSchemaLengthFacet[] GetXmlSchemaLengthFacets(this XmlSchemaSimpleTypeRestriction str) => str.Facets.OfType<XmlSchemaLengthFacet>().ToArray();
        public static XmlSchemaMinLengthFacet[] GetXmlSchemaMinLengthFacets(this XmlSchemaSimpleTypeRestriction str) => str.Facets.OfType<XmlSchemaMinLengthFacet>().ToArray();
        public static XmlSchemaMaxLengthFacet[] GetXmlSchemaMaxLengthFacets(this XmlSchemaSimpleTypeRestriction str) => str.Facets.OfType<XmlSchemaMaxLengthFacet>().ToArray();
        public static XmlSchemaPatternFacet[] GetXmlSchemaPatternFacets(this XmlSchemaSimpleTypeRestriction str) => str.Facets.OfType<XmlSchemaPatternFacet>().ToArray();
        public static XmlSchemaEnumerationFacet[] GetXmlSchemaEnumerationFacets(this XmlSchemaSimpleTypeRestriction str) => str.Facets.OfType<XmlSchemaEnumerationFacet>().ToArray();
        public static XmlSchemaMaxInclusiveFacet[] GetXmlSchemaMaxInclusiveFacets(this XmlSchemaSimpleTypeRestriction str) => str.Facets.OfType<XmlSchemaMaxInclusiveFacet>().ToArray();
        public static XmlSchemaMaxExclusiveFacet[] GetXmlSchemaMaxExclusiveFacets(this XmlSchemaSimpleTypeRestriction str) => str.Facets.OfType<XmlSchemaMaxExclusiveFacet>().ToArray();
        public static XmlSchemaMinInclusiveFacet[] GetXmlSchemaMinInclusiveFacets(this XmlSchemaSimpleTypeRestriction str) => str.Facets.OfType<XmlSchemaMinInclusiveFacet>().ToArray();
        public static XmlSchemaMinExclusiveFacet[] GetXmlSchemaMinExclusiveFacets(this XmlSchemaSimpleTypeRestriction str) => str.Facets.OfType<XmlSchemaMinExclusiveFacet>().ToArray();
        public static XmlSchemaFractionDigitsFacet[] GetXmlSchemaFractionDigitsFacets(this XmlSchemaSimpleTypeRestriction str) => str.Facets.OfType<XmlSchemaFractionDigitsFacet>().ToArray();
        public static XmlSchemaTotalDigitsFacet[] GetXmlSchemaTotalDigitsFacets(this XmlSchemaSimpleTypeRestriction str) => str.Facets.OfType<XmlSchemaTotalDigitsFacet>().ToArray();
        public static XmlSchemaWhiteSpaceFacet[] GetXmlSchemaWhiteSpaceFacets(this XmlSchemaSimpleTypeRestriction str) => str.Facets.OfType<XmlSchemaWhiteSpaceFacet>().ToArray();

        public static Regex ToRegex(this XmlSchemaPatternFacet patternFacet, RegexOptions regexOptions = RegexOptions.Compiled)
        {
            return new Regex(patternFacet.Value, regexOptions);
        }

        public static IEnumerable<XmlSchemaSimpleType> GetGlobalSimpleTypes(this XmlSchemaSet xmlSchemaSet)
        {
            var globalTypes = xmlSchemaSet.GlobalXsdTypes();
            var simpleTypes = globalTypes.OfType<XmlSchemaSimpleType>();
            return simpleTypes;
        }
    }
}