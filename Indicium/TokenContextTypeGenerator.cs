using System;
using System.Collections.Generic;
using CSharpSyntax;
using Indicium.Schemas;
using Xml.Schema.Linq.Extensions;

namespace Indicium
{
    public class TokenContextTypeGenerator
    {
        private static TypeSyntax NewGenericTypeName(string genericType, string genericArg1)
        {
            return new GenericNameSyntax {
                Identifier = genericType,
                TypeArgumentList = new TypeArgumentListSyntax {
                    Arguments = {
                        new IdentifierNameSyntax {Identifier = genericArg1}
                    }
                }
            };
        }

        public static NamespaceDeclarationSyntax Namespace(TokenContext context)
        {
            return default(NamespaceDeclarationSyntax);
        }

        public static ClassDeclarationSyntax Class(TokenContext context, string identifier)
        {
            if (identifier.IsEmpty()) throw new ArgumentNullException(nameof(identifier));

            var equalsValueClauseSyntax = new EqualsValueClauseSyntax {
                Value = new ObjectCreationExpressionSyntax {
                    Type = NewGenericTypeName("List", nameof(TokenBase)),
                    ArgumentList = new ArgumentListSyntax()
                }
            };

            const string tokensIdentifier = "_tokens";

            var classDeclaration = new ClassDeclarationSyntax {
                Identifier = identifier,
                Modifiers = Modifiers.Public | Modifiers.Partial,
                Members = {
                    new FieldDeclarationSyntax {
                        Modifiers = Modifiers.Private,
                        Declaration = new VariableDeclarationSyntax {
                            Type = NewGenericTypeName("List", nameof(TokenBase)),
                            Variables = {
                                new VariableDeclaratorSyntax {
                                    Identifier = tokensIdentifier
                                }
                            }
                        }
                    },
                    new ConstructorDeclarationSyntax {
                        Identifier = identifier,
                        Modifiers = Modifiers.Public,
                        Body = new BlockSyntax {
                            Statements = {
                                new ExpressionStatementSyntax {
                                    Expression = new MemberAccessExpressionSyntax {
                                        Name = new IdentifierNameSyntax {
                                            Identifier = tokensIdentifier
                                        }
                                    }
                                }
                            }
                        },
                        ParameterList = new ParameterListSyntax()
                    }
                }
            };

            return classDeclaration;
        }

        public static void MyMethod()
        {
            
        }
    }
}