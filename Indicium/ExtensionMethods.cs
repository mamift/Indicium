using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xml.Schema.Linq.Extensions;

namespace Indicium
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Strips a string of all characters that are not the first 255 characters in UTF-8
        /// </summary>
        /// <param name="theString"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string StripStringOfAllNonAnsiCharacters(this string theString, char? replacement = null)
        {
            return Regex.Replace(theString, @"[^\u0000-\u007F]+", ((replacement == null) ? string.Empty : $"{replacement}"));
        }

        /// <summary>
        /// Removes all non alphanumeric characters. Optionally provide a replacement string.
        /// </summary>
        /// <param name="theString"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string StripOfNonAlphanumericChars(this string theString, string replacement = "")
        {
            if (theString.IsEmpty())
                return replacement;

            var replaceTitleRegex = new Regex("\\W+", RegexOptions.Compiled);
            return replaceTitleRegex.Replace(theString, replacement);
        }


        /// <summary>
        /// Checks if the current <paramref name="thing"/> is equal to at least one of the <paramref name="others"/>.
        /// <para>Invokes <see cref="object.Equals(object)"/> to conduct comparison.</para>
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="thing"></param>
        /// <param name="others">An array of other things to compare against.</param>
        /// <returns></returns>
        public static bool IsOneOf<TType>(this TType thing, params TType[] others)
        {
            if (!others.Any()) return false;

            for (var i = 0; i < others.Length; i++) {
                if (thing.Equals(others[i])) return true;
            }

            return false;
        }

        public static string ToDelimitedString<TType>(this IEnumerable<TType> things)
        {
            var sb = new StringBuilder();

            var lexemesList = things.ToList();
            for (var i = 0; i < lexemesList.Count; i++) {
                var comma = (i == lexemesList.Count - 1) ? string.Empty : ",";
                sb.Append($"{lexemesList.ElementAt(i)}{comma}");
            }

            return sb.ToString();
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