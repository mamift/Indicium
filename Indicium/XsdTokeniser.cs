﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using Indicium.Schemas;
using W3C.XSD;
using RegexOptions = System.Text.RegularExpressions.RegexOptions;

namespace Indicium
{
    public partial class XsdTokeniser
    {
        private readonly schema _schema;

        public XsdTokeniser(schema schema)
        {
            _schema = schema;

            var includes = schema.include.ToList();
            var allSimpleTypes = _schema.simpleType.Select(st => st.Content.name);
            var regexes = _schema.simpleType.Select(st => st.Content.restriction.pattern);
            var elementsOfSimpleTypes = _schema.element.Where(e => e.Content.type != null).Select(e => e.Content.type);
            var complexTypeNames = _schema.complexType.Select(ct => ct.Content.name);
        }

        public object ExtractLexeme(string input, int inputIndex, bool ignoreSpaces, out int index, 
            out int matchLength, Regex spaceCharacters = null)
        {
            if (spaceCharacters == null) spaceCharacters = new Regex(@"\t|\s", RegexOptions.Compiled);

            index = inputIndex; // begin at given index of string
            matchLength = 0; // will reset to zero for each invocation of this method
            if (index >= input.Length) return default(Lexeme); // then we're at the end of the string, and can return;

            if (ignoreSpaces) {
                // if ignore spaces is true and there are space chars provided
                while (spaceCharacters.Matches(input[index].ToString()).Count > 0) {
                    index++;
                    if (index >= input.Length) return default;
                }
            }
            
            foreach (var def in _schema.element) {
                var pattern = _schema.simpleType.First(st => st.Content.id == def.Content.simpleType.id)
                    .Content.restriction.pattern.First();
                var regex = new Regex(pattern)
            }

            index++;
            return new {
                TypedValue = input[index - 1].ToString(CultureInfo.InvariantCulture),
                LineIndex = index - matchLength
            };
        }

        private bool GetLexeme(string input, ref int index, ref int matchLength, simpleType def, out element lexeme, 
            RegexOptions opts = RegexOptions.Compiled)
        {
            var regex = new Regex("");
            var match = regex.Match(input, index);

            lexeme = new element(new topLevelElement() {  });

            if (!match.Success || match.Index != index) return false;
            if (match.Length == 0) return false;

            index += match.Length;
            matchLength = match.Length;

            lexeme = new element(new topLevelElement() {  });
            return true;
        }
    }
}
