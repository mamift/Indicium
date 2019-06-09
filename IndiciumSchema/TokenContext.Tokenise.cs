using System.Collections.Generic;
using System.Diagnostics;
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

        protected Token GetToken(string line, ref int index)
        {
            var tokenDefinitions = Token.ToList();
            Token theToken = null;
            foreach (var definedToken in tokenDefinitions) {
                var regex = definedToken.GetMatcher();
                var substr = line.Substring(index);
                var match = regex.Match(substr);

                // because we're iterating over all the token defs,
                // a regex might match something but from the wrong starting index.
                var matchIndex = match.Index != index;
                if (!match.Success || matchIndex) {
                    continue; // continue searching
                }

                var token = new Token {
                    TypedValue = $"{{{definedToken.Id} = '{match.Value}'}}"
                };

                index += match.Length - 1;
                Debug.WriteLine($"{token.TypedValue} @ {index}");
                theToken = token;
                break;
            }

            return theToken;
        }
    }
}