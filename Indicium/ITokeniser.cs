using System.Collections.Generic;
using System.IO;

namespace Indicium
{
    public interface ITokeniser<out TToken>
        where TToken: class, new()
    {
        /// <summary>
        /// Determines if the tokeniser is processing at the start of the <see cref="InputString"/> or not.
        /// </summary>
        bool IsAtStart { get; }

        /// <summary>
        /// Gets the current index of the <see cref="InputString"/>. If this is 0, then <see cref="IsAtStart"/>
        /// is <c>true</c>.
        /// </summary>
        int LineIndex { get; }

        /// <summary>
        /// <para>Set the input string to extract <see cref="Token"/>s from.</para>
        /// </summary>
        string InputString { set; }

        /// <summary>
        /// When processing input strings line by line, set this value to indicate which line number we are
        /// currently processing, so that output <see cref="Lexeme"/>s can refer to it.
        /// <para>Because this value is set by API callers, the caller should communicate whether output <see cref="Lexeme"/> objects
        /// have their <see cref="Lexeme.LineNumber"/> set as a 0-based or 1-based value. Line numbers should start
        /// at one, but column indices should start at 0.</para>
        /// <para>This value is also used by <see cref="ProcessTokens(TextReader)"/> and for <see cref="ProcessTokens(string,char)"/> methods,
        /// whereby the value of this property .</para>
        /// <para>Defaults to 1.</para>
        /// </summary>
        int LineNumber { set; }

        /// <summary>
        /// Resets the current <see cref="TokenContext.InputString"/> to null, internal line index (column) and line number
        /// tracking values are also reset.
        /// </summary>
        void Reset();

        /// <summary>
        /// Get's the next <see cref="TokenContext.Token"/> for the current <see cref="TokenContext.InputString"/>.
        /// </summary>
        /// <returns></returns>
        TToken GetToken();

        /// <summary>
        /// Returns the next <see cref="Lexeme"/> that would be next, without incrementing values for
        /// <see cref="TokenContext.LineIndex"/>. Obeys <see cref="TokenContext.IgnoreWhitespace"/>.
        /// <para>Calling this method first, then calling <see cref="TokenContext.GetToken"/> should produce equal, but not identical 
        /// instances of <see cref="Lexeme"/>s (as in they will not be references to the same instance).</para>
        /// </summary>
        /// <returns></returns>
        TToken PeekToken();

        /// <summary>
        /// Get all <see cref="TokenContext.Token"/>s for the current <see cref="TokenContext.InputString"/>.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TToken> GetTokens();

        /// <summary>
        /// Process a single line of text. If the string contains a return carriage or new line character,
        /// then anything after that is ignored.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        IEnumerable<TToken> ProcessLine(string line, int lineNumber);

        /// <summary>
        /// Process the text from a given <see cref="TextReader"/> (<paramref name="reader"/>)
        /// and produce tokenised output. 
        /// <para>This method usually suffices for processing arbitrary text. Finer control can be achieved using a combination of
        /// <see cref="TokenContext.InputString"/>, <see cref="TokenContext.LineNumber"/>, <see cref="TokenContext.LineIndex"/>, <see cref="TokenContext.Reset"/>, <see cref="TokenContext.GetTokens"/> and <see cref="TokenContext.GetToken"/>.</para>
        /// <para>This method calls <see cref="TokenContext.Reset"/>, but does not reset the value for <see cref="TokenContext.IgnoreWhitespace"/>.</para>
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        IEnumerable<TToken> ProcessTokens(TextReader reader);

        /// <summary>
        /// Process the text from a given <see cref="string"/>, with an optional line <paramref name="delimiter"/>
        /// and produce tokenised output.
        /// <para>This method behaves identically to <see cref="TokenContext.ProcessTokens(System.IO.TextReader)"/>, but accepts a <see cref="string"/>
        /// instead of a <see cref="TextReader"/>.</para>
        /// </summary>
        /// <param name="string"></param>
        /// <param name="delimiter">Defaults to new line. To not split the string by anything, pass <c>default(char)</c></param>
        /// <returns></returns>
        IEnumerable<TToken> ProcessTokens(string @string, char delimiter = '\n');
    }
}