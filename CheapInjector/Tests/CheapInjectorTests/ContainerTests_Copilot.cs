using NUnit.Framework;
using CheapInjector;
using CheapInjector.Entity;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace CheapInjectorTests
{
    [TestFixture]
    internal class ContainerTests_Copilot
    {
        private string configPath;

        [SetUp]
        public void SetUp()
        {
            configPath = Path.GetTempFileName();
            File.WriteAllText(configPath, @"
<root>
  <ImplementLibraries>
    <ImplementLibrary name='mscorlib' path='mscorlib.dll'/>
  </ImplementLibraries>
  <Definitions>
    <Definition alias='str' interfacename='System.ICloneable' libraryname='mscorlib' fullname='System.String'/>
  </Definitions>
</root>");
            // Containerの状態をリセットする場合はリセットメソッドを追加してください
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(configPath);
        }

        [Test]
        public void Initialize_ValidConfig_Succeeds()
        {
            Assert.DoesNotThrow(() => Container.Initialize(configPath));
        }

        [Test]
        public void Initialize_AlreadyInitialized_DoesNothing()
        {
            Container.Initialize(configPath);
            Assert.DoesNotThrow(() => Container.Initialize(configPath));
        }

        [Test]
        public void Initialize_NullPath_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Container.Initialize(null));
        }

        [Test]
        public void Initialize_FileNotFound_ThrowsFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => Container.Initialize("notfound.xml"));
        }

        [Test]
        public void CreateInstance_ThrowsIfNotInitialized()
        {
            Assert.Throws<Exception>(() => Container.CreateInstance<ICloneable>());
        }

        [Test]
        public void CreateInstance_ThrowsIfNotInterface()
        {
            Container.Initialize(configPath);
            Assert.Throws<ArgumentException>(() => Container.CreateInstance<string>());
        }

        [Test]
        public void CreateInstance_ThrowsIfNotRegistered()
        {
            Container.Initialize(configPath);
            Assert.Throws<Exception>(() => Container.CreateInstance<IDisposable>());
        }

        [Test]
        public void CreateInstance_ByAlias_Succeeds()
        {
            Container.Initialize(configPath);
            var instance = Container.CreateInstance<ICloneable>("str");
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.InstanceOf<ICloneable>());
        }

        [Test]
        public void CreateInstance_ByAlias_ThrowsIfAliasNotFound()
        {
            Container.Initialize(configPath);
            Assert.Throws<Exception>(() => Container.CreateInstance<ICloneable>("notfound"));
        }

        [Test]
        public void CreateInstance_Singleton_ReturnsSameInstance()
        {
            Container.Initialize(configPath);
            var a = Container.CreateInstance_Singleton<ICloneable>("str");
            var b = Container.CreateInstance_Singleton<ICloneable>("str");
            Assert.That(a, Is.SameAs(b));
        }

        [Test]
        public void DeleteSingletonInstance_RemovesInstance()
        {
            Container.Initialize(configPath);
            var a = Container.CreateInstance_Singleton<ICloneable>("str");
            var deleted = Container.DeleteSingletonInstance<ICloneable>("str");
            Assert.That(deleted, Is.True);
            var b = Container.CreateInstance_Singleton<ICloneable>("str");
            Assert.That(a, Is.Not.SameAs(b));
        }
    }
}
