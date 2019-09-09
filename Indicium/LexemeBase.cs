using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Indicium
{
    /// <summary>
    /// Class that serves as a base for all other Lexeme objects.
    /// </summary>
    /// <typeparam name="TTokenBase"></typeparam>
    [DebuggerDisplay("Value = {" + nameof(Value) + "}, Line = {" + nameof(LineNumber) + "}, Col = {" + nameof(LineIndex) + "}, Token = {" + nameof(Token) + ".ToString()}")]
    public class LexemeBase<TTokenBase> 
        where TTokenBase: TokenBase
    {
        public virtual TTokenBase Token { get; }
        
        public string Value { get; set; }

        public int LineNumber { get; set; }
        
        public int LineIndex { get; set; }

        public LexemeBase(TTokenBase token)
        {
            Token = token;
        }

        protected LexemeBase() { }

        protected LexemeBase(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        protected LexemeBase(string value, int lineNumber, int lineIndex) : this(value)
        {
            LineNumber = lineNumber;
            LineIndex = lineIndex;
        }

        public override string ToString() => $"Value = {Value}, Line = {LineNumber}, Col = {LineIndex}, Token = {Token}";

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode() => 
            Token.GetHashCode() ^ Value.GetHashCode() ^ LineNumber.GetHashCode() ^ LineIndex.GetHashCode();

        protected bool Equals(LexemeBase<TTokenBase> other)
        {
            return EqualityComparer<TTokenBase>.Default.Equals(Token, other.Token) &&
                   Value == other.Value &&
                   LineNumber == other.LineNumber &&
                   LineIndex == other.LineIndex;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LexemeBase<TTokenBase>) obj);
        }

        public static bool operator ==(LexemeBase<TTokenBase> left, LexemeBase<TTokenBase> right) => Equals(left, right);

        public static bool operator !=(LexemeBase<TTokenBase> left, LexemeBase<TTokenBase> right) => !Equals(left, right);
    }
}