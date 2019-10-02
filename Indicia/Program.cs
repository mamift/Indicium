using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Schema;
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
            Console.WriteLine($"Indicia - A command line tool for building simple lexers/tokenisers.\n");

            var parserResult = Parser.Default.ParseArguments<CommandLineOptions, CodeGenOptions, TokeniseOptions>(args);

            parserResult.WithParsed<CodeGenOptions>(GenerateCode);

            parserResult.WithParsed<TokeniseOptions>(Tokenise);
            
            return 0;
        }

        public static bool Validate(string inputXml)
        {
            var validationErrors = new List<ValidationEventArgs>();

            if (inputXml.IsEmpty()) {
                Console.WriteLine("Requires an input XML file!");
                return false;
            }

            Console.WriteLine($"Validating '{inputXml}'...");

            var progressReporter = new Progress<ValidationEventArgs>(e => {
                validationErrors.Add(e);
                var message = $"Line: {e.Exception.LineNumber}, Index: {e.Exception.LinePosition}, Message: {e.Message}\n";
                if (e.Severity == XmlSeverityType.Warning) Console.WriteLine(message);
                if (e.Severity == XmlSeverityType.Error) Console.WriteLine(message);
            });

            TokenContext.Validate(inputXml, progressReporter);

            return !validationErrors.Any();
        }

        public static void GenerateCode(CodeGenOptions opts)
        {
            var ok = Validate(opts.InputXml);
            Console.WriteLine("OI!");
            if (!ok) {
                Console.WriteLine("Validation failed! Exiting...");
                return;
            }

            if (opts.Output.IsEmpty()) opts.Output = $"{Path.GetFileNameWithoutExtension(opts.InputXml)}.cs";

            var fileStream = File.OpenRead(opts.InputXml);
            var streamReader = new StreamReader(fileStream);

            using (fileStream)
            using (streamReader) {
                var context = TokenContext.Load(streamReader);

                var code = context.GenerateCode();

                File.WriteAllText(opts.Output, code, Encoding.UTF8);
            }
        }

        public static void Tokenise(TokeniseOptions opts)
        {
            if (opts.Output.IsEmpty()) opts.Output = $"{Path.GetFileNameWithoutExtension(opts.InputXml)}_tokenised.xml";
            if (opts.TextFile.IsEmpty()) {
                Console.WriteLine("Input text file is required");
                return;
            }

            using (var streamReader = new StreamReader(File.OpenRead(opts.InputXml))) 
            using (var textFileReader = new StreamReader(File.OpenRead(opts.TextFile))) {
                var context = TokenContext.Load(streamReader);
                var lexemes = context.ProcessTokens(textFileReader);
                var xDoc = lexemes.ToXDocument();

                Console.WriteLine($"Outputting to: \n\t{opts.Output}");

                xDoc.Save(File.Create(opts.Output));
            }
        }
    }
}
