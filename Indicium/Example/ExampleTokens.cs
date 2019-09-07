using System.Text.RegularExpressions;

namespace Indicium.Example
{
    public class IdentifierToken: TokenBase
    {
        public static readonly IdentifierToken Default = new IdentifierToken();

        public override string Id { get; } = nameof(IdentifierToken);

        public override Regex Regex { get; }

        public IdentifierToken()
        {
            Regex = new Regex(@"[a-zA-Z]+", RegexOptions);
        }
    }

    public class KeywordLexeme : LexemeBase<IdentifierToken>
    {
        public KeywordLexeme(string value) : base(value) { }

        public override IdentifierToken Token { get; } = IdentifierToken.Default;
    }

    public class WhitespaceToken : TokenBase
    {
        public static readonly WhitespaceToken Default = new WhitespaceToken();

        public override string Id { get; } = nameof(WhitespaceToken);

        public override Regex Regex { get; }

        public WhitespaceToken()
        {
            Regex = new Regex(@"[\s]", RegexOptions);
        }
    }

    public class WhitespaceLexeme : LexemeBase<WhitespaceToken>
    {
        public WhitespaceLexeme(string value) : base(value) { }

        public override WhitespaceToken Token { get; } = WhitespaceToken.Default;
    }
}
