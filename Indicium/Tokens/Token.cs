using System.Diagnostics;

namespace Indicium.Tokens
{
    [DebuggerDisplay("Name = {" + nameof(Name) + ("}, Value = {" + nameof(Value) + "}"))]
    public struct Token
    {
        public string Name { get; }

        public string Value { get; }

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

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var tokenEquality = (Token)obj == this;
            return tokenEquality || base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + Value.GetHashCode();
        }
    }
}