using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Indicium.Schemas;
using Xml.Schema.Linq.Extensions;
using RegexOptions = System.Text.RegularExpressions.RegexOptions;

namespace Indicium
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Converts a string representation of enum values in <see cref="RegexOptions"/> to its integral type.
        /// <para>Not case sensitive.</para>
        /// </summary>
        /// <param name="regexOptString"></param>
        /// <returns></returns>
        public static RegexOptions RegexOptionsFromString(this string regexOptString)
        {
            // this bit will remove the 'RegexOptions' prefix if it has it.
            var forComparison = regexOptString.ToLower();
            var regexOptionsName = $"{nameof(RegexOptions)}.".ToLower();

            if (forComparison.StartsWith(regexOptionsName)) {
                regexOptString = forComparison.Replace(regexOptionsName, string.Empty);
            }

            var caseInsensitive = regexOptString.ToLower();

            if (caseInsensitive == nameof(RegexOptions.Compiled).ToLower()) { return RegexOptions.Compiled; }
            if (caseInsensitive == nameof(RegexOptions.CultureInvariant).ToLower()) { return RegexOptions.CultureInvariant; }
            if (caseInsensitive == nameof(RegexOptions.ECMAScript).ToLower()) { return RegexOptions.ECMAScript; }
            if (caseInsensitive == nameof(RegexOptions.ExplicitCapture).ToLower()) { return RegexOptions.ExplicitCapture; }
            if (caseInsensitive == nameof(RegexOptions.IgnoreCase).ToLower()) { return RegexOptions.IgnoreCase; }
            if (caseInsensitive == nameof(RegexOptions.IgnorePatternWhitespace).ToLower()) { return RegexOptions.IgnorePatternWhitespace; }
            if (caseInsensitive == nameof(RegexOptions.Multiline).ToLower()) { return RegexOptions.Multiline; }
            if (caseInsensitive == nameof(RegexOptions.None).ToLower()) { return RegexOptions.None; }
            if (caseInsensitive == nameof(RegexOptions.RightToLeft).ToLower()) { return RegexOptions.RightToLeft; }
            if (caseInsensitive == nameof(RegexOptions.Singleline).ToLower()) { return RegexOptions.Singleline; }

            return RegexOptions.None;
        }

        /// <summary>
        /// Serialises a bunch of <see cref="Lexeme"/>s into an XML document.
        /// </summary>
        /// <param name="lexemes"></param>
        /// <param name="inputXmlPath"></param>
        /// <param name="textFilePath"></param>
        /// <returns></returns>
        public static XDocument ToXDocument(this IEnumerable<Lexeme> lexemes, string inputXmlPath = "", string textFilePath = "")
        {
            var content = lexemes.Select(l => {
                l.Untyped.Name = l.Untyped.Name.LocalName; // remove xmlns
                return l.Untyped;
            }).ToArray();

            // ReSharper disable once CoVariantArrayConversion
            var rootEl = new XElement(XName.Get("Lexemes"), content);
            if (inputXmlPath.IsNotEmpty()) rootEl.SetAttributeValue(XName.Get("Schema"), Path.GetFullPath(inputXmlPath));
            if (textFilePath.IsNotEmpty()) rootEl.SetAttributeValue(XName.Get("TextFile"), Path.GetFullPath(textFilePath));
            return new XDocument(rootEl);
        }

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