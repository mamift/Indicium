using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Indicium
{
    public static class ExtensionMethods
    {
        public static string ToDelimitedString<TType>(this IEnumerable<TType> lexemes)
        {
            var sb = new StringBuilder();

            var lexemesList = lexemes.ToList();
            for (var i = 0; i < lexemesList.Count; i++) {
                var comma = (i == lexemesList.Count - 1) ? string.Empty : ",";
                sb.Append($"{lexemesList.ElementAt(i)}{comma}");
            }

            return sb.ToString();
        }

        public static void AddMinimumNamespaces(this CodeNamespace cn, bool addIndciumRef = false)
        {
            var system = nameof(System);
            var text = nameof(System.Text);
            var regularExpressions = nameof(System.Text.RegularExpressions);
            cn.Imports.Add(new CodeNamespaceImport(system));
            if (addIndciumRef) cn.Imports.Add(new CodeNamespaceImport(nameof(Indicium)));
            cn.Imports.Add(new CodeNamespaceImport($"{system}.{text}"));
            cn.Imports.Add(new CodeNamespaceImport($"{system}.{text}.{regularExpressions}"));
        }

        /// <summary>
        /// Transforms an identifier string to comply with the private field naming convention.
        /// <para>That is: 'MyName' => '_myName'</para>
        /// </summary>
        /// <param name="theString"></param>
        /// <returns></returns>
        public static string Privatise(this string theString)
        {
            var splitByCase = Regex.Matches(theString, @"([A-Z][a-z]+)").Cast<Match>().Select(m => m.Value).ToList();
            splitByCase[0] = $"_{splitByCase[0].ToLower()}";
            return string.Join(string.Empty, splitByCase);
        }

        /// <summary>
        /// Append a sequence of <paramref name="chars"/>, but does not append multiple whitespace chars.
        /// <para>Ensures that exactly one whitespace char is appended.</para>
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="chars"></param>
        public static void AppendNoConsecutiveWhitespace(this StringBuilder sb, IEnumerable<char> chars)
        {
            var charArray = chars as char[] ?? chars.ToArray();
            var hasAtLeastOneWhitespace = false;
            for (var i = 0; i < charArray.Length; i++) {
                if (charArray[i] == '\t') charArray[i] = ' '; // tabs to space! death to tabs! and people who prefer them!
                var isWhitespace = char.IsWhiteSpace(charArray[i]);
                if (isWhitespace && hasAtLeastOneWhitespace) continue;
                if (charArray[i] == default(char)) continue;
                sb.Append(charArray[i]);
                if (isWhitespace) hasAtLeastOneWhitespace = true;
            }
        }
    }
}