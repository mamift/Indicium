using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using RegexOptions = System.Text.RegularExpressions.RegexOptions;

namespace Indicium
{
    public partial class XsdTokeniser
    {
        private readonly XmlSchema _schema;

        public XsdTokeniser(XmlSchema schema)
        {
            _schema = schema;
        }

        public object ExtractLexeme(string input, int inputIndex, bool ignoreSpaces, out int index, 
            out int matchLength, Regex spaceCharacters = null)
        {
            if (spaceCharacters == null) spaceCharacters = new Regex(@"\t|\s", RegexOptions.Compiled);

            index = inputIndex; // begin at given index of string
            matchLength = 0; // will reset to zero for each invocation of this method
            if (index >= input.Length) return default(object); // then we're at the end of the string, and can return;

            if (ignoreSpaces) {
                // if ignore spaces is true and there are space chars provided
                while (spaceCharacters.Matches(input[index].ToString()).Count > 0) {
                    index++;
                    if (index >= input.Length) return default(object);
                }
            }
            
            // iterate over all the elements
            foreach (var def in _schema.Elements.Values.Cast<XmlSchemaElement>()) {
                var defId = def.Id;
                var type = def.ElementSchemaType;

                var pattern = type;
            }

            index++;
            return new {
                TypedValue = input[index - 1].ToString(CultureInfo.InvariantCulture),
                LineIndex = index - matchLength
            };
        }

        //private bool GetLexeme(string input, ref int index, ref int matchLength, simpleType def, out element lexeme, 
        //    RegexOptions opts = RegexOptions.Compiled)
        //{
        //    var regex = new Regex("");
        //    var match = regex.Match(input, index);

        //    lexeme = new element(new topLevelElement() {  });

        //    if (!match.Success || match.Index != index) return false;
        //    if (match.Length == 0) return false;

        //    index += match.Length;
        //    matchLength = match.Length;

        //    lexeme = new element(new topLevelElement() {  });
        //    return true;
        //}
    }
}
