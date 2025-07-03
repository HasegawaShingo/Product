using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using CheapInjector.Tests.TestImplementations;

namespace CheapInjector.Tests
{
    [TestFixture]
    public class ContainerTests
    {
        private string? _configFilePath;

        [TearDown]
        public void TearDown()
        {
            // Reset static fields in Container to ensure test isolation
            var type = typeof(Container);
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var field in fields)
            {
                if (field.Name == "definitionsEntities" || field.Name == "implementLibraryEntities")
                {
                    field.SetValue(null, null);
                }
                else if (field.Name == "IsInitialized")
                {
                    field.SetValue(null, false);
                }
            }

            if (_configFilePath != null)
            {
                TestConfigurationHelper.CleanupConfigFile(_configFilePath);
            }
        }

        private void SetupConfigFile(string testName, Action<System.Xml.Linq.XElement, System.Xml.Linq.XElement> modifier)
        {
            _configFilePath = TestConfigurationHelper.CreateConfigFile(testName, modifier);
        }

        [Test]
        public void Initialize_WithValidConfigFile_ShouldInitialize()
        {
            SetupConfigFile(nameof(Initialize_WithValidConfigFile_ShouldInitialize), (libs, defs) =>
            {
                libs.Add(new System.Xml.Linq.XElement("ImplementLibrary",
                    new System.Xml.Linq.XAttribute("name", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("path", Assembly.GetExecutingAssembly().Location)));
                defs.Add(new System.Xml.Linq.XElement("Definition",
                    new System.Xml.Linq.XAttribute("alias", "service1"),
                    new System.Xml.Linq.XAttribute("interfaceName", "CheapInjector.Tests.TestImplementations.IService"),
                    new System.Xml.Linq.XAttribute("libraryName", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("fullName", "CheapInjector.Tests.TestImplementations.ServiceImplementation")));
            });

            Assert.DoesNotThrow(() => Container.Initialize(_configFilePath));
        }

        [Test]
        public void Initialize_WithNullOrEmptyPath_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Container.Initialize(null));
            Assert.Throws<ArgumentNullException>(() => Container.Initialize(""));
            Assert.Throws<ArgumentNullException>(() => Container.Initialize("   "));
        }

