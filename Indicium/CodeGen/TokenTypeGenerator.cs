using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Indicium.Schemas;

namespace Indicium.CodeGen
{
    public partial class TokenTypeGenerator
    {
        /// <summary>
        /// Generates the TokenContext class that processes input strings and outputs Tokens found (in the order that they are found..
        /// </summary>
        /// <returns></returns>
        public static CodeTypeDeclaration GenerateTokenContextClass()
        {
            var type = new CodeTypeDeclaration(Indicium.CodeGen.CodeGen.TokenContextClassName) {
                TypeAttributes = TypeAttributes.Public,
                IsPartial = true
            };

            var codeConstructor = new CodeConstructor {
                Name = Indicium.CodeGen.CodeGen.TokenContextClassName,
                Attributes = MemberAttributes.Public
            };
            
            type.Members.Add(codeConstructor);
                        
            return type;
        }

        /// <summary>
        /// Outputs, under a single <paramref name="namespace"/>, code for classes, that correspond to each given <see cref="Token"/>.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="namespace"></param>
        /// <returns></returns>
        public static CodeCompileUnit GenerateClassesForTokenDefinitions(IEnumerable<Token> tokens,
            string @namespace = "Indicia")
        {
            var containingNamespace = new CodeNamespace(@namespace);
            containingNamespace.AddMinimumNamespaces();
            var tokenContextClass = GenerateTokenContextClass();

            containingNamespace.Types.Add(tokenContextClass);
            containingNamespace.Types.Add(GenerateTokenAbstractBaseClass());
            containingNamespace.Types.Add(GenerateLexemeAbstractBaseClass());
            
            var tokenList = tokens.ToList();

            var ccu = new CodeCompileUnit();

            foreach (var tokenDef in tokenList) {
                var tokenType = GenerateTokenClassForTokenDef(tokenDef);
                var lexemeType = GenerateLexemeClassForTokenDef(tokenDef, tokenType);

                containingNamespace.Types.Add(tokenType);
                containingNamespace.Types.Add(lexemeType);
            }
            
            ccu.Namespaces.Add(containingNamespace);
            return ccu;
        }

        /// <summary>
        /// Generates the base type that Token classes inherit from.
        /// </summary>
        /// <returns></returns>
        public static CodeTypeDeclaration GenerateTokenAbstractBaseClass()
        {
            var tokenBaseClass = new CodeTypeDeclaration(Indicium.CodeGen.CodeGen.AbstractTokenBaseClassName) {
                TypeAttributes = TypeAttributes.Abstract | TypeAttributes.Public,
                IsPartial = true
            };

            var publicAbstract = MemberAttributes.Public | MemberAttributes.Abstract;
            tokenBaseClass.AddDefaultGetter("string", "Id", publicAbstract);
            tokenBaseClass.AddDefaultGetter(nameof(Regex), nameof(Regex), publicAbstract);

            var constructor = new CodeConstructor {
                Name = Indicium.CodeGen.CodeGen.AbstractLexemeBaseClassName,
                Attributes = MemberAttributes.Family
            };

            tokenBaseClass.Members.Add(constructor);

            return tokenBaseClass;
        }

        /// <summary>
        /// Generates the base type that Lexeme classes inherit from.
        /// </summary>
        /// <returns></returns>
        public static CodeTypeDeclaration GenerateLexemeAbstractBaseClass()
        {
            const string valueVarName = "value";
            var privateValueFieldName = $"_{valueVarName}";

            var genericTTokenType = new CodeTypeParameter(Indicium.CodeGen.CodeGen.GenericTTokenTypeName) {
                HasConstructorConstraint = true,
                Constraints = { Indicium.CodeGen.CodeGen.TokenBaseCodeTypeReference }
            };
            var lexemeBaseClass = new CodeTypeDeclaration(Indicium.CodeGen.CodeGen.AbstractLexemeBaseClassName) {
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Abstract,
                IsPartial = true,
                TypeParameters = { genericTTokenType }
            };

            lexemeBaseClass.AddDefaultGetter(Indicium.CodeGen.CodeGen.GenericTTokenTypeName, "Token", MemberAttributes.Public | MemberAttributes.Abstract);

            var valueField = new CodeMemberField {
                Name = privateValueFieldName,
                Type = new CodeTypeReference(typeof(string)),
                Attributes = MemberAttributes.Family | MemberAttributes.Final
            };
            var valueProp = new CodeMemberProperty {
                Type = new CodeTypeReference(typeof(string)),
                Name = "Value",
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                HasGet = true,
                GetStatements = {
                    new CodeMethodReturnStatement(new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), privateValueFieldName))
                }
            };
            
            var throwArgNullExceptionStmt = new CodeThrowExceptionStatement(
                new CodeObjectCreateExpression(
                    new CodeTypeReference(nameof(ArgumentNullException)), new CodeSnippetExpression($"nameof({valueVarName})")));
            
