using System;
using System.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Indicium.CodeGen
{
    using SF = SyntaxFactory;

    public static partial class RoslynExtensionMethods
    {
        public static NamespaceDeclarationSyntax AddExtractLexemeExtensionMethod(this NamespaceDeclarationSyntax nds)
        {
            ClassDeclarationSyntax @class = SF.ClassDeclaration("ExtensionMethods");
            SyntaxToken publicKeyword = SF.Token(SyntaxKind.PublicKeyword);
            SyntaxToken staticKeyword = SF.Token(SyntaxKind.StaticKeyword);

            @class = @class.AddModifiers(publicKeyword, staticKeyword);

            const string extractLexemeMethodName = "ExtractLexeme";
            TypeSyntax extractLexemeMethodReturnType = SF.ParseTypeName(CodeGen.AbstractLexemeBaseClassName);
            TypeSyntax extractLexemeMethodFirstThisArgument = SF.ParseTypeName($"{nameof(IEnumerable)}<{CodeGen.AbstractTokenBaseClassName}>");
            MethodDeclarationSyntax extractLexemeMethod = SF.MethodDeclaration(extractLexemeMethodReturnType, extractLexemeMethodName);

            extractLexemeMethod = extractLexemeMethod.AddModifiers(publicKeyword, staticKeyword);

            string tokensVarName = "tokens";
            SyntaxToken tokensIdentifierToken = SF.Identifier(tokensVarName);
            ParameterSyntax tokensParam = SF.Parameter(tokensIdentifierToken)
                                            .WithType(extractLexemeMethodFirstThisArgument)
                                            .WithModifiers(SF.TokenList(SF.Token(SyntaxKind.ThisKeyword)));

            TypeSyntax stringType = SF.ParseTypeName("string");
            string inputVarName = "input";
            ParameterSyntax inputStringParam = SF.Parameter(SF.Identifier(inputVarName))
                                                 .WithType(stringType);

            TypeSyntax intType = SF.ParseTypeName("int");
            const string inputIndexVarName = "inputIndex";
            ParameterSyntax inputIndexIntParam = SF.Parameter(SF.Identifier(inputIndexVarName))
                                                   .WithType(intType);

            TypeSyntax boolType = SF.ParseTypeName("bool");
            string ignoreSpacesVarName = "ignoreSpaces";
            ParameterSyntax ignoreSpacesBoolParam = SF.Parameter(SF.Identifier(ignoreSpacesVarName))
                                                      .WithType(boolType);

            SyntaxToken outKeyword = SF.Token(SyntaxKind.OutKeyword);
            const string indexVariableName = "index";
            ParameterSyntax indexOutParam = SF.Parameter(SF.Identifier(indexVariableName))
                                              .WithType(intType)
                                              .WithModifiers(SF.TokenList(outKeyword));

            const string matchLengthVarName = "matchLength";
            ParameterSyntax matchLengthIntParam = SF.Parameter(SF.Identifier(matchLengthVarName))
                                                    .WithType(intType)
                                                    .WithModifiers(SF.TokenList(outKeyword));

            EqualsValueClauseSyntax booleanEqualsFalseDefaultClause = SF.EqualsValueClause(SF.Token(SyntaxKind.EqualsToken), 
                SF.LiteralExpression(SyntaxKind.FalseLiteralExpression));

            const string obeyEvalOrderVarName = "obeyEvalOrder";
            ParameterSyntax obeyEvalOrderBoolParam = SF.Parameter(SF.Identifier(obeyEvalOrderVarName))
                                                       .WithType(boolType)
                                                       .WithDefault(booleanEqualsFalseDefaultClause);

            extractLexemeMethod = extractLexemeMethod.AddParameterListParameters(tokensParam, inputStringParam, inputIndexIntParam,
                ignoreSpacesBoolParam, indexOutParam, matchLengthIntParam, obeyEvalOrderBoolParam);

            ExpressionStatementSyntax indexIsInputIndexStmt = SF.ExpressionStatement(SF.ParseExpression($"{indexVariableName} = {inputIndexVarName}"));
            ExpressionStatementSyntax matchLengthIsZeroStmt = SF.ExpressionStatement(SF.ParseExpression($"{matchLengthVarName} = 0"));
            ExpressionSyntax ifIndexIsGtOrEqToInputLengthExpr = SF.ParseExpression($"{indexVariableName} >= {inputVarName}.{nameof(String.Length)}");
            ReturnStatementSyntax returnDefaultLexemeStmt = SF.ReturnStatement(SF.ParseExpression($"default({CodeGen.AbstractLexemeBaseClassName})"));
            IfStatementSyntax ifIndexIsGtOrEqToInputLengthStmt = SF.IfStatement(ifIndexIsGtOrEqToInputLengthExpr, returnDefaultLexemeStmt)
                                                     .WithTrailingTrivia(SF.CarriageReturn);

            // past the method initialisation stuff, now the meat of method implementation
            BinaryExpressionSyntax whileIgnoreSpacesConditionExpr = SF.BinaryExpression(
                SyntaxKind.LogicalAndExpression,
                SF.ParseExpression($"({inputVarName}[{indexVariableName}] == ' ' || {inputVarName}[{indexVariableName}] == '\\t')"), 
                SF.ParseExpression($"{ignoreSpacesVarName}")
            );
            StatementSyntax incrementIndexVarStmt = SF.ParseStatement($"{indexVariableName}++;");
            IfStatementSyntax ifIndexIsEqOrGtInputVarLengthThenReturnDefaultBecauseWeAreAtTheEnd = SF.IfStatement(
                SF.ParseExpression($"{indexVariableName} >= {inputVarName}.Length"), 
                returnDefaultLexemeStmt
            );
            BlockSyntax whileIgnoreSpacesExprIsTrueBlock = SF.Block(
                incrementIndexVarStmt,
                ifIndexIsEqOrGtInputVarLengthThenReturnDefaultBecauseWeAreAtTheEnd
            );
            WhileStatementSyntax whileIgnoreSpacesStmt = SF.WhileStatement(whileIgnoreSpacesConditionExpr, whileIgnoreSpacesExprIsTrueBlock);

            const string tokenListVarName = "tokenList";
            ConditionalExpressionSyntax tokenListVarTernaryOperatorInitStmt = SF.ConditionalExpression(
                SF.ParseExpression("true"),
                SF.ParseExpression($"{tokensVarName}.ToList()"),
                SF.ParseExpression($"{tokensVarName}.ToList()")
            );

            // declare tokenList var
            EqualsValueClauseSyntax tokenListVarEqualsValueStmt = SF.EqualsValueClause(tokenListVarTernaryOperatorInitStmt);
            VariableDeclaratorSyntax tokenListVarDeclarator = SF.VariableDeclarator(tokenListVarName)
                .WithInitializer(tokenListVarEqualsValueStmt);
            LocalDeclarationStatementSyntax tokenListVarDeclarationStmt = SF.LocalDeclarationStatement(SF.VariableDeclaration(SF.ParseTypeName("var")))
                .AddDeclarationVariables(tokenListVarDeclarator);
            
            // foreach statement
            var foreachStmt = GenerateForEachStatementCode().WithTrailingTrivia(SF.TriviaList(SF.CarriageReturn));

            // index++ statement
            var indexPostIncrementStatement = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.PostfixUnaryExpression(
                    SyntaxKind.PostIncrementExpression,
                    SyntaxFactory.IdentifierName("index")));

            var finalReturnStatement = GenerateReturnNewLexemeStatement();

            extractLexemeMethod = extractLexemeMethod.AddBodyStatements(
                indexIsInputIndexStmt,
                matchLengthIsZeroStmt,
                ifIndexIsGtOrEqToInputLengthStmt,
                whileIgnoreSpacesStmt,
                tokenListVarDeclarationStmt,
                foreachStmt,
                indexPostIncrementStatement,
                finalReturnStatement
            );

            @class = @class.AddMembers(extractLexemeMethod);

            nds = nds.AddMembers(@class);
            
            return nds;
        }
    }
}