using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Indicium
{
    public static class CodeDomExtensionMethods
    {
        /// <summary>
        /// Adds a get property that returns a default value.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeName"></param>
        /// <param name="propertyName"></param>
        /// <param name="attrs"></param>
        /// <returns></returns>
        public static CodeMemberProperty AddDefaultGetter(this CodeTypeDeclaration type, string typeName,
            string propertyName, MemberAttributes attrs = MemberAttributes.Public)
        {
            if (typeName == null) throw new ArgumentNullException(nameof(typeName));

            var tokenBaseType = new CodeTypeReference(new CodeTypeParameter(typeName));

            if (typeName == "string" || typeName == "System.String")
                tokenBaseType = new CodeTypeReference(typeof(string));

            var defaultProperty = new CodeMemberProperty {
                Type = tokenBaseType,
                Name = propertyName,
                Attributes = attrs,
                HasGet = true
            };

            if (!attrs.HasFlag(MemberAttributes.Abstract)) {
                defaultProperty.GetStatements.Add(new CodeMethodReturnStatement {
                    Expression = new CodeDefaultValueExpression {
                        Type = tokenBaseType
                    }
                });
            }

            type.Members.Add(defaultProperty);

            return defaultProperty;
        }

        /// <summary>
        /// Generates C# code from the current <see cref="CodeCompileUnit"/>.
        /// </summary>
        /// <param name="ccu"></param>
        /// <param name="opts">Optional <see cref="CodeGeneratorOptions"/></param>
        /// <returns></returns>
        public static string ToCSharpString(this CodeCompileUnit ccu, CodeGeneratorOptions opts = null)
        {
            var cdProvider = CodeDomProvider.CreateProvider("CSharp");
            var cdOptions = opts ?? new CodeGeneratorOptions {
                BlankLinesBetweenMembers = true,
                BracingStyle = "Block",
                VerbatimOrder = true,
                IndentString = "\t",
                ElseOnClosing = true
            };
            var codeDomSb = new StringBuilder();
            var sw = new StringWriter(codeDomSb);
            cdProvider.GenerateCodeFromCompileUnit(ccu, sw, cdOptions);
            return sw.ToString();
        }

        /// <summary>
        /// Generates C# code from the current <see cref="CodeCompileUnit"/>, and returns it as <see cref="SourceText"/>.
        /// </summary>
        /// <param name="ccu"></param>
        /// <param name="opts">Optional <see cref="CodeGeneratorOptions"/></param>
        /// <returns></returns>
        public static SourceText ToSourceText(this CodeCompileUnit ccu, CodeGeneratorOptions opts = null)
        {
            var csharpCode = ccu.ToCSharpString(opts);
            return SourceText.From(csharpCode);
        }

        /// <summary>
        /// Generates C# code from the current <see cref="CodeCompileUnit"/>, and returns it as <see cref="SourceText"/>.
        /// </summary>
        /// <param name="ccu"></param>
        /// <param name="opts">Optional <see cref="CodeGeneratorOptions"/></param>
        /// <param name="parseOpts">Optional <see cref="CSharpParseOptions"/>, defaults to <see cref="CSharpParseOptions.Default"/> if null.</param>
        /// <returns></returns>
        public static SyntaxTree ToSyntaxTree(this CodeCompileUnit ccu, CodeGeneratorOptions opts = null, 
            CSharpParseOptions parseOpts = null)
        {
            var csharpCode = ccu.ToCSharpString(opts);
            var sourceText = SourceText.From(csharpCode);

            var parseOptions = parseOpts ?? CSharpParseOptions.Default;

            return CSharpSyntaxTree.ParseText(sourceText, parseOptions);
        }
    }
}