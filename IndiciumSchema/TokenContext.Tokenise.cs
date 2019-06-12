using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Indicium.Schemas
{
    public partial class TokenContext
    {
        public List<Token> GetTokens(TextReader textReader, bool ignoreWhiteSpace = false)
        {
            var tokenDefinitions = Token.ToList();
            string line;

            var stringBuilder = new StringBuilder();

            var tokenList = new List<Token>();

            while ((line = textReader.ReadLine()) != null) {
                var index = 0;

                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!ignoreWhiteSpace) {
                    while (char.IsWhiteSpace(line[index])) {
                        // ignore whitespace
                        index++;
                    }
                }

                while (index <= line.Length) {
                    bool didMatch = false;
                    foreach (var definedToken in tokenDefinitions) {
                        var regex = definedToken.GetMatcher();
                        var match = regex.Match(line, index);

                        // because we're iterating over all the token defs,
                        // a regex might match something but from the wrong starting index.
                        var matchIndex = match.Index != index;
                        if (!match.Success || matchIndex) {
                            continue; // continue searching
                        }

                        var token = new Token {
                            TypedValue = $"{{{definedToken.Id} = '{match.Value}'}}"
                        };
                        tokenList.Add(token);
                        index += match.Length - 1;
                        didMatch = true;
                        Debug.WriteLine($"{token.TypedValue} @ {index}");
                    }

                    /*var token = GetToken(line, ref index);
                    tokenList.Add(token);*/
                    index++; // continue searching
                }

                stringBuilder.Append(line);
            }

            Debug.WriteLine($"{tokenList.Count} tokens.");
            return tokenList;
        }

        public Token GetToken(ref int index, string inputString, bool ignoreSpaces)
        {
            if (index >= inputString.Length) return default(Token);

            while ((inputString[index] == ' ' || inputString[index] == '\t') && ignoreSpaces) {
                index++;
                if (index >= inputString.Length) return default(Token);
            }

            foreach (var pair in Token) {
                var regex = pair.GetMatcher();
                var match = regex.Match(inputString, index);

                if (!match.Success || match.Index != index) continue;

                if (match.Length == 0)
                    continue;
                index += match.Length;
                return new Token {
                    Id = pair.Id,
                    TypedValue = match.Value
                };
            }

            index++;
            return new Token {
                Id = "Undefined",
                TypedValue = inputString[index - 1].ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}