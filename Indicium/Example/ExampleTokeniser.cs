using System.Collections.Generic;
using System.IO;
using Indicium.Schemas;

namespace Indicium.Example
{
    public class ExampleTokeniser
    {
        private string _inputString;

        private List<TokenBase> Tokens { get; } = new List<TokenBase>() {
            WhitespaceToken.Default,
            IdentifierToken.Default
        };

        public bool IsAtStart => LineIndex == 0;

        public int LineIndex { get; private set; }

        public string InputString
        {
            private get => _inputString;
            set {
                _inputString = value;
                LineIndex = 0;
            }
        }

        public int LineNumber { private get; set; }

        public void Reset()
        {
            InputString = null;
            LineIndex = 0;
            LineNumber = 0;
        }

        public LexemeBase<TokenBase> ReadToken()
        {
            var lexeme = Tokens.ExtractLexeme(_inputString, LineIndex, false, out int lineIndex, out var matchLength);
            LineIndex = lineIndex;
            if (lexeme != null) {
                lexeme.LineNumber = LineNumber;
            }

            return lexeme;
        }

        public LexemeBase<TokenBase> PeekToken()
        {
            return null;
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
            yield break;
        }
    }
}