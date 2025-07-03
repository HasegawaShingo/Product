using CheapInjector;
using CheapInjector.Entity;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CheapInjectorTests
{
    [TestFixture]
    internal class ImplementTests_Copilot
    {

        [Test]
        public void LoadAssemblies_AlreadyLoaded_SetsIsLoading()
        {
            var list = new List<ImplementLibraryEntity>
            {
                new ImplementLibraryEntity { Name = "System.Private.CoreLib", LibraryPath = typeof(object).Assembly.Location, IsLoading = false }
            };
            var result = Implement.LoadAssemblies(list);
            Assert.That(result[0].IsLoading, Is.True);
        }

        [Test]
        public void LoadAssemblies_LoadFromPath_SetsIsLoading()
        {
            var asm = typeof(object).Assembly;
            var list = new List<ImplementLibraryEntity>
            {
                new ImplementLibraryEntity { Name = asm.GetName().Name, LibraryPath = asm.Location, IsLoading = false }
            };
            var result = Implement.LoadAssemblies(list);
            Assert.That(result[0].IsLoading, Is.True);
        }

        [Test]
        public void GetInstance_ThrowsIfNotImplementsInterface()
        {
            var def = new DefinitionsEntity
            {
                Alias = "test",
                InterfaceName = "System.IDisposable",
                LibararyName = "System.Private.CoreLib",
                FullName = "System.String"
            };
            Assert.Throws<Exception>(() => Implement.GetInstance<IDisposable>(def));
        }

        [Test]
        public void GetInstance_Success()
        {
            var def = new DefinitionsEntity
            {
                Alias = "test",
                InterfaceName = "System.ICloneable",
                LibararyName = "System.Private.CoreLib",
                FullName = "System.String"
            };
            var instance = Implement.GetInstance<ICloneable>(def);
            Assert.That(instance, Is.InstanceOf<ICloneable>());
        }

        [Test]
        public void GetSingletonInstance_ReturnsSameInstance()
        {
            var def = new DefinitionsEntity
            {
                Alias = "test",
                InterfaceName = "System.ICloneable",
                LibararyName = "System.Private.CoreLib",
                FullName = "System.String"
            };
            var a = Implement.GetSingletonInstance<ICloneable>(def);
            var b = Implement.GetSingletonInstance<ICloneable>(def);
            Assert.That(a, Is.SameAs(b));
        }

        [Test]
        public void HasDispose_ReturnsTrueOrFalse()
        {
            Assert.That(Implement.HasDispose(new MemoryStream()), Is.True);
            Assert.That(Implement.HasDispose("test"), Is.False);
        }

        [Test]
        public void DisposeInstance_CallsDispose()
        {
            var ms = new MemoryStream();
            Assert.That(Implement.DisposeInstance(ms), Is.True);
        }

        [Test]
        public void DisposeInstance_ThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => Implement.DisposeInstance(null));
        }
    }
}
