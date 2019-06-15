using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Indicium.Schemas
{
    public partial class TokenContext
    {
        private int _index = 0;

        private int _lineNumber = 0;

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
        /// Gets the current index of the <see cref="InputString"/>. If this is 0, then <see cref="IsAtStart"/>
        /// is <c>true</c>.
        /// </summary>
        public int LineColumnIndex => _index;

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
        /// When processing input strings line by line, set this value to indicate which number line,
        /// so that tokenised output produced can refer to it. <para>Because this value is set by API
        /// callers, the caller should communicate whether tokenised output (<see cref="TokenValue"/> objects)
        /// have their <see cref="TokenValue.LineNumber"/> set as a 0-based or 1-based value.</para>
        /// </summary>
        public int LineNumber
        {
            set => _lineNumber = value;
        }

        /// <summary>
        /// Resets the current <see cref="InputString"/> to null, and the default setting for <see cref="IgnoreSpaces"/>.
        /// </summary>
        public void Reset()
        {
            InputString = null;
            IgnoreSpaces = false;
        }

        /// <summary>
        /// Get's the next <see cref="Token"/> for the current <see cref="InputString"/>.
        /// </summary>
        /// <returns></returns>
        public TokenValue GetToken()
        {
            if (_index >= _inputString.Length) return default(TokenValue);

            while ((_inputString[_index] == ' ' || _inputString[_index] == '\t') && IgnoreSpaces) {
                _index++;
                if (_index >= _inputString.Length) return default(TokenValue);
            }

            foreach (var def in Token) {
                var regex = def.GetMatcher();
                var match = regex.Match(_inputString, _index);

                if (!match.Success || match.Index != _index) continue;

                if (match.Length == 0) continue;
                _index += match.Length;

                return new TokenValue {
                    Id = def.Id,
                    Value = match.Value,
                    LineIndex = _index,
                    LineNumber = _lineNumber
                };
            }

            _index++;
            return new TokenValue {
                Id = "Undefined",
                Value = _inputString[_index - 1].ToString(CultureInfo.InvariantCulture),
                LineIndex = _index,
                LineNumber = _lineNumber
            };
        }

        /// <summary>
        /// Get all <see cref="Token"/>s for the current <see cref="InputString"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TokenValue> GetTokens()
        {
            var token = GetToken();

            while (token != default(TokenValue)) {
                yield return token;
                token = GetToken();
            }
        }

        public IEnumerable<TokenValue> ProcessTokens(TextReader reader)
        {
            string line;
            int lineCount = 0;
            while ((line = reader.ReadLine()) != null) {
                InputString = line;
                LineNumber = lineCount;

                var token = GetToken();
                while (token != default(TokenValue)) {
                    yield return token;
                    token = GetToken();
                }

                lineCount++;
            }
        }
    }
}