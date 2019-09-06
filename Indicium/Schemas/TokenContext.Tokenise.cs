using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Indicium.Schemas
{
    public partial class TokenContext
    {
        private int _index = 0;

        private int _lineNumber = 1;

        private string _inputString = string.Empty;

        /// <summary>
        /// Not serialised.
        /// <para>Set to ignore or obey a Token's <see cref="Schemas.Token.EvaluationOrder"/> setting.</para>
        /// </summary>
        public bool ObeyEvaluationOrder = true;

        /// <summary>
        /// <para>Default <see cref="System.Text.RegularExpressions.RegexOptions"/> when creating <see cref="Schemas.Token"/>
        /// instances.</para>
        /// <para>This is a static field instance and for now cannot be serialised to XML.</para>
        /// </summary>
        public static RegexOptions RegexOptions = RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline;

        /// <summary>
        /// Not serialised.
        /// <para>Setting this to false will result in lots of undefined tokens
        /// if there is no token definition for whitespace characters.</para>
        /// <para>Conversely, setting this to true while there are token definitions for whitespace characters
        /// may result in the lexeme count being much less than expected.</para>
        /// <para>This is always <c>false</c> by default and the setting is not saved.</para>
        /// </summary>
        public bool IgnoreSpaces;

        /// <summary>
        /// Determines if the tokeniser is processing at the start of the <see cref="InputString"/> or not.
        /// </summary>
        public bool IsAtStart => _index == 0;

        /// <summary>
        /// Gets the current index of the <see cref="InputString"/>. If this is 0, then <see cref="IsAtStart"/>
        /// is <c>true</c>.
        /// </summary>
        public int LineIndex => _index;

        /// <summary>
        /// Not serialised.
        /// <para>Set the input string to extract <see cref="Token"/>s from.</para>
        /// </summary>
        public string InputString
        {
            set {
                _index = 0;
                _inputString = value;
            }
        }

        /// <summary>
        /// When processing input strings line by line, set this value to indicate which line number we are
        /// currently processing, so that tokenised output can refer to it. <para>Because this value is set by API
        /// callers, the caller should communicate whether tokenised output (<see cref="Lexeme"/> objects)
        /// have their <see cref="Lexeme.LineNumber"/> set as a 0-based or 1-based value.</para>
        /// <para>This value is ignored for <see cref="ProcessTokens(TextReader)"/> and for <see cref="ProcessTokens(string,char)"/> methods,
        /// as the line number is determined by the input.</para>
        /// <para>Defaults to 1.</para>
        /// </summary>
        public int LineNumber
        {
            set => _lineNumber = value;
        }

        /// <summary>
        /// Resets the current <see cref="InputString"/> to null, internal line index (column) and line number
        /// tracking values are also reset.
        /// </summary>
        public void Reset()
        {
            InputString = null;
            _index = 0;
            _lineNumber = 1;
        }

        /// <summary>
        /// Get's the next <see cref="Token"/> for the current <see cref="InputString"/>.
        /// </summary>
        /// <returns></returns>
        public Lexeme GetToken()
        {
            var lexeme = Token.ExtractLexeme(_inputString, _index, IgnoreSpaces, out _index, out var matchLength);
            if (lexeme == default(Lexeme)) return null;

            lexeme.LineNumber = _lineNumber;

            return lexeme;
        }

        /// <summary>
        /// Returns the next <see cref="Lexeme"/> that would be next, without incrementing values for
        /// <see cref="LineIndex"/>. Obeys <see cref="IgnoreSpaces"/>.
        /// <para>Calling this method first, then calling <see cref="GetToken"/> should produce equal, but not identical 
        /// instances of <see cref="Lexeme"/>s (as in they will not be references to the same instance).</para>
        /// </summary>
        /// <returns></returns>
        public Lexeme PeekToken()
        {
            var startIndexCopy = _index;
            var subStr = _inputString.Substring(startIndexCopy);

            var lexeme = Token.ExtractLexeme(subStr, startIndexCopy, IgnoreSpaces, out _, out _, ObeyEvaluationOrder);
            
            lexeme.LineNumber = _lineNumber;

            return lexeme;
        }

        /// <summary>
        /// Get all <see cref="Token"/>s for the current <see cref="InputString"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Lexeme> GetTokens()
        {
            var token = GetToken();

            while (token != default(Lexeme)) {
                yield return token;
                token = GetToken();
            }
        }

        /// <summary>
        /// Process a single line of text. If the string contains a return carriage or new line character,
        /// then anything after that is ignored.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        public IEnumerable<Lexeme> ProcessLine(string line, int lineNumber)
        {
            InputString = line;
            LineNumber = lineNumber;

            var token = GetToken();
            while (token != default(Lexeme))
            {
                yield return token;
                token = GetToken();
            }
        }

        /// <summary>
        /// Process the text from a given <see cref="TextReader"/> (<paramref name="reader"/>)
        /// and produce tokenised output. 
        /// <para>This method usually suffices for processing arbitrary text. Finer control can be achieved using a combination of
        /// <see cref="InputString"/>, <see cref="LineNumber"/>, <see cref="LineIndex"/>, <see cref="Reset"/>, <see cref="GetTokens"/> and <see cref="GetToken"/>.</para>
        /// <para>This method calls <see cref="Reset"/>, but does not reset the value for <see cref="IgnoreSpaces"/>.</para>
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IEnumerable<Lexeme> ProcessTokens(TextReader reader)
        {
            Reset();
            string line;
            var lineCount = 1;
            while ((line = reader.ReadLine()) != null) {
                foreach (var token in ProcessLine(line, lineCount))
                    yield return token;

                lineCount++;
            }
        }

        /// <summary>
        /// Process the text from a given <see cref="string"/>, with an optional line <paramref name="delimiter"/>
        /// and produce tokenised output.
        /// <para>This method behaves identically to <see cref="ProcessTokens(TextReader)"/>, but accepts a <see cref="string"/>
        /// instead of a <see cref="TextReader"/>.</para>
        /// </summary>
        /// <param name="string"></param>
        /// <param name="delimiter">Defaults to new line. To not split the string by anything, pass <c>default(char)</c></param>
        /// <returns></returns>
        public IEnumerable<Lexeme> ProcessTokens(string @string, char delimiter = '\n')
        {
            Reset();
            var lineCount = 1;
            var splitStrings = delimiter != default(char) ? @string.Split(delimiter) : new [] { @string };
            foreach (var line in splitStrings) {
                foreach (var token in ProcessLine(line, lineCount))
                    yield return token;

                lineCount++;
            }
        }
    }
}