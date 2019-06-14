using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Indicium.Schemas;
using NUnit.Framework;

namespace Tests
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

        private TokenContext Context { get; set; }

        [SetUp]
        public void Setup()
        {
            var cwd = Environment.CurrentDirectory;
            var file = Path.Combine(cwd, @"Prototype1.xml");

            Context = TokenContext.Load(file);

            Assert.IsNotNull(Context);
        }

        [Test]
        public void IgnoreWhitespaceTest()
        {
            var fullTokenList = new List<Token>();

            foreach (var line in Lyrics.Split('\n')) {
                Context.IgnoreSpaces = true;
                Context.InputString = line;

                var tokens = Context.GetTokens().ToList();

                fullTokenList.AddRange(tokens);
            }

            var definedTokens = fullTokenList.Where(t => t.Id != "Undefined").ToList();
            var undefinedTokens = fullTokenList.Except(definedTokens).ToList();

            Assert.IsNotEmpty(fullTokenList);
            Assert.IsTrue(fullTokenList.Count == 461);
            Assert.IsTrue(undefinedTokens.Count == 3);
            Assert.IsTrue(definedTokens.Count == 458);
        }

        [Test]
        public void IncludeWhitespaceTest()
        {
            var fullTokenList = new List<Token>();

            foreach (var line in Lyrics.Split('\n')) {
                Context.IgnoreSpaces = false;
                Context.InputString = line;

                var tokens = Context.GetTokens().ToList();

                fullTokenList.AddRange(tokens);
            }

            var definedTokens = fullTokenList.Where(t => t.Id != "Undefined").ToList();
            var undefinedTokens = fullTokenList.Except(definedTokens).ToList();

            Assert.IsNotEmpty(fullTokenList);
            Assert.IsTrue(fullTokenList.Count == 740);
            Assert.IsTrue(undefinedTokens.Count == 2);
            Assert.IsTrue(definedTokens.Count == 738);
        }
    }
}