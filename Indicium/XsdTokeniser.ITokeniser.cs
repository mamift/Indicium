using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Indicium.Schemas;

namespace Indicium
{
    public partial class XsdTokeniser: ITokeniser
    {
        public bool IsAtStart { get; }
        public int LineIndex { get; private set; }
        public string InputString { private get; set; }
        public int LineNumber { get; set; }

        public void Reset()
        {
            InputString = null;
            LineIndex = 0;
            LineNumber = 1;
        }

        public Lexeme GetToken()
        {
            var lexeme = ExtractLexeme(InputString, 0, false, out var index, out var matchLength);
            if (lexeme == default(Lexeme)) return default;

            //lexeme.LineNumber = _lineNumber;

            return (Lexeme)lexeme;
        }

        public Lexeme PeekToken()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Lexeme> GetTokens()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Lexeme> ProcessLine(string line, int lineNumber)
        {
            InputString = line;
            LineNumber = lineNumber;
            
            var token = GetToken();
            while (token != default(Lexeme))
            {
                yield return token;
                token = GetToken();
            }
        }

        public IEnumerable<Lexeme> ProcessTokens(TextReader reader)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Lexeme> ProcessTokens(string @string, char delimiter = '\n')
        {
            throw new NotImplementedException();
        }
    }
}
