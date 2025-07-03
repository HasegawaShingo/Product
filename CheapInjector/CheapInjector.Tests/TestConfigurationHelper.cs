
using System.Xml.Linq;

namespace CheapInjector.Tests
{
    internal static class TestConfigurationHelper
    {
        public static string CreateConfigFile(string testName, Action<XElement, XElement> modifier)
        {
            var filePath = Path.Combine(Path.GetTempPath(), $"{testName}.xml");

            var root = new XElement("root");
            var implementLibraries = new XElement("ImplementLibraries");
            var definitions = new XElement("Definitions");

            root.Add(implementLibraries, definitions);

            modifier(implementLibraries, definitions);

            var doc = new XDocument(root);
            doc.Save(filePath);

            return filePath;
        }

        public static void CleanupConfigFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
