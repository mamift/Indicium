using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Indicium.Tokens
{
    /// <summary>
    /// Instantiates a class that can process tokens from an <see cref="InputString"/>.
    /// <para>Requires that tokens be defined first via <see cref="DefineToken"/>.</para>
    /// </summary>
    public class Tokeniser
    {
        private readonly Dictionary<string, string> _tokensDefinitionsDict;
        private string _inputString;
        private int _index;

        /// <summary>
        /// Returns a list of all <see cref="TokenDefinition"/> that were defined via <see cref="DefineToken"/>.
        /// </summary>
        /// <returns></returns>
        public List<TokenDefinition> TokenDefinitions => 
            _tokensDefinitionsDict.Select(kvp => new TokenDefinition(kvp.Key, kvp.Value)).ToList();

        /// <summary>
        /// Set to <c>true</c> to ignore spaces.
        /// <para>This value can be switched on or off, after setting a value
        /// for <see cref="InputString"/>. The value set is valid for the next invocation of <see cref="DefineToken"/>,
        /// <see cref="GetToken"/> or <see cref="GetTokens"/>.</para>
        /// </summary>
        public bool IgnoreSpaces { get; set; }

        public bool IsAtStart => _index == 0;

        /// <summary>
        /// Set this value, then use <see cref="GetToken"/> to retrieve a token.
        /// </summary>
        public string InputString
        {
            set => _inputString = value;
        }

        public Tokeniser()
        {
            _tokensDefinitionsDict = new Dictionary<string, string>();
            _index = 0;
            _inputString = string.Empty;
            IgnoreSpaces = false;
        }

        /// <summary>
        /// Reset this <see cref="Tokeniser"/> instance.
        /// </summary>
        public void Reset()
        {
            _tokensDefinitionsDict.Clear();
            _inputString = string.Empty;
            _index = 0;
        }

        /// <summary>
        /// Creates a new token definition using a given <paramref name="regEx"/> string and an
        /// <paramref name="identifier"/>.
        /// <para>Returns an empty string if the attempt was successful, otherwise an error message.</para>
        /// </summary>
        /// <param name="regEx"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public string DefineToken(string regEx, string identifier)
        {
            var trimmedId = identifier.Trim();
            if (_tokensDefinitionsDict.ContainsKey(trimmedId))
                return "";

            identifier = trimmedId;
            if (!IsValidRegex(regEx)) {
                return GetInvalidMessage(regEx);
            }
            _tokensDefinitionsDict.Add(identifier, regEx);

            return "";
        }

        /// <summary>
        /// Assuming a given <paramref name="regEx"/> string is invalid, returns the error message
        /// that indicates why it is invalid. If it's valid, then it returns <see cref="string.Empty"/>.
        /// </summary>
        /// <param name="regEx"></param>
        /// <returns></returns>
        private string GetInvalidMessage(string regEx)
        {
            try {
                var r = new Regex(regEx);
                // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                r.Match("");
                // ReSharper restore ReturnValueOfPureMethodIsNotUsed
            } catch (Exception e) {
                return e.Message;
            }

            return string.Empty;
        }

        /// <summary>
        /// Checks that a given <paramref name="regEx"/> string is valid.
        /// </summary>
        /// <param name="regEx"></param>
        /// <returns></returns>
        private bool IsValidRegex(string regEx)
        {
            try {
                var r = new Regex(regEx);
                // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                r.Match("");
                // ReSharper restore ReturnValueOfPureMethodIsNotUsed
            } catch (Exception) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a <see cref="Token"/> from the <see cref="InputString"/>.
        /// <para>Invoke this method after setting a value for <see cref="InputString"/>.</para>
        /// </summary>
        /// <returns></returns>
        public Token GetToken()
        {
            if (_index >= _inputString.Length) return default(Token);

            while ((_inputString[_index] == ' ' || _inputString[_index] == '\t') && IgnoreSpaces)
            {
                _index++;
                if (_index >= _inputString.Length) return default(Token);
            }
            
            foreach (var pair in TokenDefinitions) {
                var regex = pair.Regex;
                var match = regex.Match(_inputString, _index);

                if (!match.Success || match.Index != _index) continue;

                if (match.Length == 0)
                    continue;
                _index += match.Length;
                return new Token(pair.Identifier, match.Value);
            }
            _index++;
            return new Token("Undefined", _inputString[_index - 1].ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Returns all tokens from the <see cref="InputString"/>.
        /// <para>Invoke this method after setting a value for <see cref="InputString"/>.</para>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Token> GetTokens()
        {
            var token = GetToken();

            while (token != default(Token)) {
                yield return token;
                token = GetToken();
            }
        }
    }
}
