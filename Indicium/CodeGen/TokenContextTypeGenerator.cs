using System;
using System.Collections.Generic;
using Indicium.Schemas;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xml.Schema.Linq.Extensions;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Indicium.CodeGen
{
    public static class TokenContextTypeGenerator
    {
        /// <summary>
        /// Generates the code for a custom namespace containing everything necessary for 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="namespaceName"></param>
        /// <returns></returns>
        public static NamespaceDeclarationSyntax GenerateTokeniserCode(TokenContext context, string namespaceName)
        {
            var namespaceDecl = SF.NamespaceDeclaration(SF.ParseName(namespaceName));

            namespaceDecl = namespaceDecl.AddUsings(
                SF.UsingDirective(SF.ParseName($"{nameof(System)}")),
                SF.UsingDirective(SF.ParseName($"{nameof(System)}.{nameof(System.Collections)}.{nameof(System.Collections.Generic)}")),
                SF.UsingDirective(SF.ParseName($"{nameof(XObjects)}")),
                SF.UsingDirective(SF.ParseName($"{nameof(Indicium)}")),
                SF.UsingDirective(SF.ParseName($"{nameof(Indicium)}.{nameof(Indicium.Schemas)}")),
                SF.UsingDirective(SF.ParseName($"{nameof(Indicium)}.{nameof(Indicium.CodeGen)}"))
            );

            foreach (var token in context.Token) {
                var tokenType = GenerateTokenClassDeclaration(token);

                namespaceDecl = namespaceDecl.AddMembers(tokenType);
            }

            var tokeniserTypeName = context.ClassName.IsEmpty() ? "Tokeniser" : context.ClassName;
            namespaceDecl = namespaceDecl.AddMembers(GenerateStaticTokeniserClassDeclaration(context, tokeniserTypeName));

            return namespaceDecl;
        }

        /// <summary>
        /// Create a static class that can be used to access the Tokeniser instance.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tokeniserTypeName">Defaults to "Tokeniser"</param>
        /// <returns></returns>
        public static ClassDeclarationSyntax GenerateStaticTokeniserClassDeclaration(TokenContext context, string tokeniserTypeName = "Tokeniser")
        {
            var publicStatic = new[]{ SF.Token(SyntaxKind.PublicKeyword), SF.Token(SyntaxKind.StaticKeyword) };

            const string tokenContextTypeName = nameof(TokenContext);

            var lineDelimiterAssignment = SF.AssignmentExpression(
                                          SyntaxKind.SimpleAssignmentExpression,
                                          SF.IdentifierName(nameof(TokenContext.LineDelimiter)),
                                          SF.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                SF.IdentifierName($"{nameof(System)}.{nameof(Environment)}"),
                                                SF.IdentifierName(nameof(Environment.NewLine)))
                                            .WithOperatorToken(SF.Token(SyntaxKind.DotToken)))
                                            .WithOperatorToken(SF.Token(SyntaxKind.EqualsToken));

            var whitespaceCharacters = !string.IsNullOrEmpty(context.WhitespaceCharacters) ? context.WhitespaceCharacters : " \t";
            var whiteSpaceCharsAssignment = SF.AssignmentExpression(
                                                SyntaxKind.SimpleAssignmentExpression,
                                                SF.IdentifierName(nameof(TokenContext.WhitespaceCharacters)),
                                                SF.LiteralExpression(SyntaxKind.StringLiteralExpression, SF.Literal(whitespaceCharacters)))
                                            .WithOperatorToken(SF.Token(SyntaxKind.EqualsToken));

            var defaultInstancesOfTokensList = new List<SyntaxNodeOrToken>(context.Token.Count);

            for (var i = 0; i < context.Token.Count; i++) {
                var token = context.Token[i];
                var last = i == context.Token.Count - 1;
                var memberAccessExpr = SF.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SF.IdentifierName(token.GetIdForCodeGen()), 
                    SF.IdentifierName("Default"))
                .WithOperatorToken(SF.Token(SyntaxKind.DotToken));

                defaultInstancesOfTokensList.Add(memberAccessExpr);
                if (!last) defaultInstancesOfTokensList.Add(SF.Token(SyntaxKind.CommaToken));
            }

            var defaultInstanceGetProperty = SF.PropertyDeclaration(
                                                  SF.IdentifierName(tokenContextTypeName),
                                                  SF.Identifier("Instance"))
                                              .WithModifiers(SF.TokenList(publicStatic))
                                              .WithAccessorList(
                                                  SF.AccessorList(
                                                        SF.SingletonList(
                                                            SF.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                                              .WithKeyword(SF.Token(SyntaxKind.GetKeyword))
                                                              .WithSemicolonToken(SF.Token(SyntaxKind.SemicolonToken))))
                                                    .WithOpenBraceToken(SF.Token(SyntaxKind.OpenBraceToken))
                                                    .WithCloseBraceToken(SF.Token(SyntaxKind.CloseBraceToken)));
            
            var constructorBodyBlock = SF.Block(
                SF.SingletonList<StatementSyntax>(
                    SF.ExpressionStatement(
                          SF.AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                SF.IdentifierName("Instance"),
                                SF.ObjectCreationExpression(SF.IdentifierName(tokenContextTypeName))
                                  .WithNewKeyword(SF.Token(SyntaxKind.NewKeyword))
                                  .WithArgumentList(
                                      SF.ArgumentList()
                                        .WithOpenParenToken(SF.Token(SyntaxKind.OpenParenToken))
                                        .WithCloseParenToken(SF.Token(SyntaxKind.CloseParenToken)))
                                  .WithInitializer(
                                      SF.InitializerExpression(
                                            SyntaxKind.ObjectInitializerExpression,
                                            SF.SeparatedList<ExpressionSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    SF.AssignmentExpression(
                                                          SyntaxKind.SimpleAssignmentExpression,
                                                          SF.IdentifierName(nameof(TokenContext.Token)),
                                                          SF.ObjectCreationExpression(
                                                                SF.GenericName(SF.Identifier("List"))
                                                                  .WithTypeArgumentList(
                                                                      SF.TypeArgumentList(SF.SingletonSeparatedList<TypeSyntax>(SF.IdentifierName("Token")))
                                                                        .WithLessThanToken(SF.Token(SyntaxKind.LessThanToken))
                                                                        .WithGreaterThanToken(SF.Token(SyntaxKind.GreaterThanToken))))
                                                            .WithNewKeyword(SF.Token(SyntaxKind.NewKeyword))
                                                            .WithArgumentList(
                                                                SF.ArgumentList()
                                                                  .WithOpenParenToken(SF.Token(SyntaxKind.OpenParenToken))
                                                                  .WithCloseParenToken(SF.Token(SyntaxKind.CloseParenToken)))
                                                            .WithInitializer(
                                                                SF.InitializerExpression(
                                                                      SyntaxKind.CollectionInitializerExpression,
                                                                      SF.SeparatedList<ExpressionSyntax>(defaultInstancesOfTokensList))
                                                                  .WithOpenBraceToken(SF.Token(SyntaxKind.OpenBraceToken))
                                                                  .WithCloseBraceToken(SF.Token(SyntaxKind.CloseBraceToken))))
                                                      .WithOperatorToken(SF.Token(SyntaxKind.EqualsToken)),
                                                    SF.Token(SyntaxKind.CommaToken),
                                                    lineDelimiterAssignment,
                                                    SF.Token(SyntaxKind.CommaToken),
                                                    whiteSpaceCharsAssignment,
                                                    SF.Token(SyntaxKind.CommaToken)}))
                                        .WithOpenBraceToken(SF.Token(SyntaxKind.OpenBraceToken))
                                        .WithCloseBraceToken(SF.Token(SyntaxKind.CloseBraceToken))))
                            .WithOperatorToken(SF.Token(SyntaxKind.EqualsToken)))
                      .WithSemicolonToken(SF.Token(SyntaxKind.SemicolonToken))));

            var staticClassDeclaration = SF.ClassDeclaration(tokeniserTypeName)
                .WithModifiers(SF.TokenList(publicStatic))
                .WithKeyword(SF.Token(SyntaxKind.ClassKeyword))
                .WithOpenBraceToken(SF.Token(SyntaxKind.OpenBraceToken))
                .WithMembers(
                    SF.List(
                        new MemberDeclarationSyntax[] {
                            defaultInstanceGetProperty,
                            SF.ConstructorDeclaration(SF.Identifier(tokeniserTypeName))
                            .WithModifiers(SF.TokenList(SF.Token(SyntaxKind.StaticKeyword)))
                            .WithParameterList(
                                SF.ParameterList()
                                .WithOpenParenToken(SF.Token(SyntaxKind.OpenParenToken))
                                .WithCloseParenToken(SF.Token(SyntaxKind.CloseParenToken)))
                            .WithBody(
                                constructorBodyBlock
                                .WithOpenBraceToken(SF.Token(SyntaxKind.OpenBraceToken))
                                .WithCloseBraceToken(SF.Token(SyntaxKind.CloseBraceToken)))}))
                .WithCloseBraceToken(SF.Token(SyntaxKind.CloseBraceToken))
                .NormalizeWhitespace();

            return staticClassDeclaration;
        }

        /// <summary>
        /// Creates a class declaration for a given <paramref name="token"/>.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static ClassDeclarationSyntax GenerateTokenClassDeclaration(Token token)
        {
            if (token.Id == nameof(Token)) throw new Exception("A token cannot have the name 'Token'!");

            var className = token.GetIdForCodeGen();

            var publicModifier = SF.Token(SyntaxKind.PublicKeyword);
            var publicSealedModifiers = SF.TokenList(
                new[] {
                    publicModifier,
                    SF.Token(SyntaxKind.SealedKeyword)
                });

            var baseTypeList = SF.BaseList(
                SF.SingletonSeparatedList<BaseTypeSyntax>(
                    SF.SimpleBaseType(SF.IdentifierName(nameof(Token)))
                ));

            var idValueAssignment = SF.ExpressionStatement(
                SF.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SF.IdentifierName(nameof(Token.Id)),
                    SF.InvocationExpression(SF.IdentifierName("nameof"))
                      .WithArgumentList(
                          SF.ArgumentList(
                              SF.SingletonSeparatedList(
                                  SF.Argument(SF.IdentifierName(className)))))));

            var regexLiteralAssignment = SF.ExpressionStatement(
                SF.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SF.IdentifierName(nameof(Token.TypedValue)),
                    SF.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        SF.Literal(token.TypedValue))));
            
            var constructorBodyBlock = SF.Block(
                idValueAssignment,
                regexLiteralAssignment
            );

            if (!string.IsNullOrWhiteSpace(token.Description)) {
                var tokenDescriptionAssignment = SF.ExpressionStatement(
                    SF.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SF.IdentifierName(nameof(Token.Description)),
                        SF.LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            SF.Literal(token.Description))));

                constructorBodyBlock = constructorBodyBlock.AddStatements(tokenDescriptionAssignment);
            }

            var constructor = SF.ConstructorDeclaration(SF.Identifier(className))
                                                 .WithModifiers(SF.TokenList(publicModifier))
                                                 .WithBody(constructorBodyBlock);

            var publicStaticModifier = SF.TokenList(
                new[] {
                    publicModifier,
                    SF.Token(SyntaxKind.StaticKeyword)
                }
            );

            var defaultStaticMember = SF.FieldDeclaration(
                                                SF.VariableDeclaration(SF.IdentifierName(className))
                                                  .WithVariables(
                                                      SF.SingletonSeparatedList(
                                                          SF.VariableDeclarator(SF.Identifier("Default"))
                                                            .WithInitializer(
                                                                SF.EqualsValueClause(
                                                                    SF.ObjectCreationExpression(
                                                                        SF.IdentifierName(className)).WithArgumentList(SF.ArgumentList()))))))
                                            .WithModifiers(publicStaticModifier);

            var classDeclaration = SF.ClassDeclaration(className)
                                     .WithModifiers(publicSealedModifiers)
                                     .WithBaseList(baseTypeList)
                                     .WithMembers(
                                         SF.List(
                                             new MemberDeclarationSyntax[] {
                                                 constructor,
                                                 defaultStaticMember
                                             }));

            return classDeclaration;
        }
    }
}