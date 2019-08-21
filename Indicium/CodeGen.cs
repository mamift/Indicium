using System.CodeDom;
using System.Text.RegularExpressions;

namespace Indicium
{
    public static class CodeGen
    {
        public static readonly string TokenContextClassName = "TokenContext";
        public static readonly string AbstractLexemeBaseClassName = "LexemeBase";
        public static readonly string AbstractTokenBaseClassName = "TokenBase";
        public static readonly string GenericTTokenTypeName = "TToken";

        public static CodeTypeReference TokenBaseCodeTypeReference => new CodeTypeReference(AbstractTokenBaseClassName);
        public static CodeTypeReference RegexCodeTypeReferenceByName => new CodeTypeReference(nameof(Regex));
        public static CodeTypeReference RegexCodeTypeReferenceByType => new CodeTypeReference(typeof(Regex));

        public static CodeMethodReturnStatement NewReturnThisFieldValueStatement(string fieldName)
        {
            return new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                fieldName));
        }
    }
}