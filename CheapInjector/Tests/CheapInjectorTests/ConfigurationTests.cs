using NUnit.Framework;
using CheapInjector;

namespace CheapInjectorTests
{
    public class ConfigurationTests
    {
        private string XmlFilePath = @"";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var result = Configuration.GetConfiguration(XmlFilePath);
        }
    }
}