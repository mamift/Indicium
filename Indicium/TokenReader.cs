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
        private readonly List<TokenBase> _tokens = new List<TokenBase>();

        public IReadOnlyCollection<TokenBase> Tokens => _tokens;

        protected StringReader Reader { get; }

        public TokenReader(string theString, int bufferSize = 3)
        {
            Reader = new StringReader(theString);
            var sb = new StringBuilder();

            while (true) {
                var buffer = new char[bufferSize];
                var count = Reader.ReadBlock(buffer, 0, buffer.Length);
                if (count <= 0) break;

                var whiteSpaceCount = buffer.Count(char.IsWhiteSpace);
                if (whiteSpaceCount == buffer.Length) continue;

                sb.AppendNoConsecutiveWhitespace(buffer);
            }
        }
    }
}