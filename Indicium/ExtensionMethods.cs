using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Indicium
{
    public static class ExtensionMethods
    {
        public static void AddMinimumNamespaces(this CodeNamespace cn)
        {
            var system = nameof(System);
            var text = nameof(System.Text);
            var regularExpressions = nameof(System.Text.RegularExpressions);
            cn.Imports.Add(new CodeNamespaceImport(system));
            cn.Imports.Add(new CodeNamespaceImport(nameof(Indicium)));
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
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="chars"></param>
        public static void AppendNoConsecutiveWhitespace(this StringBuilder sb, IEnumerable<char> chars)
        {
            var charArray = chars as char[] ?? chars.ToArray();
            var hasAtleastOneWhitespace = false;
            for (var i = 0; i < charArray.Count(); i++)
            {
                var isWhitespace = char.IsWhiteSpace(charArray[i]);
                if (isWhitespace && hasAtleastOneWhitespace) continue;
                if (charArray[i] == default(char)) continue;
                sb.Append(charArray[i]);
                if (isWhitespace) hasAtleastOneWhitespace = true;
            }
        }
    }
}