            var stringIsNullOrWhitespaceInvocation = new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(
                    new CodeTypeReference(typeof(string))), 
                nameof(string.IsNullOrEmpty), 
                new CodeVariableReferenceExpression(valueVarName));

            var ifValueIsNullStmt = new CodeConditionStatement(
                stringIsNullOrWhitespaceInvocation, throwArgNullExceptionStmt);
            
            var constructor = new CodeConstructor {
                Parameters = {
                    new CodeParameterDeclarationExpression(typeof(string), valueVarName)
                },
                Name = lexemeBaseClass.Name,
                Attributes = MemberAttributes.Family,
                Statements = {
                    ifValueIsNullStmt,
                    new CodeAssignStatement(
                        new CodePropertyReferenceExpression(
                            new CodeThisReferenceExpression(), 
                            privateValueFieldName), 
                        new CodeVariableReferenceExpression(valueVarName))
                }
            };
            
            lexemeBaseClass.Members.Add(valueField);
            lexemeBaseClass.Members.Add(valueProp);

            lexemeBaseClass.Members.Add(constructor);

            return lexemeBaseClass;
        }

        /// <summary>
        /// Generates the Lexeme classes accompanies each Token class.
        /// </summary>
        /// <param name="tokenDef"></param>
        /// <param name="tokenType"></param>
        /// <returns></returns>
        public static CodeTypeDeclaration GenerateLexemeClassForTokenDef(Token tokenDef, CodeTypeDeclaration tokenType)
        {
            const string valueFieldName = "value";

            var lexemeTypeName = $"{tokenDef.Id}Lexeme";

            var lexemeType = new CodeTypeDeclaration(lexemeTypeName) {
                TypeAttributes = TypeAttributes.Public,
                IsPartial = true,
                BaseTypes = {
                    new CodeTypeReference(new CodeTypeParameter(Indicium.CodeGen.CodeGen.AbstractLexemeBaseClassName)) {
                        TypeArguments = { new CodeTypeReference(tokenType.Name) }
                    }
                }
            };

            var overriddenTokenProp = new CodeMemberProperty {
                Name = "Token",
                Type = new CodeTypeReference(new CodeTypeParameter($"{tokenDef.Id}Token")),
                Attributes = MemberAttributes.Public | MemberAttributes.Override,
                HasGet = true,
                GetStatements = {
                    new CodeMethodReturnStatement(new CodeFieldReferenceExpression(
                        new CodeTypeReferenceExpression(tokenType.Name), "Default"))
                }
            };
            
            var constructor = new CodeConstructor {
                Parameters = { new CodeParameterDeclarationExpression(typeof(string), valueFieldName) },
                Name = lexemeType.Name,
                Attributes = MemberAttributes.Public,
                BaseConstructorArgs = { new CodeVariableReferenceExpression(valueFieldName) }
            };

            lexemeType.Members.Add(overriddenTokenProp);
            lexemeType.Members.Add(constructor);

            return lexemeType;
        }

        /// <summary>
        /// Generates the class (fields and properties) for a given <paramref name="tokenDef"/>.
        /// </summary>
        /// <param name="tokenDef"></param>
        /// <returns></returns>
        public static CodeTypeDeclaration GenerateTokenClassForTokenDef(Token tokenDef)
        {
            var tokenClass = new CodeTypeDeclaration($"{tokenDef.Id}Token") {
                TypeAttributes = TypeAttributes.Public,
                BaseTypes = {
                    new CodeTypeReference(new CodeTypeParameter(Indicium.CodeGen.CodeGen.AbstractTokenBaseClassName))
                },
                IsPartial = true
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
                Type = Indicium.CodeGen.CodeGen.RegexCodeTypeReferenceByName,
                InitExpression = privateRegexFieldInitialisation
            };

            // public properties
            var identifierPropertyReturnStatement = new CodeMethodReturnStatement(
                                                        new CodeFieldReferenceExpression(
                                                            new CodeTypeReferenceExpression(tokenClass.Name), privateIdentifierField.Name));

            var propertyAttrs = MemberAttributes.Public | MemberAttributes.Override;

            var identifierProperty = new CodeMemberProperty {
                Attributes = propertyAttrs,
                Name = nameof(tokenDef.Id),
                HasGet = true,
                HasSet = false,
                ImplementationTypes = {new CodeTypeReference(typeof(string))},
                GetStatements = {identifierPropertyReturnStatement},
                Type = new CodeTypeReference(typeof(string))
            };

            var regexPropertyReturnStatement = new CodeMethodReturnStatement(
                                                new CodeFieldReferenceExpression(
                                                    new CodeTypeReferenceExpression(tokenClass.Name), privateRegexField.Name));
            var regexProperty = new CodeMemberProperty {
                Attributes = propertyAttrs,
                Name = nameof(Regex),
                HasSet = false, HasGet = true,
                ImplementationTypes = {new CodeTypeReference(typeof(Regex))},
                GetStatements = {regexPropertyReturnStatement},
                Type = Indicium.CodeGen.CodeGen.RegexCodeTypeReferenceByName
            };

            //default value
            var defaultFieldName = "_default";
            var privateStaticDefaultToken = new CodeMemberField(tokenClass.Name, defaultFieldName) {
                Attributes = MemberAttributes.Private | MemberAttributes.Static,
                InitExpression = new CodeObjectCreateExpression(new CodeTypeReference(new CodeTypeParameter(tokenClass.Name)))
            };
            var publicStaticDefaultGetter = new CodeMemberProperty {
                Name = "Default",
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
                Type = new CodeTypeReference(new CodeTypeParameter(tokenClass.Name)),
                HasGet = true,
                GetStatements = {
                    new CodeMethodReturnStatement(new CodeFieldReferenceExpression(
                        new CodeTypeReferenceExpression(tokenClass.Name), defaultFieldName))
                }
            };

            var constructor = new CodeConstructor {
                Attributes = MemberAttributes.Public
            };

            tokenClass.Members.Add(privateIdentifierField);
            tokenClass.Members.Add(privateRegexField);
            tokenClass.Members.Add(privateStaticDefaultToken);
            
            tokenClass.Members.Add(identifierProperty);
            tokenClass.Members.Add(regexProperty);
            tokenClass.Members.Add(publicStaticDefaultGetter);

            tokenClass.Members.Add(constructor);

            return tokenClass;
        }
    }
}