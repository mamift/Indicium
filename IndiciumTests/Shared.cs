using System;
using System.Collections.Generic;
using System.IO;

namespace Indicium.Tests
{
    public static class Shared
    {
        public static readonly string Lyrics = File.ReadAllText("lyrics.txt");
        public static readonly FileInfo SimpleCSchema = new FileInfo(@"Schemas\SimpleC.xsd");
        public static readonly FileInfo SensibleSqlKeywordTypesSchema = new FileInfo(@"Schemas\SensibleSQL_KeywordTypes.xsd");
    }
}