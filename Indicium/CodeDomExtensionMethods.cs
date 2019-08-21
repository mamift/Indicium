using System;
using System.CodeDom;

namespace Indicium
{
    public static class CodeDomExtensionMethods
    {
        /// <summary>
        /// Adds a get property that returns a default value.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeName"></param>
        /// <param name="propertyName"></param>
        /// <param name="attrs"></param>
        /// <returns></returns>
        public static CodeMemberProperty AddDefaultGetter(this CodeTypeDeclaration type, string typeName,
            string propertyName, MemberAttributes attrs = MemberAttributes.Public)
        {
            if (typeName == null) throw new ArgumentNullException(nameof(typeName));

            var tokenBaseType = new CodeTypeReference(new CodeTypeParameter(typeName));

            if (typeName == "string" || typeName == "System.String")
                tokenBaseType = new CodeTypeReference(typeof(string));

            var defaultProperty = new CodeMemberProperty {
                Type = tokenBaseType,
                Name = propertyName,
                Attributes = attrs,
                HasGet = true
            };

            if (!attrs.HasFlag(MemberAttributes.Abstract)) {
                defaultProperty.GetStatements.Add(new CodeMethodReturnStatement {
                    Expression = new CodeDefaultValueExpression {
                        Type = tokenBaseType
                    }
                });
            }

            type.Members.Add(defaultProperty);

            return defaultProperty;
        }

        /// <summary>
        /// Adds a get and set property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeName"></param>
        /// <param name="propertyName"></param>
        /// <param name="attrs"></param>
        /// <returns></returns>
        public static CodeMemberProperty AddDefaultGetterAndSetter(this CodeTypeDeclaration type, string typeName,
            string propertyName, MemberAttributes attrs = MemberAttributes.Public)
        {
            if (typeName == null) throw new ArgumentNullException(nameof(typeName));

            var tokenBaseType = new CodeTypeReference(new CodeTypeParameter(typeName));

            if (typeName == "string" || typeName == "System.String")
                tokenBaseType = new CodeTypeReference(typeof(string));

            var defaultProperty = new CodeMemberProperty {
                Type = tokenBaseType,
                Name = propertyName,
                Attributes = attrs,
                HasGet = true,
                HasSet = true
            };

            type.Members.Add(defaultProperty);

            return defaultProperty;
        }
    }
}