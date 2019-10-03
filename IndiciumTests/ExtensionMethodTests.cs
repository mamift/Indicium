using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Indicium.Tests
{
    public class ExtensionMethodTests
    {
        [Test]
        public void RegexOptsFromStringTest1()
        {
            var list = new[] {
                nameof(RegexOptions.Compiled),
                nameof(RegexOptions.CultureInvariant),
                nameof(RegexOptions.ECMAScript),
                nameof(RegexOptions.ExplicitCapture),
                nameof(RegexOptions.IgnoreCase),
                nameof(RegexOptions.IgnorePatternWhitespace),
                nameof(RegexOptions.Multiline),
                nameof(RegexOptions.None),
                nameof(RegexOptions.RightToLeft),
                nameof(RegexOptions.Singleline)
            };
            
            var opts = list.Select(s => s.RegexOptionsFromString()).ToList();

            Assert.IsNotEmpty(opts);
            Assert.IsTrue(opts.Count == 10);
            Assert.IsTrue(opts.Distinct().Count() == 10);
        }

        [Test]
        public void RegexOptsFromStringTest2()
        {
            var list = new[] {
                "compiled",
                "cultureinvariant",
                "ecmascript",
                "explicitcapture",
                "ignorecase",
                "ignorepatternwhitespace",
                "multiline",
                "none",
                "righttoleft",
                "singleline",
            };

            var opts = list.Select(s => s.RegexOptionsFromString()).ToList();

            Assert.IsNotEmpty(opts);
            Assert.IsTrue(opts.Count == 10);
            Assert.IsTrue(opts.Distinct().Count() == 10);
        }

        [Test]
        public void RegexOptsFromStringTest3()
        {
            var list = new[] {
                "compiled",
                "CULTUREINVARIANT",
                "ecmascript",
                "explicitcapture",
                "ignorecase",
                "IGNOREPATTERNWHITESPACE",
                "multiline",
                "None",
                "righttoleft",
                "singleline",
            };

            var opts = list.Select(s => s.RegexOptionsFromString()).ToList();

            Assert.IsNotEmpty(opts);
            Assert.IsTrue(opts.Count == 10);
            Assert.IsTrue(opts.Distinct().Count() == 10);
        }
    }
}