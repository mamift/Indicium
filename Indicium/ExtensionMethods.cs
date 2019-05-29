using System.CodeDom;

namespace Indicium
{
    public static class ExtensionMethods
    {
        public static void AddMinimumNamespaces(this CodeNamespace cn)
        {
            var system = nameof(System);
            var text = nameof(System.Text);
            var regularExpressions = nameof(System.Text.RegularExpressions);
            cn.Imports.Add(new CodeNamespaceImport(system));
            cn.Imports.Add(new CodeNamespaceImport(nameof(Indicium)));
            cn.Imports.Add(new CodeNamespaceImport($"{system}.{text}"));
            cn.Imports.Add(new CodeNamespaceImport($"{system}.{text}.{regularExpressions}"));
        }
    }
}