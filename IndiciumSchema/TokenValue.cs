using System.Diagnostics.CodeAnalysis;

namespace Indicium.Schemas
{
    /// <summary>
    /// Represents a part of string or text that has been recognised according to a <see cref="Token"/> definition.
    /// </summary>
    public class TokenValue
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public int LineIndex { get; set; }
        public int LineNumber { get; set; }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ LineIndex;
                hashCode = (hashCode * 397) ^ LineNumber;
                return hashCode;
            }
        }

        public override string ToString() => 
            $"Token(Id = {Id}, Val = {Value}, Index = {LineIndex}, Line = {LineNumber})";
    }
}