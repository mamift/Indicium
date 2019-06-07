using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Indicium.Tokens;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Indicium
{
    public class TypeGenerator
    {
        public static List<ClassDeclarationSyntax> GenerateTokenClasses(TokenProcessor tokenProcessor)
        {
            var tokenDefinitions = tokenProcessor.Tokeniser.TokenDefinitions;

            var classNames = tokenDefinitions.Select(td => td.Identifier).Distinct();

            var classDefinitions = classNames.Select(cn => {
                var cd = SyntaxFactory.ClassDeclaration(cn);

                var stringType = SyntaxFactory.ParseTypeName("string");
                var regexType = SyntaxFactory.ParseTypeName("System.Text.RegularExpressions.Regex");

                var publicKeywordSyntax = SyntaxFactory.Token(SyntaxKind.PublicKeyword);

                var idFieldDeclaration = SyntaxFactory.VariableDeclaration(stringType)
                                                      .AddVariables(SyntaxFactory.VariableDeclarator("Identifier"));
                var idField = SyntaxFactory.FieldDeclaration(idFieldDeclaration).AddModifiers(publicKeywordSyntax);

                var regexFieldDecl = SyntaxFactory.VariableDeclaration(regexType)
                                                  .AddVariables(SyntaxFactory.VariableDeclarator("Regex"));
                var regexField = SyntaxFactory.FieldDeclaration(regexFieldDecl).AddModifiers(publicKeywordSyntax);

                cd = cd.AddMembers(idField, regexField);

                return cd;
            });

            return classDefinitions.ToList();
        }

        /// <summary>
        /// Outputs, under a single <paramref name="namespace"/>, code for classes, that correspond to each given <see cref="TokenDefinition"/>.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public static CodeCompileUnit GenerateClassesForTokenDefinitions(IEnumerable<TokenDefinition> tokens,
            string @namespace = "Indicia")
        {
            var containingNamespace = new CodeNamespace(@namespace);
            containingNamespace.AddMinimumNamespaces();
            var tokenList = tokens.ToList();

            var ccu = new CodeCompileUnit();
            foreach (var tokenDef in tokenList) {
                var typeClass = GenerateClassForTokenDef(tokenDef);

                containingNamespace.Types.Add(typeClass);
            }

            ccu.Namespaces.Add(containingNamespace);
            return ccu;
        }

        /// <summary>
        /// Generates the class (fields and properties) for a given <see cref="TokenDefinition"/>.
        /// </summary>
        /// <param name="tokenDef"></param>
        /// <returns></returns>
        private static CodeTypeDeclaration GenerateClassForTokenDef(TokenDefinition tokenDef)
        {
            var tokenClass = new CodeTypeDeclaration($"{tokenDef.Identifier}Token") {
                TypeAttributes = TypeAttributes.Sealed | TypeAttributes.Public,
                BaseTypes = {new CodeTypeReference(new CodeTypeParameter(nameof(TokenBase)))}
            };

            // private member fields
            var idString = $"{nameof(tokenDef.Identifier)}String";
            var privateIdentifierField = new CodeMemberField {
                Attributes = MemberAttributes.Private | MemberAttributes.Const,
                Name = idString.Privatise(),
                Type = new CodeTypeReference(typeof(string)),
                InitExpression = new CodePrimitiveExpression(tokenDef.Identifier)
            };

            var privateRegexFieldInitialisation = new CodeObjectCreateExpression(nameof(Regex), 
                new CodePrimitiveExpression(tokenDef.Regex.ToString()));
            var privateRegexField = new CodeMemberField {
                Attributes = MemberAttributes.Private | MemberAttributes.Static,
                Name = nameof(Regex).Privatise(),
                Type = new CodeTypeReference(nameof(Regex)),
                InitExpression = privateRegexFieldInitialisation
            };

            // public properties
            var identifierPropertyReturnStatement = new CodeMethodReturnStatement(
                                                        new CodeFieldReferenceExpression(
                                                            new CodeVariableReferenceExpression(tokenClass.Name), privateIdentifierField.Name));
            var identifierProperty = new CodeMemberProperty {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = nameof(tokenDef.Identifier),
                HasGet = true,
                HasSet = false,
                ImplementationTypes = {new CodeTypeReference(typeof(string))},
                GetStatements = {identifierPropertyReturnStatement},
                Type = new CodeTypeReference(typeof(string)),
            };

            var regexPropertyReturnStatement = new CodeMethodReturnStatement(
                                                new CodeFieldReferenceExpression(
                                                    new CodeVariableReferenceExpression(tokenClass.Name), privateRegexField.Name));
            var regexProperty = new CodeMemberProperty {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = nameof(tokenDef.Regex),
                HasSet = false, HasGet = true,
                ImplementationTypes = {new CodeTypeReference(typeof(Regex))},
                GetStatements = {regexPropertyReturnStatement},
                Type = new CodeTypeReference(nameof(Regex))
            };

            var constructor = new CodeConstructor {
                Attributes = MemberAttributes.Public
            };

            tokenClass.Members.Add(privateIdentifierField);
            tokenClass.Members.Add(privateRegexField);

            tokenClass.Members.Add(identifierProperty);
            tokenClass.Members.Add(regexProperty);

            tokenClass.Members.Add(constructor);

            return tokenClass;
        }
    }
}