using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Indicium.Schemas
{
    /// <summary>
    /// Represents a part of string or text that has been recognised according to a <see cref="Token"/> definition.
    /// </summary>
    public class Lexeme
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

        /// <summary>
        /// Returns a string representation of the current instance. Includes line and line index numbers.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // because the delimiter is a colon, if the lexeme value begins with one, we escape it with a \
            var escapeColonInValue = Value.StartsWith(":")
                ? Regex.Replace(Value, @"^\:{1}", @"\:")
                : Value;
            // conversely at the end of the value, the same is also true for the semi colon
            var escapeSemicolonInValue = escapeColonInValue.EndsWith(";")
                ? Regex.Replace(escapeColonInValue, @"\;{1}$", @"\;")
                : escapeColonInValue;

            return $"{{{Id}:{escapeSemicolonInValue};{LineNumber}:{LineIndex}}}";
        }

        /// <summary>
        /// Returns a compact string representation of the current instance.
        /// </summary>
        /// <returns></returns>
        public string ToCompactString() => $"{{{Id}:{Value}}}";
    }
}