using System.Collections.Generic;
using System.Globalization;

namespace Indicium.Schemas
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Stateless extraction of a <see cref="Lexeme"/> from a given <paramref name="input"/> string.
        /// <para>This is an extension method to guarantee it does not modify state of the <see cref="TokenContext"/> class.</para>
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="input">Extract a <see cref="Lexeme"/> from the given string.</param>
        /// <param name="inputIndex">The starting index to begin extracting a <see cref="Lexeme"/> from the <paramref name="input"/>.</param>
        /// <param name="ignoreSpaces">Ignores whitespaces when attempting to extract.</param>
        /// <param name="index">After extracting a <see cref="Lexeme"/>, save the 0-based index of it from the <see cref="input"/>. If it's
        /// not an undefined Lexeme it should be higher than <paramref name="inputIndex"/>.</param>
        /// <param name="matchLength">After extracting a <see cref="Lexeme"/>, save the length of the part of the string that forms it.</param>
        /// <returns></returns>
        public static Lexeme ExtractLexeme(this IEnumerable<Token> tokens, string input,
            int inputIndex, bool ignoreSpaces, out int index, out int matchLength)
        {
            index = inputIndex;
            matchLength = 0;
            if (index >= input.Length) return default(Lexeme);

            while ((input[index] == ' ' || input[index] == '\t') && ignoreSpaces) {
                index++;
                if (index >= input.Length) return default(Lexeme);
            }

            foreach (var def in tokens) {
                var regex = def.GetMatcher();
                var match = regex.Match(input, index);

                if (!match.Success || match.Index != index) continue;
                if (match.Length == 0) continue;

                index += match.Length;
                matchLength = match.Length;

                return new Lexeme {
                    Id = def.Id,
                    Value = match.Value,
                    LineIndex = index - matchLength
                };
            }

            index++;
            return new Lexeme {
                Id = "Undefined",
                Value = input[index - 1].ToString(CultureInfo.InvariantCulture),
                LineIndex = index - matchLength
            };
        }
    }
}