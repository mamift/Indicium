using System;
using System.Linq;
using Indicium.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Xml.Fxt;

namespace Indicium.Tests.Extensions
{
    [TestFixture()]
    public class XmlSchemaExtensionsTests
    {
        [Test()]
        public void GetGlobalSimpleTypesTest()
        {
            var simpleC = Shared.GetSimpleCSchemaSet();

            var simpleTypes = simpleC.GetGlobalSimpleTypes().ToList();
            var allTypes = simpleC.GlobalXsdTypes().ToList();

            Assert.IsNotEmpty(allTypes);
            Assert.IsNotEmpty(simpleTypes);
            Assert.True(allTypes.Count > simpleTypes.Count);
        }
    }
}