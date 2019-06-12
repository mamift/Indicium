using Text = System.Text.RegularExpressions;

namespace Indicium.Schemas
{
    public partial class Token
    {
        private Text.Regex _regex;

        public Text.Regex GetMatcher() => _regex ?? (_regex = new Text.Regex(TypedValue));

        /// <summary>
        /// Not part of the XML model; determines the line number this token was found in.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Not part of the XML model; determines the index in the current line.
        /// </summary>
        public int LineIndex { get; set; }
    }
}
