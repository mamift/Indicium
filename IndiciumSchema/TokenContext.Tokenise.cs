using System.Collections.Generic;
using System.Globalization;

namespace Indicium.Schemas
{
    public partial class TokenContext
    {
        private int _index = 0;

        private string _inputString = string.Empty;

        /// <summary>
        /// Not part of the XML model.
        /// <para>Setting this to false will result in lots of undefined tokens
        /// if there is no token definition for whitespace characters.</para>
        /// </summary>
        public bool IgnoreSpaces { get; set; }

        /// <summary>
        /// Determines if the tokeniser is processing at the start of the <see cref="InputString"/> or not.
        /// </summary>
        public bool IsAtStart => _index == 0;

        /// <summary>
        /// Set the input string to extract <see cref="Token"/>s from.
        /// </summary>
        public string InputString
        {
            set {
                _index = 0;
                _inputString = value;
            }
        }

        /// <summary>
        /// Get's the next <see cref="Token"/> for the current <see cref="InputString"/>.
        /// </summary>
        /// <returns></returns>
        public Token GetToken()
        {
            if (_index >= _inputString.Length) return default(Token);

            while ((_inputString[_index] == ' ' || _inputString[_index] == '\t') && IgnoreSpaces) {
                _index++;
                if (_index >= _inputString.Length) return default(Token);
            }

            foreach (var def in Token) {
                var regex = def.GetMatcher();
                var match = regex.Match(_inputString, _index);

                if (!match.Success || match.Index != _index) continue;

                if (match.Length == 0) continue;
                _index += match.Length;

                return new Token {
                    Id = def.Id,
                    TypedValue = match.Value,
                    LineIndex = _index
                };
            }

            _index++;
            return new Token {
                Id = "Undefined",
                TypedValue = _inputString[_index - 1].ToString(CultureInfo.InvariantCulture)
            };
        }

        /// <summary>
        /// Get all <see cref="Token"/>s for the current <see cref="InputString"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Token> GetTokens()
        {
            var token = GetToken();

            while (token != default(Token))
            {
                yield return token;
                token = GetToken();
            }
        }
    }
}