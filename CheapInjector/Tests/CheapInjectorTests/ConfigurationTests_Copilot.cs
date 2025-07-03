using NUnit.Framework;
using CheapInjector;
using CheapInjector.Entity;
using System.IO;
using System.Xml.Linq;
using System;

namespace CheapInjectorTests
{
    [TestFixture]
    internal class ConfigurationTests_Copilot
    {

        private string validConfigPath;
        private string invalidConfigPath;
        private string duplicateAliasConfigPath;

        [SetUp]
        public void SetUp()
        {
            // テスト用のXMLファイルを動的に作成
            validConfigPath = Path.GetTempFileName();
            File.WriteAllText(validConfigPath, @"
<root>
  <ImplementLibraries>
    <ImplementLibrary name='LibA' path='LibA.dll'/>
    <ImplementLibrary name='LibB' path='LibB.dll'/>
  </ImplementLibraries>
  <Definitions>
    <Definition alias='A' interfacename='TestInterfaceA.IClassA' libraryname='LibA' fullname='TestLibA.ClassA'/>
    <Definition alias='B' interfacename='TestInterfaceB.IClassB' libraryname='LibB' fullname='TestLibB.ClassB'/>
  </Definitions>
</root>");

            invalidConfigPath = Path.GetTempFileName();
            File.WriteAllText(invalidConfigPath, "<root></root>");

            duplicateAliasConfigPath = Path.GetTempFileName();
            File.WriteAllText(duplicateAliasConfigPath, @"
<root>
  <ImplementLibraries>
    <ImplementLibrary name='LibA' path='LibA.dll'/>
  </ImplementLibraries>
  <Definitions>
    <Definition alias='A' interfacename='TestInterfaceA.IClassA' libraryname='LibA' fullname='TestLibA.ClassA'/>
    <Definition alias='A' interfacename='TestInterfaceA.IClassA' libraryname='LibA' fullname='TestLibA.ClassA2'/>
  </Definitions>
</root>");
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(validConfigPath);
            File.Delete(invalidConfigPath);
            File.Delete(duplicateAliasConfigPath);
        }

        [Test]
        public void GetConfiguration_ValidFile_ReturnsEntities()
        {
            var (libs, defs) = Configuration.GetConfiguration(validConfigPath);
            Assert.That(libs.Count, Is.EqualTo(2));
            Assert.That(defs.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetConfiguration_InvalidFile_ReturnsEmptyLists()
        {
            var (libs, defs) = Configuration.GetConfiguration(invalidConfigPath);
            Assert.That(libs.Count, Is.EqualTo(0));
            Assert.That(defs.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetConfiguration_DuplicateAlias_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Configuration.GetConfiguration(duplicateAliasConfigPath));
        }

        [Test]
        public void GetConfiguration_FileNotFound_ThrowsFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => Configuration.GetConfiguration("notfound.xml"));
        }
    }
}
