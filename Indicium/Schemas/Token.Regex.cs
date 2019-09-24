using Text = System.Text.RegularExpressions;

namespace Indicium.Schemas
{
    public partial class Token
    {
        private Text.Regex _regex;

        /// <summary>
        /// Returns the <see cref="Text.Regex"/> instance for this Token. Uses options specified in the
        /// static field: <see cref="TokenContext.RegexOptions"/>.
        /// </summary>
        /// <returns></returns>
        public Text.Regex GetMatcher()
        {
            return _regex ?? (_regex = new Text.Regex(TypedValue.Trim(), TokenContext.RegexOptions));
        }

        /// <summary>
        /// Returns a new <see cref="Token"/> instance representing an undefined token, and a given <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        public static Token GetUndefined(string value) => new Token { Id = "Undefined", TypedValue = value };

        public string GetIdForCodeGen() => $"{Id.StripOfNonAlphanumericChars()}Token";
    }
}
