using System;

namespace Indicium
{
    /// <summary>
    /// Class that serves as a base for all other Lexeme objects.
    /// </summary>
    /// <typeparam name="TTokenBase"></typeparam>
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
    }
}