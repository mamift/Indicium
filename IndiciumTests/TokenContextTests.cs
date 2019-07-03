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
        internal const string Lyrics = "If that's the way it's gonna be.\n" +
                                       "Leave your shiny yellow key on the doorstep.\n" +
                                       "And start burning up your tires.\n" +
                                       "And take your cold hard cash.\n" +
                                       "And this shirt right off my back.\n" +
                                       "I don't mind.\n" +
                                       "'Cause I'm gonna set this house on.\n" +
                                       "Don't have to hit below the belt.\n" +
                                       "With those leather shoes you wear so well.\n" +
                                       "No, you don't have to kiss and tell.\n" +
                                       "'Cause you're only gonna hurt yourself.\n" +
                                       "The minute that I walk in.\n" +
                                       "You're tryna hold me down.\n" +
                                       "Tryna sink your claws in.\n" +
                                       "'Til I'm face flat on the ground.\n" +
                                       "Don't know what you've been drinking.\n" +
                                       "Every time you come around.\n" +
                                       "So let me down.\n" +
                                       "If that's the way it's gonna be.\n" +
                                       "Leave your shiny yellow key on the doorstep.\n" +
                                       "And start burning up your tires.\n" +
                                       "And take your cold hard cash.\n" +
                                       "Take this shirt right off my back.\n" +
                                       "I don't mind.\n" +
                                       "'Cause I'm gonna set this house on fire.\n" +
                                       "I sent you running for the hills.\n" +
                                       "I guess by now you know the drill.\n" +
                                       "The type that only shoots to kill.\n" +
                                       "And you only do it for the thrill.\n" +
                                       "The minute that I walk in.\n" +
                                       "You're tryna hold me down.\n" +
                                       "Tryna sink your claws in.\n" +
                                       "'Til I'm face flat on the ground.\n" +
                                       "Don't know what you've been drinking.\n" +
                                       "Every time you come around.\n" +
                                       "So let me down.\n" +
                                       "If that's the way it's gonna be.\n" +
                                       "Leave your shiny yellow key on the doorstep.\n" +
                                       "And start burning up your tires.\n" +
                                       "And take your cold hard cash.\n" +
                                       "Take this shirt right off my back.\n" +
                                       "I don't mind.\n" +
                                       "'Cause I'm gonna set this house on fire.\n" +
                                       "House on.\n" +
                                       "Set this house on.\n" +
                                       "House on.\n" +
                                       "House on fire.\n" +
                                       "House on fire.\n" +
                                       "House on fire.\n" +
                                       "House on fire.\n" +
                                       "If that's the way it's gonna be.\n" +
                                       "Leave your shiny yellow key on the doorstep.\n" +
                                       "And start burning up your tires.\n" +
                                       "And take your cold hard cash.\n" +
                                       "Take this shirt right off my back.\n" +
                                       "I don't mind.\n" +
                                       "'Cause I'm gonna set this house on.\n\r" +
                                       "\"Fire\" by Peking Duck.@";

        public static TokenContext GetDefaultContext() => new TokenContext
        {
            Token = new List<Token> {
                new Token {
                    Id = "Identifier", TypedValue = @"[\w]+", EvaluationOrder = 1
                },
                new Token {
                    Id = "Non-Identifier", TypedValue = @"[\W]+", EvaluationOrder = 3
                },
                new Token {
                    Id = "Whitespace", TypedValue = @"[\s]+", EvaluationOrder = 2
                }
            }
        };

        private TokenContext _context; 

        [SetUp]
        public void Setup()
        {
            var cwd = Environment.CurrentDirectory;
            var file = Path.Combine(cwd, @"Prototype1.xml");

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

            var tokensFromLyrics = Lyrics.Split('\n').SelectMany((line, i) => {
                newContext.LineNumber = i;
                newContext.InputString = line;

                return newContext.GetTokens();
            }).ToList();

            Assert.IsNotEmpty(tokensFromLyrics);
            Assert.IsTrue(tokensFromLyrics.Count == 736);
        }

        [Test]
        public void TestEvaluationOrder1()
        {
            var context = GetDefaultContext();

            var tokensFromLyrics = Lyrics.Split('\n').SelectMany((line, i) => {
                context.LineNumber = i;
                context.InputString = line;

                return context.GetTokens();
            }).ToList();

            Assert.IsNotEmpty(tokensFromLyrics);
            Assert.IsTrue(tokensFromLyrics.Count == 737);
        }

        [Test]
        public void TestEvaluationOrder2()
        {
            var context = GetDefaultContext();
            context.IgnoreSpaces = false;
            context.ObeyEvaluationOrder = true;

            var unsorted = context.Token.ToList();

            Assert.IsTrue(unsorted.Last().Id == "Whitespace");

            var sorted = context.Token.OrderBy(t => t.EvaluationOrder).ToList();

            Assert.IsTrue(sorted.Last().Id == "Non-Identifier");
        }

        [Test]
        public void IgnoreWhitespaceTest()
        {
            var fullTokenList = new List<Lexeme>();

            var lyricLines = Lyrics.Split('\n');
            for (var lineNumber = 0; lineNumber < lyricLines.Length; lineNumber++) {
                _context.LineNumber = lineNumber;
                _context.IgnoreSpaces = true;
                _context.InputString = lyricLines[lineNumber];

                var tokens = _context.GetTokens().ToList();

                fullTokenList.AddRange(tokens);
            }

            var definedTokens = fullTokenList.Where(t => t.Id != "Undefined").ToList();
            var undefinedTokens = fullTokenList.Except(definedTokens).ToList();

            Assert.IsNotEmpty(fullTokenList);
            Assert.IsTrue(fullTokenList.Count == 461);
            Assert.IsTrue(undefinedTokens.Count == 1);
            Assert.IsTrue(definedTokens.Count == 460);
        }

        [Test]
        public void IncludeWhitespaceTest()
        {
            var fullTokenList = new List<Lexeme>();

            foreach (var line in Lyrics.Split('\n')) {
                _context.IgnoreSpaces = false;
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