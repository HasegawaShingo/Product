using NUnit.Framework;
using CheapInjector;
using TestInterfaceA;
using TestInterfaceB;

namespace CheapInjectorTests
{
    internal class ContainerTests
    {
        private string XmlFilePath = @"";

        [OneTimeSetUp]
        public void Setup()
        {
            Container.Initialize(XmlFilePath);
        }

        [Test]
        public void CreateInstanceTest_Case1()
        {
            var target = Container.CreateInstance<IClassB>();
            var actual = target.GetString();
            var expected = "引数なしのコンストラクタでインスタンスを作成しました。";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateInstanceTest_Case2_WithArgs()
        {
            var target = Container.CreateInstance<IClassB>(new object[] { "引数を設定しました。" });
            var actual = target.GetString();
            var expected = "引数を設定しました。";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateInstanceTest_Case3_WithAlias()
        {
            var target = Container.CreateInstance<IClassA>("B classA");
            var actual = target.GetString();
            var expected = "TestLibBのClassAです。";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateInstanceTest_Case4_WithArgsAndAlias()
        {
            var target = Container.CreateInstance<IClassA>("A classA", new object[] { "エイリアスと引数の確認。" });
            var actual = target.GetString();
            var expected = "エイリアスと引数の確認。";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CreateInstance_SingletonTest_Case1()
        {
            var target = Container.CreateInstance_Singleton<IClassB>();
            target.Message = "シングルトンでインスタンス作成。";

            var retarget = Container.CreateInstance_Singleton<IClassB>();
            var actual = retarget.GetString();
            var expected = "シングルトンでインスタンス作成。";

            Assert.AreEqual(expected, actual);
        }
    }
}
