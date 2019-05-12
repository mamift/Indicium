using System.Data.Common;

namespace Indicium.Tokens
{
    public struct Token
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public Token(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public static bool operator ==(Token left, Token right)
        {
            var sameName = left.Name == right.Name;
            var sameValue = left.Value == right.Value;
            return sameName && sameValue;
        }

        public static bool operator !=(Token left, Token right)
        {
            return !(left == right);
        }
    }
}