using NUnit.Framework;
using CheapInjector;
using TestInterfaceA;
using TestInterfaceB;

namespace CheapInjectorTests
{
    internal class ImplementTests
    {
        private string XmlFilePath = @"";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Container.Initialize(XmlFilePath);
            var instance = Container.CreateInstance<IClassA>();
            var message = instance.GetString();
        }

        [Test]
        public void Test2()
        {
            Container.Initialize(XmlFilePath);
            var instance = Container.CreateInstance<IClassA>(new object[] { "test method" });
            var message = instance.GetString();
        }

        [Test]
        public void Test3()
        {
            Container.Initialize(XmlFilePath);
            var instance = Container.CreateInstance_Singleton<IClassA>();
            var message = instance.GetString();

            instance.Message = "test message";

            var reInstance = Container.CreateInstance_Singleton<IClassA>();
            var getMessage = reInstance.Message;
        }

        [Test]
        public void Test4()
        {
            Container.Initialize(XmlFilePath);
            var instanceA = Container.CreateInstance_Singleton<IClassA>();
            var messageA = instanceA.GetString();
            instanceA.Message = "test message A";

            var instanceB = Container.CreateInstance_Singleton<IClassA>(new object[] {"test message instanceB"});
            var messageB = instanceB.GetString();
            instanceB.Message = "test message B";

            var instanceA_Message = instanceA.Message;
        }

        [Test]
        public void Test5()
        {
            Container.Initialize(XmlFilePath);
            var instanceA = Container.CreateInstance<IClassA>();
            var messageA = instanceA.GetString();
            instanceA.Message = "test message A";

            var instanceB = Container.CreateInstance<IClassA>(new object[] { "test message instanceB" });
            var messageB = instanceB.GetString();
            instanceB.Message = "test message B";

            var instanceA_Message = instanceA.Message;
        }

        [Test]
        public void Test6()
        {
            Container.Initialize(XmlFilePath);
            var instanceA = Container.CreateInstance<IClassA>();
            var actual = Implement.HasDispose(instanceA);

            Assert.That(!actual);
        }

        [Test]
        public void Test7()
        {
            Container.Initialize(XmlFilePath);
            var instanceB = Container.CreateInstance<IClassB>();
            var actual = Implement.HasDispose(instanceB);

            Assert.That(actual);
        }

        [Test]
        public void Test8()
        {
            Container.Initialize(XmlFilePath);
            var instanceB = Container.CreateInstance<IClassB>();
            var actual = Implement.DisposeInstance(instanceB);

            Assert.That(actual);
        }

        [Test]
        public void Test9()
        {
            Container.Initialize(XmlFilePath);
            var instanceA = Container.CreateInstance<IClassA>("A class");
            var message = instanceA.GetString();
        }

        [Test]
        public void Test10()
        {
            Container.Initialize(XmlFilePath);
            var instanceA = Container.CreateInstance<IClassA>("1classA");
            var message = instanceA.GetString();
        }
    }
}
