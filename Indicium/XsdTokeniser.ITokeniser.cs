using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Indicium
{
    public partial class XsdTokeniser: ITokeniser<object>
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

        public object GetToken()
        {
            var lexeme = ExtractLexeme(InputString, 0, false, out var index, out var matchLength);
            if (lexeme == default(Object)) return default(object);

            //lexeme.LineNumber = _lineNumber;

            return (Object)lexeme;
        }

        public Object PeekToken()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Object> GetTokens()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Object> ProcessLine(string line, int lineNumber)
        {
            InputString = line;
            LineNumber = lineNumber;
            
            var token = GetToken();
            while (token != default(Object))
            {
                yield return token;
                token = GetToken();
            }
        }

        public IEnumerable<Object> ProcessTokens(TextReader reader)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Object> ProcessTokens(string @string, char delimiter = '\n')
        {
            throw new NotImplementedException();
        }
    }
}
