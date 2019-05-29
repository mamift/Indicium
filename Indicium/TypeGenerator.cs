using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
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

        public static CodeCompileUnit GenerateCodeUnits(IEnumerable<TokenDefinition> tokens)
        {
            var containingNamespace = new CodeNamespace("Indicia");
            containingNamespace.AddMinimumNamespaces();
            var tokenList = tokens.ToList();

            var ccu = new CodeCompileUnit();
            foreach (var tokenDef in tokenList) {
                var tClass = new CodeTypeDeclaration(tokenDef.Identifier) {
                    Attributes = MemberAttributes.Public
                };

                var identiferProperty = new CodeMemberProperty {
                    Attributes = MemberAttributes.Const | MemberAttributes.Public | MemberAttributes.Static,
                    GetStatements = { new CodeExpressionStatement(new CodeExpression()) }
                };

                var identifierField = new CodeMemberField {
                    Attributes = MemberAttributes.Const | MemberAttributes.Public | MemberAttributes.Static,
                    Name = nameof(tokenDef.Identifier),
                    Type = new CodeTypeReference(typeof(string)),
                    InitExpression = new CodePrimitiveExpression(tokenDef.Regex.ToString())
                };

                var regexField = new CodeMemberField {
                    Attributes = MemberAttributes.Public,
                    Name = $"{nameof(Regex)}Matcher",
                    Type = new CodeTypeReference(typeof(Regex)),
                    InitExpression = new CodeObjectCreateExpression(typeof(Regex), new CodePrimitiveExpression(tokenDef.Regex.ToString()))
                };

                var constructor = new CodeConstructor {
                    Attributes = MemberAttributes.Public
                };

                tClass.Members.Add(identifierField);
                tClass.Members.Add(constructor);
                tClass.Members.Add(regexField);

                containingNamespace.Types.Add(tClass);
            }

            ccu.Namespaces.Add(containingNamespace);
            return ccu;
        }
    }
}