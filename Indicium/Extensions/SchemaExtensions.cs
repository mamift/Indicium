using System.Linq;
using W3C.XSD;

namespace Indicium.Extensions
{
    /// <summary>
    /// Extensions to the <see cref="schema"/> type.
    /// </summary>
    public static class SchemaExtensions
    {
        public static schema ResolveElementTypes(this schema schema)
        {
            var clonedSchema = (schema) schema.Clone();

            foreach (var el in clonedSchema.element) {
                if (el.Content.simpleType != null) continue;
                if (el.Content.complexType != null) continue;

                var correspondingTypeName = el.Content.type.Name;

                var possibleSimpleType = clonedSchema.simpleType.FirstOrDefault(s => s.Content.name == correspondingTypeName);
                var possibleComplexType = clonedSchema.complexType.FirstOrDefault(s => s.Content.name == correspondingTypeName);

                if (possibleComplexType != null) {
                    el.Content.complexType = (localComplexType) possibleComplexType.Content.Untyped;
                }

                if (possibleSimpleType != null) {
                    el.Content.simpleType = (localSimpleType) possibleSimpleType.Content.Untyped;
                }
            }

            return clonedSchema;
        }
    }
}