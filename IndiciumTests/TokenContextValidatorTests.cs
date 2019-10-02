using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Indicium.Schemas;
using NUnit.Framework;

namespace Indicium.Tests
{
    public class TokenContextValidatorTests
    {
        [Test]
        public void ValidateTest()
        {
            var resourceNames = typeof(TokenContext).Assembly.GetManifestResourceNames();
            
            Assert.IsNotEmpty(resourceNames);
            Assert.IsTrue(resourceNames.Length == 1);
            Assert.IsTrue(resourceNames.First() == "Indicium.Schemas.TokenSchema.xsd", "'Indicium.Schemas.TokenSchema.xsd' needs to be embedded!");

            var argsList = new List<ValidationEventArgs>();

            Assert.DoesNotThrow(() => argsList = TokenContext.Validate(@"Schemas\Prototype1.xml"));
        }
    }
}