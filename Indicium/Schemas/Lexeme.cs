using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using CSharpSyntax;

namespace Indicium.Schemas
{
    /// <summary>
    /// Represents a part of string or text that has been recognised according to a <see cref="Token"/> definition.
    /// </summary>
    public partial class Lexeme
    {
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TypedValue != null ? TypedValue.GetHashCode() : 0);
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
            var escapeColonInValue = TypedValue.StartsWith(":")
                ? Regex.Replace(TypedValue, @"^\:{1}", @"\:")
                : TypedValue;
            // conversely at the end of the value, the same is also true for the semi colon
            var escapeSemicolonInValue = escapeColonInValue.EndsWith(";")
                ? Regex.Replace(escapeColonInValue, @"\;{1}$", @"\;")
                : escapeColonInValue;

            var str = $"{{{Id}:{escapeSemicolonInValue};{LineNumber}:{LineIndex}}}";

            return str;
        }

        /// <summary>
        /// Returns a compact string representation of the current instance.
        /// </summary>
        /// <returns></returns>
        public string ToCompactString() => $"{{{Id}:{TypedValue}}}";

        /// <summary>
        /// Represents an undefined <see cref="Lexeme"/>.
        /// </summary>
        public static readonly Lexeme Undefined = new Lexeme {Id = "Undefined"};
        
        protected bool Equals(Lexeme other) => Id == other?.Id;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Lexeme) obj);
        }

        public static bool operator ==(Lexeme one, Lexeme other) => Equals(one, other);

        public static bool operator !=(Lexeme one, Lexeme other) => !Equals(one, other);
    }
}