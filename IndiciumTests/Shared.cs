using System;
using System.Collections.Generic;
using System.IO;

namespace Indicium.Tests
{
    public static class Shared
    {
        public static readonly string Lyrics = File.ReadAllText("lyrics.txt");
    }
}