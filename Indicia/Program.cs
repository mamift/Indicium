using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Alba.CsConsoleFormat.Fluent;
using Indicium;
using Indicium.Schemas;
using MicroBatchFramework;

namespace Indicia
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var hostBuilder = BatchHost.CreateDefaultBuilder();

            Colors.WriteLine($"Indicia - A command line tool for building simple lexers/tokenisers.\n".White());
            await hostBuilder.RunBatchEngineAsync<IndiciaBatch>(args).ConfigureAwait(false);

            return 0;
        }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global"), SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class IndiciaBatch : BatchBase
    {
        private readonly List<ValidationEventArgs> _validationErrors = new List<ValidationEventArgs>();

        public void Validate(string inputXml)
        {
            Colors.WriteLine($"Validating '{inputXml}'...");

            var progressReporter = new Progress<ValidationEventArgs>(e => {
                _validationErrors.Add(e);
                var message = $"Line: {e.Exception.LineNumber}, Index: {e.Exception.LinePosition}, Message: {e.Message}\n";
                if (e.Severity == XmlSeverityType.Warning) Colors.WriteLine(message.Yellow());
                if (e.Severity == XmlSeverityType.Error) Colors.WriteLine(message.Red());
            });

            TokenContext.Validate(inputXml, progressReporter);
        }

        [Command("codegen")]
        public void GenerateCode(
            [Option(0, "Input XML file representing token definitions.")] string inputXml,
            [Option("o", " Optional output .CS file containing generated code.")] string outputCs = null)
        {
            Validate(inputXml);
            if (_validationErrors.Any()) {
                Colors.WriteLine("Validation failed! Exiting...".Yellow());
                return;
            }

            if (string.IsNullOrWhiteSpace(outputCs)) outputCs = $"{Path.GetFileNameWithoutExtension(inputXml)}.cs";

            var fileStream = File.OpenRead(inputXml);
            var streamReader = new StreamReader(fileStream);

            using (fileStream)
            using (streamReader) {
                var context = TokenContext.Load(streamReader);

                var code = context.GenerateCode();

                File.WriteAllText(outputCs, code, Encoding.UTF8);
            }
        }

        [Command("tokenise")]
        public void Tokenise(
            [Option(0, "Input XML file representing token definitions")] string inputXml,
            [Option("t", "Input text file containing the string to be tokenised.")] string textFile,
            [Option("o", " Optional output .XML file containing tokenised output.")] string outputXml = null)
        {
            if (string.IsNullOrWhiteSpace(outputXml)) outputXml = $"{Path.GetFileNameWithoutExtension(inputXml)}_tokenised.xml";

            using (var streamReader = new StreamReader(File.OpenRead(inputXml))) 
            using (var textFileReader = new StreamReader(File.OpenRead(textFile))) {
                var context = TokenContext.Load(streamReader);
                var lexemes = context.ProcessTokens(textFileReader);
                var xDoc = lexemes.ToXDocument();

                Colors.WriteLine("Outputting to: \n".Green(), $"\t{outputXml}".White());

                xDoc.Save(File.Create(outputXml));
            }
        }
    }
}
