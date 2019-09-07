using System.Text.RegularExpressions;

namespace Indicium
{
    public class UndefinedToken: TokenBase
    {
        public static readonly UndefinedToken Default = new UndefinedToken();

        public override string Id { get; } = nameof(UndefinedToken);

        public override Regex Regex { get; } = null;
    }
}