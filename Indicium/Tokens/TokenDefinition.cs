using System.Text.RegularExpressions;

namespace Indicium.Tokens
{
    public struct TokenDefinition
    {
        public string RegexString { get; }

        public string Identifier { get; }

        public Regex Regex { get; }

        public TokenDefinition(string identifer, string regexString)
        {
            RegexString = regexString;
            Identifier = identifer;
            Regex = new Regex(regexString);
        }
    }
}