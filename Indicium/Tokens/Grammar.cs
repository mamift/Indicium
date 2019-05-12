using System;
using System.IO;
using System.Linq;

namespace Indicium.Tokens
{
    public struct Grammar
    {
        public Line[] Lines { get; }

        public Tokeniser Tokeniser { get; }

        public Grammar(string[] lines) : this(lines.Select(str => new Line(str)).ToArray()) { }

        private Grammar(Line[] lines)
        {
            Tokeniser = new Tokeniser();
            Lines = lines;
            Process(Lines);
        }

        /// <summary>
        /// Process the grammar by parsing grammar text line by <see cref="Line"/>.
        /// </summary>
        /// <param name="tokenGrammarLines"></param>
        public void Process(Line[] tokenGrammarLines)
        {
            foreach (var line in tokenGrammarLines) {
                if (string.IsNullOrWhiteSpace(line.Text) || line.Text.StartsWith("#")) continue;

                var regex = string.Empty;
                var prevChar = '\0';
                var prevPrevChar = '\0';
                var quoteNum = 0;

                var ndx = 0;

                foreach (var c in line.Text) {
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

                var identifier = line.Text.Remove(0, ndx).Trim();

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

                var retval = Tokeniser.DefineToken(regex, identifier);
                if (retval != string.Empty) throw new Exception("Error!");
            }
        }

        /// <summary>
        /// Instantiates a new instance using a given <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Grammar Load(string filePath)
        {
            var fileLines = File.ReadAllLines(filePath);

            return new Grammar(fileLines);
        }
    }
}