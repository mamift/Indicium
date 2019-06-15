using Text = System.Text.RegularExpressions;

namespace Indicium.Schemas
{
    public partial class Token
    {
        private Text.Regex _regex;

        public Text.Regex GetMatcher() => _regex ?? (_regex = new Text.Regex(TypedValue.Trim()));
    }
}
