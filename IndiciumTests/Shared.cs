using System;
using System.Collections.Generic;
using System.IO;
using Indicium.CodeGen;
using Indicium.Schemas;
using Microsoft.CodeAnalysis;

namespace Indicium.Tests
{
    public static class Shared
    {
        public static readonly string Lyrics = File.ReadAllText("lyrics.txt");

        public static TokenContext GetDefaultContext() => new TokenContext
        {
            Token = new List<Token> {
                new Token {
                    Id = "Identifier", TypedValue = @"[\w]+",
                },
                new Token {
                    Id = "Non-Identifier", TypedValue = @"[\W]+",
                },
                new Token {
                    Id = "Whitespace", TypedValue = @"[\s]+",
                }
            }
        };

        public static TokenContext GetPrototype1Context()
        {
            var cwd = Environment.CurrentDirectory;
            var file = Path.Combine(cwd, @"Schemas\Prototype1.xml");

            return TokenContext.Load(file);
        }

        public static string GetGeneratedCode(TokenContext context)
        {
            var namespaceDeclaration = TokenContextTypeGenerator.GenerateTokeniserCode(context, "TokensNamespace");

            return namespaceDeclaration.NormalizeWhitespace().ToFullString();
        }
    }
}