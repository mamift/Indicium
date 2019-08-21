using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Indicium.Schemas;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Indicium
{
    using SF = SyntaxFactory;

    public partial class TokenTypeGenerator
    {
        public static NamespaceDeclarationSyntax GenerateTokenClasses(TokenContext tokenProcessor,
            string nsName = "CustomTokens")
        {
            NamespaceDeclarationSyntax namespaceDec =
                SF.NamespaceDeclaration(SF.ParseName(nsName));
            UsingDirectiveSyntax sysUsingDirective =
                SF.UsingDirective(SF.ParseName(nameof(System)));
            UsingDirectiveSyntax regexUsingDirective = SF.UsingDirective(
                SF.ParseName(
                    $"{nameof(System)}.{nameof(System.Text)}.{nameof(System.Text.RegularExpressions)}"));

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
                SimpleBaseTypeSyntax tokenBaseType =
                    SF.SimpleBaseType(SF.ParseTypeName(nameof(TokenBase)));
                classDeclaration = classDeclaration.AddModifiers(publicKeyword).AddBaseListTypes(tokenBaseType);

                TypeSyntax stringType = SF.ParseTypeName(nameof(System.String));
                TypeSyntax regexType = SF.ParseTypeName(nameof(Regex));

                VariableDeclarationSyntax idFieldDeclaration = SF.VariableDeclaration(stringType)
                                                                 .AddVariables(
                                                                     SF.VariableDeclarator(
                                                                         "_identifier"));
                FieldDeclarationSyntax idField = SF
                                                 .FieldDeclaration(idFieldDeclaration)
                                                 .AddModifiers(privateKeyword, staticKeyword);

                ReturnStatementSyntax idPropGetterBlockStatement = SF.ReturnStatement(returnKeyword,
                    SF.IdentifierName("_identifier"), semicolonToken);
                BlockSyntax idPropGetterBlock = SF.Block(idPropGetterBlockStatement);
                AccessorDeclarationSyntax idPropGetter =
                    SF.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, idPropGetterBlock);
                PropertyDeclarationSyntax idProp = SF
                                                   .PropertyDeclaration(stringType, "Identifier")
                                                   .AddModifiers(publicKeyword).AddAccessorListAccessors(idPropGetter);

                VariableDeclarationSyntax regexFieldDecl = SF.VariableDeclaration(regexType)
                                                             .AddVariables(
                                                                 SF
                                                                     .VariableDeclarator("_regex"));
                FieldDeclarationSyntax regexField = SF
                                                    .FieldDeclaration(regexFieldDecl)
                                                    .AddModifiers(privateKeyword, staticKeyword);

                ReturnStatementSyntax regexPropGetterBlockStatement =
                    SF.ReturnStatement(returnKeyword, SF.IdentifierName("_regex"),
                        semicolonToken);
                BlockSyntax regexPropGetterBlock = SF.Block(regexPropGetterBlockStatement);
                AccessorDeclarationSyntax regexPropGetter =
                    SF.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, regexPropGetterBlock);
                PropertyDeclarationSyntax regexProp = SF
                                                      .PropertyDeclaration(regexType, "Regex")
                                                      .AddModifiers(publicKeyword)
                                                      .AddAccessorListAccessors(regexPropGetter);

                classDeclaration = classDeclaration.AddMembers(
                    idField.WithTrailingTrivia(SF.EndOfLine("\r\n")), regexField, idProp, regexProp);

                return classDeclaration;
            }).ToArray();

            namespaceDec = namespaceDec.AddMembers(classDefinitions);

            return namespaceDec;
        }
    }
}