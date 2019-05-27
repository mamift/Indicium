using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Indicium.Tokens
{
    [DebuggerDisplay("Id = {" + nameof(Identifier) + ("}, Regex = {" + nameof(Regex) + "}"))]
    public struct TokenDefinition
    {
        public string Identifier { get; }

        public Regex Regex { get; }

        public TokenDefinition(string identifier, string regexString)
        {
            Identifier = identifier;
            Regex = new Regex(regexString);
        }
    }
}