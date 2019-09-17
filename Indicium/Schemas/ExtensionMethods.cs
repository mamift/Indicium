using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Indicium.Schemas
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns a <see cref="Lexeme"/> from a given <paramref name="input"/> string. 
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="input">Extract a <see cref="Lexeme"/> from the given string.</param>
        /// <param name="startIndex">The starting index to begin extracting a <see cref="Lexeme"/> from the <paramref name="input"/>.</param>
        /// <param name="ignoreSpaces">Ignores whitespace chars (space and tab) when attempting to extract.</param>
        /// <param name="endIndex">After extracting a <see cref="Lexeme"/>, save the 0-based index of it from the <see cref="input"/>. If it's
        /// not an undefined Lexeme it should be higher than <paramref name="startIndex"/>.</param>
        /// <param name="matchLength">After extracting a <see cref="Lexeme"/>, save the length of the part of the string that forms it.</param>
        /// <param name="spaceCharacters">Provide custom array of <c>char</c>s to define what is a whitespace character. Defaults to space and tab.
        /// NOTE: Explicitly passing <c>null</c> will still default to \t and \s - instead pass an empty array to never ignore any whitespace chars.</param>
        /// <returns>The returned <see cref="Lexeme"/> will not include a <see cref="Lexeme.LineNumber"/> value. This value should be set after
        /// the method returns.</returns>
        public static LexemeBase<TTokenBase> ExtractLexeme<TTokenBase, TLexeme>(this IEnumerable<TokenBase> tokens, string input,
            int startIndex, bool ignoreSpaces, out int endIndex, out int matchLength, char[] spaceCharacters = null)
            where TTokenBase: TokenBase
            where TLexeme: LexemeBase<TTokenBase>, new()
        {
            if (spaceCharacters == null) spaceCharacters = new[] {' ', '\t'};

            endIndex = startIndex; // begin at given index of string
            matchLength = 0; // will reset to zero for each invocation of this method
            if (endIndex >= input.Length) return default(TLexeme); // then we're at the end of the string, and can return;

            if (spaceCharacters.Any()) {
                // if ignore spaces is true and there are space chars provided
                while (input[endIndex].IsOneOf(spaceCharacters) && ignoreSpaces) {
                    endIndex++;
                    if (endIndex >= input.Length) return default(TLexeme);
                }
            }

            foreach (var def in tokens) {
                if (def.GetLexeme<TTokenBase, TLexeme>(input, ref endIndex, ref matchLength, out var lexeme)) {
                    return lexeme;
                }
            }

            endIndex++;
            return new TLexeme {
                Value = input[endIndex - 1].ToString(CultureInfo.InvariantCulture),
                LineIndex = endIndex - matchLength
            };
        }
        
        public static LexemeBase<TokenBase> ExtractLexeme(this IEnumerable<TokenBase> tokens, string input,
            int startIndex, bool ignoreSpaces, out int endIndex, out int matchLength, char[] spaceCharacters = null)
        {
            if (spaceCharacters == null) spaceCharacters = new[] {' ', '\t'};

            endIndex = startIndex; // begin at given index of string
            matchLength = 0; // will reset to zero for each invocation of this method
            if (endIndex >= input.Length) return default(LexemeBase<TokenBase>); // then we're at the end of the string, and can return;

            if (spaceCharacters.Any()) {
                // if ignore spaces is true and there are space chars provided
                while (input[endIndex].IsOneOf(spaceCharacters) && ignoreSpaces) {
                    endIndex++;
                    if (endIndex >= input.Length) return default(LexemeBase<TokenBase>);
                }
            }

            foreach (var def in tokens) {
                if (def.GetLexeme(input, ref endIndex, ref matchLength, out var lexeme)) {
                    return lexeme;
                }
            }

            endIndex++;
            return new LexemeBase<TokenBase>(UndefinedToken.Default) {
                Value = input[endIndex - 1].ToString(CultureInfo.InvariantCulture),
                LineIndex = matchLength > 0 ? (endIndex - matchLength) : startIndex
            };
        }

        private static bool GetLexeme<TTokenType, TLexeme>(string input, ref int index, ref int matchLength, TokenBase def, out LexemeBase<TTokenType> lexeme)
            where TTokenType : TokenBase, new()
            where TLexeme : LexemeBase<TTokenType>, new()
        {
            var regex = def.Regex;
            var match = regex.Match(input, index);

            lexeme = default(LexemeBase<TTokenType>);

            if (!match.Success || match.Index != index) return false;
            if (match.Length == 0) return false;

            index += match.Length;
            matchLength = match.Length;

            lexeme = new TLexeme {
                Value = match.Value,
                LineIndex = index - matchLength
            };

            return true;
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
    }
}