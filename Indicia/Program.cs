using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using Alba.CsConsoleFormat.Fluent;
using CommandLine;
using Indicium;
using Indicium.Extensions;
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

            var validationErrors = Array.Empty<ValidationEventArgs>();

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
                throw new NotImplementedException();
            }

            if (opts.Output.IsEmpty()) opts.Output = $"{Path.GetFileNameWithoutExtension(opts.InputXsd)}.cs";

            var fileStream = File.OpenRead(opts.InputXsd);
            var streamReader = new StreamReader(fileStream);

            using (fileStream)
            using (streamReader) {
                var schema = XmlSchema.Read(fileStream, (sender, args) => {
                    if (args.Exception != null) throw args.Exception;
                });

                var schemaSet = schema.ResolveIncludes(Path.GetDirectoryName(opts.InputXsd));


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

            using (var streamReader = new StreamReader(File.OpenRead(opts.InputXsd))) 
            using (var textFileReader = new StreamReader(File.OpenRead(opts.TextFile))) {
                throw new NotImplementedException();
            }
        }
    }
}
