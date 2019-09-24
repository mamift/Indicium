using System.IO;
using System.Linq;
using System.Reflection;
using Alba.CsConsoleFormat.Fluent;
using Indicium;
using Indicium.Schemas;

namespace Indicia
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Colors.WriteLine($"Usage: {Path.GetFileName(Assembly.GetExecutingAssembly().Location)} ".White(),
                    "<tokenSchema.xml>".OnDarkBlue().White(), 
                    " ",
                    "<inputFileToBeTokenised.txt>".OnDarkRed().White());
                Colors.WriteLine("Will then output tokenised output to:".White());
                Colors.WriteLine("<inputFileToBeTokenised.txt>.output".Yellow());

                return 0;
            }

            var tokenDefinitionFile = args.First();

            var tokeniser = TokenContext.Load(tokenDefinitionFile);

            var inputTextFile = args[1];
            var reader = new StreamReader(File.Open(inputTextFile, FileMode.Open));

            var tokens = tokeniser.ProcessTokens(reader).ToList();

            var dirOfInputTextFile = Path.GetDirectoryName(inputTextFile);
            var outputPath = Path.Combine(dirOfInputTextFile, $"{inputTextFile}.output");

            Colors.WriteLine($"Output: ".Green(), outputPath.Yellow());
            File.WriteAllText(outputPath, tokens.ToDelimitedString());

            return 0;
        }
    }
}
