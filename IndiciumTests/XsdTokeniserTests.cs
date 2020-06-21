﻿using System.Linq;
using System.Xml;
using Indicium.Extensions;
using NUnit.Framework;
using Xml.Schema.Linq.Extensions;

namespace Indicium.Tests
{
    [TestFixture]
    public class XsdTokeniserTests
    {
        [Test]
        public void ConstructorTest()
        {
            var xsd = XmlReader.Create(Shared.SimpleCSchema.FullName).ToXmlSchemaSet();
            var tokeniser = new XsdTokeniser(xsd);
        }
    }
}