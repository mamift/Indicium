using Text = System.Text.RegularExpressions;

namespace Indicium.Schemas
{
    public partial class Token
    {
        private Text.Regex _regex;

        public Text.Regex GetMatcher()
        {
            return _regex ?? (_regex = new Text.Regex(TypedValue.Trim()));
        }

        /// <summary>
        /// Returns a new <see cref="Token"/> instance representing an undefined token, and a given <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        public static Token GetUndefined(string value) => new Token { Id = "Undefined", TypedValue = value };
    }
}
