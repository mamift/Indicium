using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Indicium
{
    using SF = SyntaxFactory;

    public static partial class RoslynExtensionMethods
    {
        public static ReturnStatementSyntax GenerateReturnNewLexemeStatement()
        {
            var lexemeType = SF.ParseTypeName("Lexeme");

            const SyntaxKind simpleMemberAccessExpression = SyntaxKind.SimpleMemberAccessExpression;

            const string idMemberName = "Id";
            const string undefinedMemberName = "Undefined";
            const string valueMemberName = "Value";
            const string inputStringVarName = "input";
            const string indexVarName = "index";
            const string toStringMethodName = "ToString";

            const string cultureInfoEnumName = "CultureInfo";
            const string invariantCultureEnumMemberName = "InvariantCulture";
            const string lineIndexMemberName = "LineIndex";
            const string matchLengthVarName = "matchLength";

            var returnNewLexemeStatement = SF.ReturnStatement(
                SF.ObjectCreationExpression(
                    lexemeType)
                .WithInitializer(
                    SF.InitializerExpression(
                        SyntaxKind.ObjectInitializerExpression,
                        SF.SeparatedList<ExpressionSyntax>(
                            new SyntaxNodeOrToken[]{
                                SF.AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    SF.IdentifierName(idMemberName),
                                    SF.MemberAccessExpression(
                                        simpleMemberAccessExpression,
                                        SF.MemberAccessExpression(
                                            simpleMemberAccessExpression,
                                            lexemeType,
                                            SF.IdentifierName(undefinedMemberName)),
                                        SF.IdentifierName(idMemberName))),
                                SF.Token(SyntaxKind.CommaToken),
                                SF.AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    SF.IdentifierName(valueMemberName),
                                    SF.InvocationExpression(
                                        SF.MemberAccessExpression(
                                            simpleMemberAccessExpression,
                                            SF.ElementAccessExpression(
                                                SF.IdentifierName(inputStringVarName))
                                            .WithArgumentList(
                                                SF.BracketedArgumentList(
                                                    SF.SingletonSeparatedList(
                                                        SF.Argument(
                                                            SF.BinaryExpression(
                                                                SyntaxKind.SubtractExpression,
                                                                SF.IdentifierName(indexVarName),
                                                                SF.LiteralExpression(
                                                                    SyntaxKind.NumericLiteralExpression,
                                                                    SF.Literal(1))))))),
                                            SF.IdentifierName(toStringMethodName)))
                                    .WithArgumentList(
                                        SF.ArgumentList(
                                            SF.SingletonSeparatedList(
                                                SF.Argument(
                                                    SF.MemberAccessExpression(
                                                        simpleMemberAccessExpression,
                                                        SF.IdentifierName(cultureInfoEnumName),
                                                        SF.IdentifierName(invariantCultureEnumMemberName))))))),
                                SF.Token(SyntaxKind.CommaToken),
                                SF.AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    SF.IdentifierName(lineIndexMemberName),
                                    SF.BinaryExpression(
                                        SyntaxKind.SubtractExpression,
                                        SF.IdentifierName(indexVarName),
                                        SF.IdentifierName(matchLengthVarName)))}))));

            return returnNewLexemeStatement;
        }

        public static ForEachStatementSyntax GenerateForEachStatementCode()
        {
            TypeSyntax varType = SF.ParseTypeName("var");

            const string tokenDefVarName = "def";
            const string regexVarName = "regex";
            const string matchVarName = "match";
            const string inputStringVarName = "input";
            const string indexVarName = "index";
            const string tokenListVarName = "tokenList";
            const string lengthPropertyName = "Length";
            const string matchLengthVarName = "matchLength";
            const string idMemberName = "Id";
            const string getMatcherMethodName = "GetMatcher";
            const string matchMethodName = "Match";
            const string lexemeTypeName = "Lexeme";
            const string valueMemberNameOnLexemeType = "Value";
            const string lineIndexMemberNameOnLexemeType = "LineIndex";
            const string successMemberName = "Success";
            const string indexMemberName = "Index";

            const SyntaxKind simpleMemberAccessExpression = SyntaxKind.SimpleMemberAccessExpression;
            const SyntaxKind simpleAssignmentExpression = SyntaxKind.SimpleAssignmentExpression;

            var regexVarNameIdentifier = SF.Identifier(regexVarName);

            var tokenDefVarIdentifierName = SF.IdentifierName(tokenDefVarName);
            var getMatcherMethodIdentifierName = SF.IdentifierName(getMatcherMethodName);

            LocalDeclarationStatementSyntax varRegexVariableDeclarationStmt = SF.LocalDeclarationStatement(
                SF.VariableDeclaration(varType)
                  .WithVariables(
                      SF.SingletonSeparatedList(
                          SF.VariableDeclarator(regexVarNameIdentifier)
                            .WithInitializer(
                                SF.EqualsValueClause(
                                    SF.InvocationExpression(
                                        SF.MemberAccessExpression(
                                            simpleMemberAccessExpression,
                                            tokenDefVarIdentifierName,
                                            getMatcherMethodIdentifierName)))))));

            var matchVarIdentifier = SF.Identifier(matchVarName);
            var regexVarIdentifierName = SF.IdentifierName(regexVarName);
            var matchMethodNameIdName = SF.IdentifierName(matchMethodName);

            var inputStringIdName = SF.IdentifierName(inputStringVarName);
            var indexVarIdName = SF.IdentifierName(indexVarName);

            LocalDeclarationStatementSyntax varMatchVariableDeclarationStmt = SF.LocalDeclarationStatement(
                SF.VariableDeclaration(varType)
                  .WithVariables(
                      SF.SingletonSeparatedList(
                          SF.VariableDeclarator(matchVarIdentifier)
                            .WithInitializer(
                                SF.EqualsValueClause(
                                    SF.InvocationExpression(
                                          SF.MemberAccessExpression(
                                              simpleMemberAccessExpression,
                                              regexVarIdentifierName,
                                              matchMethodNameIdName))
                                      .WithArgumentList(
                                          SF.ArgumentList(
                                              SF.SeparatedList<ArgumentSyntax>(
                                                  new SyntaxNodeOrToken[]{
                                                      SF.Argument(inputStringIdName),
                                                      SF.Token(SyntaxKind.CommaToken),
                                                      SF.Argument(indexVarIdName)}))))))));

            var matchVarIdName = SF.IdentifierName(matchVarName);
            var successMemberIdName = SF.IdentifierName(successMemberName);
            var indexMemberIdName = SF.IdentifierName(indexMemberName);

            IfStatementSyntax ifMatchSuccessNeTrueOrMatchIndexNeIndexStmt = SF.IfStatement(
                SF.BinaryExpression(
                    SyntaxKind.LogicalOrExpression,
                    SF.PrefixUnaryExpression(
                        SyntaxKind.LogicalNotExpression,
                        SF.MemberAccessExpression(
                            simpleMemberAccessExpression,
                            matchVarIdName,
                            successMemberIdName)),
                    SF.BinaryExpression(
                        SyntaxKind.NotEqualsExpression,
                        SF.MemberAccessExpression(
                            simpleMemberAccessExpression,
                            matchVarIdName,
                            indexMemberIdName),
                        indexVarIdName)),
                SF.ContinueStatement());

            var lengthPropertyIdName = SF.IdentifierName(lengthPropertyName);
            IfStatementSyntax ifMatchLengthEq0Stmt = SF.IfStatement(
                SF.BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    SF.MemberAccessExpression(
                        simpleMemberAccessExpression,
                        matchVarIdName,
                        lengthPropertyIdName),
                    SF.LiteralExpression(
                        SyntaxKind.NumericLiteralExpression,
                        SF.Literal(0))),
                SF.ContinueStatement());

            ExpressionStatementSyntax indexAddAssignmentMatchLengthStmt = SF.ExpressionStatement(
                SF.AssignmentExpression(
                    SyntaxKind.AddAssignmentExpression,
                    indexVarIdName,
                    SF.MemberAccessExpression(
                        simpleMemberAccessExpression,
                        matchVarIdName,
                        lengthPropertyIdName)));

            var matchLengthVarIdName = SF.IdentifierName(matchLengthVarName);
            ExpressionStatementSyntax assignMatchLengthStmt = SF.ExpressionStatement(
                SF.AssignmentExpression(
                    simpleAssignmentExpression,
                    matchLengthVarIdName,
                    SF.MemberAccessExpression(
                        simpleMemberAccessExpression,
                        matchVarIdName,
                        lengthPropertyIdName)));

            var idMemberIdName = SF.IdentifierName(idMemberName);
            var valueMemberOnLexemeTypeIdName = SF.IdentifierName(valueMemberNameOnLexemeType);
            var lineIndexMemberOnLexemeTypeIdName = SF.IdentifierName(lineIndexMemberNameOnLexemeType);

            ReturnStatementSyntax returnNewLexemeObjectStmt = SF.ReturnStatement(
                SF.ObjectCreationExpression(
                      SF.ParseTypeName(lexemeTypeName))
                  .WithInitializer(
                      SF.InitializerExpression(
                          SyntaxKind.ObjectInitializerExpression,
                          SF.SeparatedList<ExpressionSyntax>(
                              new SyntaxNodeOrToken[]{
                                  SF.AssignmentExpression(
                                      simpleAssignmentExpression,
                                      idMemberIdName,
                                      SF.MemberAccessExpression(
                                          simpleMemberAccessExpression,
                                          tokenDefVarIdentifierName,
                                          idMemberIdName)),
                                  SF.Token(SyntaxKind.CommaToken),
                                  SF.AssignmentExpression(
                                      simpleAssignmentExpression,
                                      valueMemberOnLexemeTypeIdName,
                                      SF.MemberAccessExpression(
                                          simpleMemberAccessExpression,
                                          matchVarIdName,
                                          valueMemberOnLexemeTypeIdName)),
                                  SF.Token(SyntaxKind.CommaToken),
                                  SF.AssignmentExpression(
                                      simpleAssignmentExpression,
                                      lineIndexMemberOnLexemeTypeIdName,
                                      SF.BinaryExpression(
                                          SyntaxKind.SubtractExpression,
                                          indexVarIdName,
                                          matchLengthVarIdName))}))));

            SyntaxToken tokenDefVarIdentifier = SF.Identifier(tokenDefVarName);
            IdentifierNameSyntax tokenListVarIdentifierName = SF.IdentifierName(tokenListVarName);

            ForEachStatementSyntax forEachStatementSyntax = SF.ForEachStatement(
                varType,
                tokenDefVarIdentifier,
                tokenListVarIdentifierName,
                SF.Block(
                    varRegexVariableDeclarationStmt,
                    varMatchVariableDeclarationStmt,
                    ifMatchSuccessNeTrueOrMatchIndexNeIndexStmt,
                    ifMatchLengthEq0Stmt,
                    indexAddAssignmentMatchLengthStmt,
                    assignMatchLengthStmt,
                    returnNewLexemeObjectStmt)).NormalizeWhitespace();

            return forEachStatementSyntax;
        }
    }
}