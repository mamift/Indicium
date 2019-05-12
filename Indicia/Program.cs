using System.IO;
using System.Linq;
using Indicium.Tokens;

namespace Indicia
{
    class Program
    {
        static void Main(string[] args)
        {
            var grammarFile = args.First();

            var grammar = Grammar.Load(grammarFile);
            
        }
    }
}
