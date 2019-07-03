using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Indicium.Schemas;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Indicium
{
    using SF = SyntaxFactory;
    
    public class TokenTypeGenerator
    {
        public static NamespaceDeclarationSyntax GenerateTokenClasses(TokenContext tokenProcessor, string nsName = "CustomTokens")
        {
            NamespaceDeclarationSyntax namespaceDec = SF.NamespaceDeclaration(SF.ParseName(nsName));
            UsingDirectiveSyntax sysUsingDirective = SF.UsingDirective(SF.ParseName(nameof(System)));
            UsingDirectiveSyntax regexUsingDirective = SF.UsingDirective(SF.ParseName($"{nameof(System)}.{nameof(System.Text)}.{nameof(System.Text.RegularExpressions)}"));

            namespaceDec = namespaceDec.AddUsings(sysUsingDirective, regexUsingDirective);

            IEnumerable<string> classNames = tokenProcessor.Token.Select(td => td.Id).Distinct();

            SyntaxToken publicKeyword = SF.Token(SyntaxKind.PublicKeyword);
            SyntaxToken privateKeyword = SF.Token(SyntaxKind.PrivateKeyword);
            SyntaxToken staticKeyword = SF.Token(SyntaxKind.StaticKeyword);
            SyntaxToken readonlyKeyword = SF.Token(SyntaxKind.ReadOnlyKeyword);

            SyntaxToken returnKeyword = SF.Token(SyntaxKind.ReturnKeyword);
            SyntaxToken semicolonToken = SF.Token(SyntaxKind.SemicolonToken);
            ClassDeclarationSyntax[] classDefinitions = classNames.Select(className => {
                SyntaxToken classId = SF.Identifier(className);

                ClassDeclarationSyntax classDeclaration = SF.ClassDeclaration(classId);
                SimpleBaseTypeSyntax tokenBaseType = SF.SimpleBaseType(SF.ParseTypeName(nameof(TokenBase)));
                classDeclaration = classDeclaration.AddModifiers(publicKeyword).AddBaseListTypes(tokenBaseType);

                TypeSyntax stringType = SF.ParseTypeName(nameof(System.String));
                TypeSyntax regexType = SF.ParseTypeName(nameof(Regex));

                VariableDeclarationSyntax idFieldDeclaration = SF.VariableDeclaration(stringType)
                                                      .AddVariables(SF.VariableDeclarator("_identifier"));
                FieldDeclarationSyntax idField = SF.FieldDeclaration(idFieldDeclaration).AddModifiers(privateKeyword, staticKeyword);

                ReturnStatementSyntax idPropGetterBlockStatement = SF.ReturnStatement(returnKeyword, SF.IdentifierName("_identifier"), semicolonToken);
                BlockSyntax idPropGetterBlock = SF.Block(idPropGetterBlockStatement);
                AccessorDeclarationSyntax idPropGetter = SF.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, idPropGetterBlock);
                PropertyDeclarationSyntax idProp = SF.PropertyDeclaration(stringType, "Identifier").AddModifiers(publicKeyword).AddAccessorListAccessors(idPropGetter);

                VariableDeclarationSyntax regexFieldDecl = SF.VariableDeclaration(regexType)
                                                  .AddVariables(SF.VariableDeclarator("_regex"));
                FieldDeclarationSyntax regexField = SF.FieldDeclaration(regexFieldDecl).AddModifiers(privateKeyword, staticKeyword);

                ReturnStatementSyntax regexPropGetterBlockStatement = SF.ReturnStatement(returnKeyword, SF.IdentifierName("_regex"), semicolonToken);
                BlockSyntax regexPropGetterBlock = SF.Block(regexPropGetterBlockStatement);
                AccessorDeclarationSyntax regexPropGetter = SF.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, regexPropGetterBlock);
                PropertyDeclarationSyntax regexProp = SF.PropertyDeclaration(regexType, "Regex").AddModifiers(publicKeyword).AddAccessorListAccessors(regexPropGetter);

                classDeclaration = classDeclaration.AddMembers(idField.WithTrailingTrivia(SF.EndOfLine("\r\n")), regexField, idProp, regexProp);

                return classDeclaration;
            }).ToArray();

            namespaceDec = namespaceDec.AddMembers(classDefinitions);

            return namespaceDec;
        }

        /// <summary>
        /// Outputs, under a single <paramref name="namespace"/>, code for classes, that correspond to each given <see cref="TokenDefinition"/>.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public static CodeCompileUnit GenerateClassesForTokenDefinitions(IEnumerable<Token> tokens,
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
        /// Generates the class (fields and properties) for a given <paramref name="tokenDef"/>.
        /// </summary>
        /// <param name="tokenDef"></param>
        /// <returns></returns>
        private static CodeTypeDeclaration GenerateClassForTokenDef(Token tokenDef)
        {
            var tokenClass = new CodeTypeDeclaration($"{tokenDef.Id}Token") {
                TypeAttributes = TypeAttributes.Sealed | TypeAttributes.Public,
                BaseTypes = {new CodeTypeReference(new CodeTypeParameter(nameof(TokenBase)))}
            };

            // private member fields
            var idString = $"{nameof(tokenDef.Id)}String";
            var privateIdentifierField = new CodeMemberField {
                Attributes = MemberAttributes.Private | MemberAttributes.Const,
                Name = idString.Privatise(),
                Type = new CodeTypeReference(typeof(string)),
                InitExpression = new CodePrimitiveExpression(tokenDef.Id)
            };

            var privateRegexFieldInitialisation = new CodeObjectCreateExpression(nameof(Regex), 
                new CodePrimitiveExpression(tokenDef.GetMatcher().ToString()));
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
                Name = nameof(tokenDef.Id),
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
                Name = nameof(Regex),
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