        [Test]
        public void Initialize_WithNonExistentFile_ShouldThrowFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => Container.Initialize("nonexistent.xml"));
        }

        [Test]
        public void Initialize_WithDuplicateAlias_ShouldThrowArgumentException()
        {
            SetupConfigFile(nameof(Initialize_WithDuplicateAlias_ShouldThrowArgumentException), (libs, defs) =>
            {
                libs.Add(new System.Xml.Linq.XElement("ImplementLibrary",
                    new System.Xml.Linq.XAttribute("name", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("path", Assembly.GetExecutingAssembly().Location)));
                defs.Add(new System.Xml.Linq.XElement("Definition",
                    new System.Xml.Linq.XAttribute("alias", "service1"),
                    new System.Xml.Linq.XAttribute("interfaceName", "CheapInjector.Tests.TestImplementations.IService"),
                    new System.Xml.Linq.XAttribute("libraryName", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("fullName", "CheapInjector.Tests.TestImplementations.ServiceImplementation")));
                defs.Add(new System.Xml.Linq.XElement("Definition",
                    new System.Xml.Linq.XAttribute("alias", "service1"),
                    new System.Xml.Linq.XAttribute("interfaceName", "CheapInjector.Tests.TestImplementations.IService"),
                    new System.Xml.Linq.XAttribute("libraryName", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("fullName", "CheapInjector.Tests.TestImplementations.AnotherServiceImplementation")));
            });

            Assert.Throws<ArgumentException>(() => Container.Initialize(_configFilePath));
        }

        [Test]
        public void CreateInstance_WhenNotInitialized_ShouldThrowException()
        {
            Assert.Throws<Exception>(() => Container.CreateInstance<IService>());
        }

        [Test]
        public void CreateInstance_Simple_ShouldReturnNewInstance()
        {
            Initialize_WithValidConfigFile_ShouldInitialize();
            var instance1 = Container.CreateInstance<IService>("service1");
            var instance2 = Container.CreateInstance<IService>("service1");

            Assert.That(instance1, Is.Not.Null);
            Assert.That(instance1, Is.InstanceOf<ServiceImplementation>());
            Assert.That(instance2, Is.Not.SameAs(instance1));
        }

        [Test]
        public void CreateInstance_WithConstructorArgs_ShouldReturnNewInstance()
        {
            SetupConfigFile(nameof(CreateInstance_WithConstructorArgs_ShouldReturnNewInstance), (libs, defs) =>
            {
                libs.Add(new System.Xml.Linq.XElement("ImplementLibrary",
                    new System.Xml.Linq.XAttribute("name", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("path", Assembly.GetExecutingAssembly().Location)));
                defs.Add(new System.Xml.Linq.XElement("Definition",
                                        new System.Xml.Linq.XAttribute("alias", "serviceWithCtor"),
                    new System.Xml.Linq.XAttribute("interfaceName", "CheapInjector.Tests.TestImplementations.IServiceWithConstructor"),
                    new System.Xml.Linq.XAttribute("libraryName", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("fullName", "CheapInjector.Tests.TestImplementations.ServiceWithConstructor")));
            });
            Container.Initialize(_configFilePath);

            var message = "Hello from constructor";
                        var instance = Container.CreateInstance<IServiceWithConstructor>("serviceWithCtor", new object[] { message });

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Greet(), Is.EqualTo(message));
        }

        [Test]
        public void CreateInstance_Singleton_ShouldReturnSameInstance()
        {
            Initialize_WithValidConfigFile_ShouldInitialize();
            var instance1 = Container.CreateInstance_Singleton<IService>("service1");
            var instance2 = Container.CreateInstance_Singleton<IService>("service1");

            Assert.That(instance1, Is.Not.Null);
            Assert.That(instance1, Is.InstanceOf<ServiceImplementation>());
            Assert.That(instance2, Is.SameAs(instance1));
        }

        [Test]
        public void GetInstance_ShouldReturnSameSingletonInstance()
        {
            Initialize_WithValidConfigFile_ShouldInitialize();
            var instance1 = Container.CreateInstance_Singleton<IService>("service1");
            var instance2 = Container.GetInstance<IService>("service1");

            Assert.That(instance2, Is.SameAs(instance1));
        }

        [Test]
        public void DeleteSingletonInstance_ShouldRemoveInstance()
        {
            Initialize_WithValidConfigFile_ShouldInitialize();
            var instance1 = Container.CreateInstance_Singleton<IService>("service1");
            var result = Container.DeleteSingletonInstance<IService>("service1");
            var instance2 = Container.CreateInstance_Singleton<IService>("service1");

            Assert.That(result, Is.True);
            Assert.That(instance2, Is.Not.SameAs(instance1));
        }

        [Test]
        public void DeleteSingletonInstance_WithDisposable_ShouldDispose()
        {
            SetupConfigFile(nameof(DeleteSingletonInstance_WithDisposable_ShouldDispose), (libs, defs) =>
            {
                libs.Add(new System.Xml.Linq.XElement("ImplementLibrary",
                    new System.Xml.Linq.XAttribute("name", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("path", Assembly.GetExecutingAssembly().Location)));
                defs.Add(new System.Xml.Linq.XElement("Definition",
                    new System.Xml.Linq.XAttribute("alias", "disposable"),
                    new System.Xml.Linq.XAttribute("interfaceName", "CheapInjector.Tests.TestImplementations.IDisposableService"),
                    new System.Xml.Linq.XAttribute("libraryName", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("fullName", "CheapInjector.Tests.TestImplementations.DisposableService")));
            });
            Container.Initialize(_configFilePath);

            var instance = Container.CreateInstance_Singleton<IDisposableService>("disposable");
            Assert.That(instance.IsDisposed, Is.False);

            var result = Container.DeleteSingletonInstance<IDisposableService>("disposable");

            Assert.That(result, Is.True);
            Assert.That(instance.IsDisposed, Is.True);
        }

        [Test]
        public void CreateInstance_NoAlias_ShouldReturnInstance()
        {
            // Setup config with a definition that has no alias
            SetupConfigFile(nameof(CreateInstance_NoAlias_ShouldReturnInstance), (libs, defs) =>
            {
                libs.Add(new System.Xml.Linq.XElement("ImplementLibrary",
                    new System.Xml.Linq.XAttribute("name", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("path", Assembly.GetExecutingAssembly().Location)));
                defs.Add(new System.Xml.Linq.XElement("Definition",
                    // No alias attribute
                    new System.Xml.Linq.XAttribute("interfaceName", "CheapInjector.Tests.TestImplementations.IService"),
                    new System.Xml.Linq.XAttribute("libraryName", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("fullName", "CheapInjector.Tests.TestImplementations.ServiceImplementation")));
            });
            Container.Initialize(_configFilePath);

            var instance = Container.CreateInstance<IService>();
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance, Is.InstanceOf<ServiceImplementation>());
        }

        [Test]
        public void CreateInstance_Singleton_NoAlias_ShouldReturnSameInstance()
        {
            // Setup config with a definition that has no alias
            SetupConfigFile(nameof(CreateInstance_Singleton_NoAlias_ShouldReturnSameInstance), (libs, defs) =>
            {
                libs.Add(new System.Xml.Linq.XElement("ImplementLibrary",
                    new System.Xml.Linq.XAttribute("name", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("path", Assembly.GetExecutingAssembly().Location)));
                defs.Add(new System.Xml.Linq.XElement("Definition",
                    // No alias attribute
                    new System.Xml.Linq.XAttribute("interfaceName", "CheapInjector.Tests.TestImplementations.IService"),
                    new System.Xml.Linq.XAttribute("libraryName", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("fullName", "CheapInjector.Tests.TestImplementations.ServiceImplementation")));
            });
            Container.Initialize(_configFilePath);

            var instance1 = Container.CreateInstance_Singleton<IService>();
            var instance2 = Container.CreateInstance_Singleton<IService>();

            Assert.That(instance1, Is.Not.Null);
            Assert.That(instance2, Is.SameAs(instance1));
        }

        [Test]
        public void GetInstance_NoAlias_ShouldReturnSameInstance()
        {
            // Setup config with a definition that has no alias
            SetupConfigFile(nameof(GetInstance_NoAlias_ShouldReturnSameInstance), (libs, defs) =>
            {
                libs.Add(new System.Xml.Linq.XElement("ImplementLibrary",
                    new System.Xml.Linq.XAttribute("name", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("path", Assembly.GetExecutingAssembly().Location)));
                defs.Add(new System.Xml.Linq.XElement("Definition",
                    // No alias attribute
                    new System.Xml.Linq.XAttribute("interfaceName", "CheapInjector.Tests.TestImplementations.IService"),
                    new System.Xml.Linq.XAttribute("libraryName", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("fullName", "CheapInjector.Tests.TestImplementations.ServiceImplementation")));
            });
            Container.Initialize(_configFilePath);

            var instance1 = Container.CreateInstance_Singleton<IService>();
            var instance2 = Container.GetInstance<IService>();

            Assert.That(instance2, Is.SameAs(instance1));
        }

        [Test]
        public void DeleteSingletonInstance_NoAlias_ShouldRemoveInstance()
        {
            // Setup config with a definition that has no alias
            SetupConfigFile(nameof(DeleteSingletonInstance_NoAlias_ShouldRemoveInstance), (libs, defs) =>
            {
                libs.Add(new System.Xml.Linq.XElement("ImplementLibrary",
                    new System.Xml.Linq.XAttribute("name", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("path", Assembly.GetExecutingAssembly().Location)));
                defs.Add(new System.Xml.Linq.XElement("Definition",
                    // No alias attribute
                    new System.Xml.Linq.XAttribute("interfaceName", "CheapInjector.Tests.TestImplementations.IService"),
                    new System.Xml.Linq.XAttribute("libraryName", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("fullName", "CheapInjector.Tests.TestImplementations.ServiceImplementation")));
            });
            Container.Initialize(_configFilePath);

            var instance1 = Container.CreateInstance_Singleton<IService>();
            var result = Container.DeleteSingletonInstance<IService>();
            var instance2 = Container.CreateInstance_Singleton<IService>();

            Assert.That(result, Is.True);
            Assert.That(instance2, Is.Not.SameAs(instance1));
        }

        [Test]
        public void CreateInstance_NoAlias_WithArgs_ShouldReturnInstance()
        {
            SetupConfigFile(nameof(CreateInstance_NoAlias_WithArgs_ShouldReturnInstance), (libs, defs) =>
            {
                libs.Add(new System.Xml.Linq.XElement("ImplementLibrary",
                    new System.Xml.Linq.XAttribute("name", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("path", Assembly.GetExecutingAssembly().Location)));
                defs.Add(new System.Xml.Linq.XElement("Definition",
                    new System.Xml.Linq.XAttribute("interfaceName", "CheapInjector.Tests.TestImplementations.IServiceWithConstructor"),
                    new System.Xml.Linq.XAttribute("libraryName", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("fullName", "CheapInjector.Tests.TestImplementations.ServiceWithConstructor")));
            });
            Container.Initialize(_configFilePath);

            var message = "Hello from constructor (no alias)";
            var instance = Container.CreateInstance<IServiceWithConstructor>(new object[] { message });

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Greet(), Is.EqualTo(message));
        }

        [Test]
        public void CreateInstance_Singleton_NoAlias_WithArgs_ShouldReturnSameInstance()
        {
            SetupConfigFile(nameof(CreateInstance_Singleton_NoAlias_WithArgs_ShouldReturnSameInstance), (libs, defs) =>
            {
                libs.Add(new System.Xml.Linq.XElement("ImplementLibrary",
                    new System.Xml.Linq.XAttribute("name", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("path", Assembly.GetExecutingAssembly().Location)));
                defs.Add(new System.Xml.Linq.XElement("Definition",
                    new System.Xml.Linq.XAttribute("interfaceName", "CheapInjector.Tests.TestImplementations.IServiceWithConstructor"),
                    new System.Xml.Linq.XAttribute("libraryName", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("fullName", "CheapInjector.Tests.TestImplementations.ServiceWithConstructor")));
            });
            Container.Initialize(_configFilePath);

            var message = "Hello from singleton constructor (no alias)";
            var instance1 = Container.CreateInstance_Singleton<IServiceWithConstructor>(new object[] { message });
            var instance2 = Container.CreateInstance_Singleton<IServiceWithConstructor>(new object[] { message });

            Assert.That(instance1, Is.Not.Null);
            Assert.That(instance1.Greet(), Is.EqualTo(message));
            Assert.That(instance2, Is.SameAs(instance1));
        }

        [Test]
        public void Initialize_WithMissingAttributeInDefinition_ShouldSkipDefinition()
        {
            SetupConfigFile(nameof(Initialize_WithMissingAttributeInDefinition_ShouldSkipDefinition), (libs, defs) =>
            {
                libs.Add(new System.Xml.Linq.XElement("ImplementLibrary",
                    new System.Xml.Linq.XAttribute("name", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("path", Assembly.GetExecutingAssembly().Location)));
                // This definition is missing the 'fullName' attribute
                defs.Add(new System.Xml.Linq.XElement("Definition",
                    new System.Xml.Linq.XAttribute("alias", "badservice"),
                    new System.Xml.Linq.XAttribute("interfaceName", "CheapInjector.Tests.TestImplementations.IService"),
                    new System.Xml.Linq.XAttribute("libraryName", "CheapInjector.Tests")));
                // This is a valid definition
                defs.Add(new System.Xml.Linq.XElement("Definition",
                    new System.Xml.Linq.XAttribute("alias", "goodservice"),
                    new System.Xml.Linq.XAttribute("interfaceName", "CheapInjector.Tests.TestImplementations.IService"),
                    new System.Xml.Linq.XAttribute("libraryName", "CheapInjector.Tests"),
                    new System.Xml.Linq.XAttribute("fullName", "CheapInjector.Tests.TestImplementations.ServiceImplementation")));
            });

            Container.Initialize(_configFilePath);

            // The good service should be resolvable
            Assert.DoesNotThrow(() => Container.CreateInstance<IService>("goodservice"));

            // The bad service should not be resolvable
            var ex = Assert.Throws<Exception>(() => Container.CreateInstance<IService>("badservice"));
            Assert.That(ex.Message, Does.Contain("not registed"));
        }
    }
}