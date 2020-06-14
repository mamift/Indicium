using System.Linq;
using W3C.XSD;

namespace Indicium.Extensions
{
    /// <summary>
    /// Extensions to the <see cref="schema"/> type.
    /// </summary>
    public static class SchemaExtensions
    {
        /// <summary>
        /// Resolves any type references to concrete definitions of local simple or complex types and copies them to inside the <see cref="element"/>
        /// definition itself.
        /// <para>The <see cref="elementType.simpleType"/> or <see cref="elementType.complexType"/> properties should be not null
        /// after invoking this method.</para>
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="move">Set to true to remove the corresponding <see cref="simpleType"/> or <see cref="complexType"/> from the resulting
        /// schema's global simple or complex type definitions.</param>
        /// <returns></returns>
        public static schema CopyTypeDefinitionsInline(this schema schema, bool move = false)
        {
            var clonedSchema = (schema) schema.Clone();

            foreach (var el in clonedSchema.element) {
                if (el.Content.simpleType != null) continue;
                if (el.Content.complexType != null) continue;
                
                var possibleSimpleType = clonedSchema.simpleType.FirstOrDefault(s => s.Content.name == el.Content.type.Name);
                var possibleComplexType = clonedSchema.complexType.FirstOrDefault(s => s.Content.name == el.Content.type.Name);

                if (possibleComplexType != null) {
                    el.Content.complexType = (localComplexType) possibleComplexType.Content.Untyped;
                    if (move) clonedSchema.complexType.Remove(possibleComplexType);
                }

                if (possibleSimpleType != null) {
                    el.Content.simpleType = (localSimpleType) possibleSimpleType.Content.Untyped;
                    if (move) clonedSchema.simpleType.Remove(possibleSimpleType);
                }
            }

            return clonedSchema;
        }
    }
}