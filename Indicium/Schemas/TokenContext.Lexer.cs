using System.Globalization;

namespace Indicium.Schemas
{
    using Text = System.Text.RegularExpressions;

    public partial class TokenContext
    {
        /// <summary>
        /// Returns a <see cref="Lexeme"/> from a given <paramref name="input"/> string. 
        /// <para>This is an extension method to guarantee it does not modify state of the <see cref="TokenContext"/> class.</para>
        /// </summary>
        /// <param name="input">Extract a <see cref="Lexeme"/> from the given string.</param>
        /// <param name="inputIndex">The starting index to begin extracting a <see cref="Lexeme"/> from the <paramref name="input"/>.</param>
        /// <param name="ignoreSpaces">Ignores whitespace chars (space and tab) when attempting to extract.</param>
        /// <param name="index">After extracting a <see cref="Lexeme"/>, save the 0-based index of it from the <see cref="input"/>. If it's
        /// not an undefined Lexeme it should be higher than <paramref name="inputIndex"/>.</param>
        /// <param name="matchLength">After extracting a <see cref="Lexeme"/>, save the length of the part of the string that forms it.</param>
        /// <param name="spaceCharacters">Provide custom array of <c>char</c>s to define what is a whitespace character. Defaults to space and tab.
        /// NOTE: Explicitly passing <c>null</c> will still default to \t and \s - instead pass an empty array to never ignore any whitespace chars.</param>
        /// <returns>The returned <see cref="Lexeme"/> will not include a <see cref="Lexeme.LineNumber"/> value. This value should be set after
        /// the method returns.</returns>
        public Lexeme ExtractLexeme(string input, int inputIndex, bool ignoreSpaces, out int index, out int matchLength,
            Text.Regex spaceCharacters = null)
        {
            if (spaceCharacters == null) spaceCharacters = new Text.Regex(@"\t|\s", Text.RegexOptions.Compiled);

            index = inputIndex; // begin at given index of string
            matchLength = 0; // will reset to zero for each invocation of this method
            if (index >= input.Length) return default(Lexeme); // then we're at the end of the string, and can return;

            if (ignoreSpaces) {
                // if ignore spaces is true and there are space chars provided
                while (spaceCharacters.Matches(input[index].ToString()).Count > 0) {
                    index++;
                    if (index >= input.Length) return default(Lexeme);
                }
            }
            
            foreach (var def in Token) {
                if (GetLexeme(input, ref index, ref matchLength, def, out var lexeme, LoadedRegexOptions)) return lexeme;
            }

            index++;
            return new Lexeme {
                Id = Lexeme.Undefined.Id,
                TypedValue = input[index - 1].ToString(CultureInfo.InvariantCulture),
                LineIndex = index - matchLength
            };
        }

        /// <summary>
        /// Extracts a <see cref="Lexeme"/> from the <paramref name="input"/> string, saving the <paramref name="index"/>,
        /// <paramref name="matchLength"/>, the resultant <paramref name="lexeme"/>, using a given
        /// <see cref="Indicium.Schemas.Token"/> definition, and optional <see cref="Text.RegexOptions"/>.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="index"></param>
        /// <param name="matchLength"></param>
        /// <param name="def"></param>
        /// <param name="lexeme"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        private bool GetLexeme(string input, ref int index, ref int matchLength, Token def, out Lexeme lexeme, 
            Text.RegexOptions opts = Text.RegexOptions.Compiled)
        {
            var regex = def.GetMatcher(opts);
            var match = regex.Match(input, index);

            lexeme = Lexeme.Undefined;

            if (!match.Success || match.Index != index) return false;
            if (match.Length == 0) return false;

            index += match.Length;
            matchLength = match.Length;

            lexeme = new Lexeme {
                Id = def.Id,
                TypedValue = match.Value,
                LineIndex = index - matchLength
            };
            return true;
        }
    }
}