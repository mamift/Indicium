using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using Alba.CsConsoleFormat.Fluent;
using CommandLine;
using Indicium;
using Indicium.Schemas;
using Xml.Schema.Linq.Extensions;

namespace Indicia
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<CommandLineOptions, CodeGenOptions, TokeniseOptions>(args);

            parserResult.WithParsed<CodeGenOptions>(GenerateCode);
            parserResult.WithParsed<TokeniseOptions>(Tokenise);

            return 0;
        }

        /// <summary>
        /// Validates a given XML token definition file.
        /// </summary>
        /// <param name="inputXml"></param>
        /// <returns></returns>
        public static bool Validate(string inputXml)
        {
            if (inputXml.IsEmpty()) {
                Colors.WriteLine("Requires an input XML file!".Red());
                return false;
            }

            Colors.WriteLine($"Validating '{inputXml}'...");
            
            var validationErrors = TokenContext.Validate(inputXml);

            if (!validationErrors.Any()) return true;

            foreach (var error in validationErrors) {
                var message = $"Line: {error.Exception.LineNumber}, Index: {error.Exception.LinePosition}, Message: {error.Message}\n";
                if (error.Severity == XmlSeverityType.Warning) Colors.WriteLine($"Warning: {message}".Yellow());
                else
                if (error.Severity == XmlSeverityType.Error) Colors.WriteLine($"Error: {message}".Red());
            }

            return false;
        }

        /// <summary>
        /// Generates code from an input XML file.
        /// </summary>
        /// <param name="opts"></param>
        public static void GenerateCode(CodeGenOptions opts)
        {
            if (opts.Example.IsNotEmpty()) {
                var example = TokenContext.GenerateExample(opts.Example);
                var outputFileName = opts.Example.EndsWith(".xml") ? opts.Example : $"{opts.Example}.xml";
                Colors.WriteLine($"Generating example file. Ignoring other arguments. Saving to: ".DarkYellow(), outputFileName.White());
                example.Save(outputFileName);
                return;
            }

            var ok = Validate(opts.InputXml);
            if (!ok) {
                Colors.WriteLine("Validation failed! Exiting...".Yellow());
                return;
            }

            if (opts.Output.IsEmpty()) opts.Output = $"{Path.GetFileNameWithoutExtension(opts.InputXml)}.cs";

            var fileStream = File.OpenRead(opts.InputXml);
            var streamReader = new StreamReader(fileStream);

            using (fileStream)
            using (streamReader) {
                var context = TokenContext.Load(streamReader);
                Colors.WriteLine($"Outputting generated code to: ".White(), opts.Output.Green());
                var code = context.GenerateCode();

                File.WriteAllText(opts.Output, code, Encoding.UTF8);
            }
        }

        /// <summary>
        /// Tokenises a text file, using a given XML file.
        /// </summary>
        /// <param name="opts"></param>
        public static void Tokenise(TokeniseOptions opts)
        {
            if (opts.Output.IsEmpty()) opts.Output = $"{Path.GetFileNameWithoutExtension(opts.TextFile)}_tokenised.xml";
            if (opts.TextFile.IsEmpty()) {
                Colors.WriteLine("Input text file is required".Red());
                return;
            }

            using (var streamReader = new StreamReader(File.OpenRead(opts.InputXml))) 
            using (var textFileReader = new StreamReader(File.OpenRead(opts.TextFile))) {
                var context = TokenContext.Load(streamReader);
                var lexemes = context.ProcessTokens(textFileReader);
                var xDoc = lexemes.ToXDocument();

                Colors.WriteLine($"Outputting to: \n\t".White(), opts.Output.Green());

                xDoc.Save(File.Create(opts.Output));
            }
        }
    }
}
