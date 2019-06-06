using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Indicium.Tokens;

namespace Indicium
{
    /// <summary>
    /// Alternative implementation of <see cref="Tokeniser"/>, except uses token types defined by
    /// code generation (<see cref="TypeGenerator.GenerateClassesForTokenDefinitions"/>).
    /// </summary>
    public class TokenReader
    {
        private List<TokenBase> _tokens = new List<TokenBase>();

        public IReadOnlyCollection<TokenBase> Tokens => _tokens;

        protected StringReader Reader { get; }

        public TokenReader(string theString)
        {
            Reader = new StringReader(s: theString);
            var sb = new StringBuilder();

            while (true) {
                var buffer = new char[3];
                var count = Reader.ReadBlock(buffer: buffer, index: 0, count: buffer.Length);
                if (count == 0) break;

                var whiteSpaceCount = buffer.Count(ch => char.IsWhiteSpace(ch));
                if (whiteSpaceCount == buffer.Length) continue;

                sb.AppendNoConsecutiveWhitespace(buffer);
            }
        }
    }
}