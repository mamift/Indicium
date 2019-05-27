using System;
using System.IO;
using System.Linq;

namespace Indicium.Tokens
{
    public class TokenProcessor
    {
        public Tokeniser Tokeniser { get; } = new Tokeniser();

        /// <summary>
        /// Instantiates a new instance using the limes read from a token definition file.
        /// </summary>
        /// <param name="tokenDefinitionLines"></param>
        public TokenProcessor(string[] tokenDefinitionLines)
        {
            Process(tokenDefinitionLines, Tokeniser);
        }

        /// <summary>
        /// Instantiates a new instance using the file path to a token definition file.
        /// </summary>
        /// <param name="tokenDefinitionFilePath"></param>
        public TokenProcessor(string tokenDefinitionFilePath)
        {
            Process(File.ReadAllLines(tokenDefinitionFilePath), Tokeniser);
        }

        /// <summary>
        /// Process a token definition file by reading the definition text line by.
        /// </summary>
        /// <param name="tokenDefinitionLines"></param>
        /// <param name="tokeniser"></param>
        internal static void Process(string[] tokenDefinitionLines, Tokeniser tokeniser)
        {
            foreach (var line in tokenDefinitionLines) {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                var regex = string.Empty;
                var prevChar = '\0';
                var prevPrevChar = '\0';
                var quoteNum = 0;

                var ndx = 0;

                foreach (var c in line) {
                    if (c == '\"') {
                        quoteNum++;

                        if (quoteNum > 1 && prevChar == '\\' && prevPrevChar == '\\') {
                            regex += c;
                            ndx++;
                            break;
                        }

                        if (quoteNum > 1 && prevChar != '\\') {
                            regex += c;
                            ndx++;
                            break;
                        }
                    }

                    regex += c;
                    ndx++;
                    prevPrevChar = prevChar;
                    prevChar = c;
                }

                var identifier = line.Remove(0, ndx).Trim();

                var ndxComment = identifier.Length - 1;

                while (ndxComment > 0) {
                    if (identifier[ndxComment] == '#')
                        break;
                    ndxComment--;
                }

                if (ndxComment > 0) {
                    identifier = identifier.Substring(0, ndxComment);
                    identifier = identifier.Trim();
                }

                regex = regex.Substring(1, regex.Length - 1);
                regex = regex.Substring(0, regex.Length - 1);
                regex = regex.Replace("\\\"", "\"");

                if (regex.Equals(string.Empty))
                    continue;

                var retval = tokeniser.DefineToken(regex, identifier);
                if (retval != string.Empty) throw new Exception("Error!");
            }
        }

        /// <summary>
        /// Instantiates a new instance using a given <paramref name="filePath"/>, and ensures the file is
        /// trimmed of excess whitespace.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static TokenProcessor Load(string filePath)
        {
            var fileLines = File.ReadAllLines(filePath);
            var trimmedLines = fileLines.Select(line => line.Trim()).ToArray();

            return new TokenProcessor(trimmedLines);
        }
    }
}