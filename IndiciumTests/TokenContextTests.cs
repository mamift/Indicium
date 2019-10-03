using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Indicium.Schemas;
using NUnit.Framework;

namespace Indicium.Tests
{
    public class TokenContextTests
    {
        private TokenContext _context; 

        [SetUp]
        public void Setup()
        {
            var cwd = Environment.CurrentDirectory;
            var file = Path.Combine(cwd, @"Schemas\Prototype1.xml");

            _context = TokenContext.Load(file);

            Assert.IsNotNull(_context);
        }

        [Test]
        public void TestProgrammaticallyDefinedContext()
        {
            var newContext = new TokenContext {
                Token = new List<Token> {
                    new Token {
                        Id = "Identifier", TypedValue = @"[\w]+"
                    },
                    new Token {
                        Id = "Non-Identifier", TypedValue = @"[\W]+"
                    }
                }
            };

            var tokensFromLyrics = Shared.Lyrics.Split("\r\n").SelectMany((line, i) => {
                newContext.LineNumber = i;
                newContext.InputString = line;

                return newContext.GetTokens();
            }).ToList();

            Assert.IsNotEmpty(tokensFromLyrics);
            Assert.IsTrue(tokensFromLyrics.Count == 736);
        }

        [Test]
        public void TestGenerateExample()
        {
            var eg = TokenContext.GenerateExample("testing.d");

            Assert.IsNotNull(eg);
            Assert.IsNotEmpty(eg.Token);
            Assert.IsFalse(eg.ClassName.Contains("."));
            Assert.IsFalse(eg.NamespaceName.Contains("."));
        }

        [Test]
        public void IgnoreWhitespaceTest()
        {
            var fullTokenList = new List<Lexeme>();

            var lyricLines = Shared.Lyrics.Split("\r\n");
            for (var lineNumber = 1; lineNumber <= lyricLines.Length; lineNumber++) {
                _context.LineNumber = lineNumber;
                _context.IgnoreWhitespace = true;
                _context.InputString = lyricLines[lineNumber-1];

                var tokens = _context.GetTokens().ToList();

                fullTokenList.AddRange(tokens);
            }

            var lexemes = fullTokenList.Where(t => t.Id != "Undefined").ToList();
            var undefinedLexemes = fullTokenList.Except(lexemes).ToList();

            Assert.IsNotEmpty(fullTokenList);
            Assert.IsTrue(fullTokenList.Count == 460);
            Assert.IsTrue(undefinedLexemes.Count == 1);
            Assert.IsTrue(lexemes.Count == 459);
        }

        [Test]
        public void IncludeWhitespaceTest()
        {
            var fullTokenList = new List<Lexeme>();

            foreach (var line in Shared.Lyrics.Split("\r\n")) {
                _context.IgnoreWhitespace = false;
                _context.InputString = line;

                var tokens = _context.GetTokens().ToList();

                fullTokenList.AddRange(tokens);
            }

            var definedTokens = fullTokenList.Where(t => t.Id != "Undefined").ToList();
            var undefinedTokens = fullTokenList.Except(definedTokens).ToList();

            Assert.IsNotEmpty(fullTokenList);
            Assert.IsTrue(fullTokenList.Count == 740);
            Assert.IsTrue(undefinedTokens.Count == 1);
            Assert.IsTrue(definedTokens.Count == 739);
        }
    }
}