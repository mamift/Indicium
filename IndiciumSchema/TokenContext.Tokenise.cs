using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public int LineIndex => _index;

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
        /// callers, the caller should communicate whether tokenised output (<see cref="Lexeme"/> objects)
        /// have their <see cref="Lexeme.LineNumber"/> set as a 0-based or 1-based value.</para>
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
            _lineNumber = 0;
        }

        /// <summary>
        /// Get's the next <see cref="Token"/> for the current <see cref="InputString"/>.
        /// </summary>
        /// <returns></returns>
        public Lexeme GetToken()
        {
            var lexeme = Token.ExtractLexeme(_inputString, _index, IgnoreSpaces, out _index, out var matchLength);
            if (lexeme == default(Lexeme)) return null;

            lexeme.LineIndex = _index - matchLength;
            lexeme.LineNumber = _lineNumber;

            return lexeme;
        }

        /// <summary>
        /// Returns the next <see cref="Lexeme"/> that would be next processed, without processing
        /// <see cref="LineIndex"/>. Obeys <see cref="IgnoreSpaces"/>.
        /// <para>Calling this method first, then calling <see cref="GetToken"/> should produce equal (but not identical)
        /// instances of <see cref="Lexeme"/>s.</para>
        /// </summary>
        /// <returns></returns>
        public Lexeme PeekToken()
        {
            var startIndexCopy = _index;
            var subStr = _inputString.Substring(startIndexCopy);

            var lexeme = Token.ExtractLexeme(subStr, startIndexCopy, IgnoreSpaces, out var indexPostExtract,
                out var matchLength);
            
            lexeme.LineIndex = indexPostExtract - matchLength;
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
        /// Process the text from a given <see cref="TextReader"/> <paramref name="reader"/>
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
            int lineCount = 1;
            while ((line = reader.ReadLine()) != null) {
                InputString = line;
                LineNumber = lineCount;

                var token = GetToken();
                while (token != default(Lexeme)) {
                    yield return token;
                    token = GetToken();
                }

                lineCount++;
            }
        }
    }
}