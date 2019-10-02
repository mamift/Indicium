using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace Indicia
{
    public class OptionsBase
    {
        [Value(0, HelpText = "Input XML file representing token definitions.")]
        public string InputXml { get; set; }

        [Value(1, HelpText = "Optional. Specify an output file.", Required = false)]
        public string Output { get; set; }
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("ReSharper", "IdentifierTypo"),
     SuppressMessage("ReSharper", "ClassNeverInstantiated.Global"), SuppressMessage("ReSharper", "InconsistentNaming")]
    public class CommandLineOptions
    {
        public CodeGenOptions codegen { get; set; }
        public TokeniseOptions tokenise { get; set; }
    }

    [Verb(nameof(CommandLineOptions.codegen), HelpText = "Generate code from an XML file representing token definitions.")]
    public class CodeGenOptions : OptionsBase { }

    [Verb(nameof(CommandLineOptions.tokenise), HelpText = "Tokenise some text file using a given XML token definition file.")]
    public class TokeniseOptions : OptionsBase
    {
        [Option('t', nameof(TextFile), HelpText = "Specify an input text file to tokenise.")]
        public string TextFile { get; set; }
    }
}