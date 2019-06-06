using System.Text.RegularExpressions;

namespace Indicium
{
    public abstract class TokenBase
    {
        public virtual string Identifier { get; }

        public virtual Regex Regex { get; }
    }
}