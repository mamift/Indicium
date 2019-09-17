using System;
using System.Collections.Generic;
using System.IO;
using Indicium.Schemas;

namespace Indicium.Example
{
    public class ExampleTokeniser
    {
        private string _inputString;

        public List<TokenBase> Tokens { get; } = new List<TokenBase>() {
            WhitespaceToken.Default,
            IdentifierToken.Default
        };

        public bool IsAtStart => LineIndex == 0;
        public bool IsAtEnd => LineIndex == _inputString.Length;

        public int LineIndex { get; private set; }

        public string InputString
        {
            private get => _inputString;
            set {
                _inputString = value;
                LineIndex = 0;
                LineNumber = 1;
            }
        }

        public string RemainingString => _inputString.Substring(LineIndex, _inputString.Length - LineIndex);

        public int LineNumber { get; set; }

        public void Reset()
        {
            InputString = null;
            LineIndex = 0;
            LineNumber = 0;
        }

        public LexemeBase<TokenBase> ReadToken()
        {
            var lexeme = Tokens.ExtractLexeme(input: _inputString, startIndex: LineIndex, ignoreSpaces: false,
                endIndex: out int lineIndex, matchLength: out int matchLength);

            LineIndex = lineIndex;
            if (lexeme != null) {
                lexeme.LineNumber = LineNumber;
            }

            return lexeme;
        }

        public LexemeBase<TokenBase> PeekToken()
        {
            var startIndexCopy = LineIndex;

            var lexeme = Tokens.ExtractLexeme(input: _inputString, startIndex: startIndexCopy, ignoreSpaces: false,
                endIndex: out _, matchLength: out _);

            lexeme.LineNumber = LineNumber;

            return lexeme;
        }

        public IEnumerable<LexemeBase<TokenBase>> ReadTokens(string line, int lineNumber)
        {
            InputString = line;
            LineNumber = lineNumber;

            var token = ReadToken();
            while (token != default(LexemeBase<TokenBase>))
            {
                yield return token;
                token = ReadToken();
            }
        }

        public IEnumerable<LexemeBase<TokenBase>> ReadTokens(TextReader reader)
        {
            throw new NotImplementedException();
        }
    }
}